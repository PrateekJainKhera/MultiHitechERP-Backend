using MultiHitechERP.API.Models.Auth;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        // Users
        Task<AuthUser?> GetUserByUsernameAsync(string username);
        Task<AuthUser?> GetUserByIdAsync(int id);
        Task<IEnumerable<AuthUser>> GetAllUsersAsync();
        Task<int> CreateUserAsync(AuthUser user);
        Task UpdateUserAsync(AuthUser user);
        Task UpdatePasswordAsync(int userId, string passwordHash);
        Task DeleteUserAsync(int id);
        Task<bool> UsernameExistsAsync(string username);

        // Roles
        Task<IEnumerable<AuthRole>> GetAllRolesAsync();
        Task<AuthRole?> GetRoleByIdAsync(int id);
        Task<int> CreateRoleAsync(AuthRole role);
        Task UpdateRoleAsync(AuthRole role);
        Task DeleteRoleAsync(int id);
        Task<int> GetUserCountForRoleAsync(int roleId);

        // Permissions
        Task<IEnumerable<AuthPermission>> GetPermissionsForRoleAsync(int roleId);
        Task SavePermissionsAsync(int roleId, IEnumerable<AuthPermission> permissions);

        // Sessions
        Task<string> CreateSessionAsync(int userId);
        Task<AuthUser?> GetUserBySessionTokenAsync(string token);
        Task InvalidateSessionAsync(string token);

        // Admin seed
        Task<bool> AdminExistsAsync();
        Task SeedAdminAsync(string passwordHash, int adminRoleId);
    }
}
