using MultiHitechERP.API.DTOs.Auth;
using MultiHitechERP.API.Models.Auth;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string username, string password);
        Task LogoutAsync(string token);
        Task<AuthUser?> ValidateSessionAsync(string token);

        Task<IEnumerable<UserResponse>> GetUsersAsync();
        Task<UserResponse> CreateUserAsync(CreateUserRequest req);
        Task UpdateUserAsync(int id, UpdateUserRequest req);
        Task ResetPasswordAsync(int id, string newPassword);
        Task DeleteUserAsync(int id);

        Task<IEnumerable<RoleResponse>> GetRolesAsync();
        Task<RoleResponse> CreateRoleAsync(CreateRoleRequest req);
        Task UpdateRoleAsync(int id, UpdateRoleRequest req);
        Task DeleteRoleAsync(int id);

        Task<Dictionary<string, Dictionary<string, bool>>> GetPermissionsAsync(int roleId);
        Task SavePermissionsAsync(int roleId, SavePermissionsRequest req);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;

        // Defined modules and actions — single source of truth
        public static readonly string[] Modules =
        [
            "Dashboard", "Masters", "Sales", "Procurement", "Stores", "Production",
            "Quality", "Inventory", "Dispatch", "Planning", "Reports", "Admin"
        ];

        public static readonly string[] Actions = ["View", "Create", "Edit", "Delete", "Approve"];

        public AuthService(IAuthRepository repo)
        {
            _repo = repo;
        }

        // ── Auth ───────────────────────────────────────────────────────────────

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var user = await _repo.GetUserByUsernameAsync(username)
                       ?? throw new Exception("Invalid username or password");

            if (!user.IsActive)
                throw new Exception("Account is inactive. Contact admin.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            var token = await _repo.CreateSessionAsync(user.Id);
            var permissions = await BuildPermissionsMapAsync(user);

            return new LoginResponse
            {
                SessionToken = token,
                UserId = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                RoleId = user.RoleId,
                RoleName = user.RoleName,
                IsAdmin = user.IsAdmin,
                Permissions = permissions,
            };
        }

        public async Task LogoutAsync(string token)
        {
            await _repo.InvalidateSessionAsync(token);
        }

        public async Task<AuthUser?> ValidateSessionAsync(string token)
        {
            return await _repo.GetUserBySessionTokenAsync(token);
        }

        // ── Users ──────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UserResponse>> GetUsersAsync()
        {
            var users = await _repo.GetAllUsersAsync();
            return users.Select(u => MapUser(u));
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest req)
        {
            if (await _repo.UsernameExistsAsync(req.Username))
                throw new Exception("Username already exists");

            var user = new AuthUser
            {
                FullName = req.FullName,
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                RoleId = req.RoleId,
            };

            var id = await _repo.CreateUserAsync(user);
            var created = await _repo.GetUserByIdAsync(id);
            return MapUser(created!);
        }

        public async Task UpdateUserAsync(int id, UpdateUserRequest req)
        {
            var user = await _repo.GetUserByIdAsync(id)
                       ?? throw new Exception("User not found");

            user.FullName = req.FullName;
            user.RoleId = req.RoleId;
            user.IsActive = req.IsActive;
            await _repo.UpdateUserAsync(user);
        }

        public async Task ResetPasswordAsync(int id, string newPassword)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _repo.UpdatePasswordAsync(id, hash);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _repo.DeleteUserAsync(id);
        }

        // ── Roles ──────────────────────────────────────────────────────────────

        public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
        {
            var roles = await _repo.GetAllRolesAsync();
            var result = new List<RoleResponse>();
            foreach (var r in roles)
            {
                result.Add(new RoleResponse
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    UserCount = await _repo.GetUserCountForRoleAsync(r.Id),
                    CreatedAt = r.CreatedAt,
                });
            }
            return result;
        }

        public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest req)
        {
            var role = new AuthRole { RoleName = req.RoleName, Description = req.Description };
            var id = await _repo.CreateRoleAsync(role);
            var created = await _repo.GetRoleByIdAsync(id);
            return new RoleResponse { Id = created!.Id, RoleName = created.RoleName, Description = created.Description };
        }

        public async Task UpdateRoleAsync(int id, UpdateRoleRequest req)
        {
            var role = await _repo.GetRoleByIdAsync(id)
                       ?? throw new Exception("Role not found");
            role.RoleName = req.RoleName;
            role.Description = req.Description;
            await _repo.UpdateRoleAsync(role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            var count = await _repo.GetUserCountForRoleAsync(id);
            if (count > 0)
                throw new Exception($"Cannot delete role — {count} user(s) are assigned to it");
            await _repo.DeleteRoleAsync(id);
        }

        // ── Permissions ────────────────────────────────────────────────────────

        public async Task<Dictionary<string, Dictionary<string, bool>>> GetPermissionsAsync(int roleId)
        {
            var rows = await _repo.GetPermissionsForRoleAsync(roleId);
            return BuildMap(rows);
        }

        public async Task SavePermissionsAsync(int roleId, SavePermissionsRequest req)
        {
            var perms = req.Permissions.Select(p => new AuthPermission
            {
                RoleId = roleId,
                Module = p.Module,
                Action = p.Action,
                IsAllowed = p.IsAllowed,
            });
            await _repo.SavePermissionsAsync(roleId, perms);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private async Task<Dictionary<string, Dictionary<string, bool>>> BuildPermissionsMapAsync(AuthUser user)
        {
            // Admin gets all permissions
            if (user.IsAdmin)
            {
                return Modules.ToDictionary(
                    m => m,
                    m => Actions.ToDictionary(a => a, _ => true));
            }

            if (user.RoleId == null)
                return Modules.ToDictionary(m => m, m => Actions.ToDictionary(a => a, _ => false));

            var rows = await _repo.GetPermissionsForRoleAsync(user.RoleId.Value);
            return BuildMap(rows);
        }

        private static Dictionary<string, Dictionary<string, bool>> BuildMap(IEnumerable<AuthPermission> rows)
        {
            var map = Modules.ToDictionary(
                m => m,
                m => Actions.ToDictionary(a => a, _ => false));

            foreach (var p in rows)
            {
                if (map.ContainsKey(p.Module) && map[p.Module].ContainsKey(p.Action))
                    map[p.Module][p.Action] = p.IsAllowed;
            }

            return map;
        }

        private static UserResponse MapUser(AuthUser u) => new()
        {
            Id = u.Id,
            FullName = u.FullName,
            Username = u.Username,
            RoleId = u.RoleId,
            RoleName = u.RoleName,
            IsAdmin = u.IsAdmin,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
        };
    }
}
