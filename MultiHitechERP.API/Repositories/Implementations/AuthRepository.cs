using System.Data;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Auth;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private IDbConnection Conn() => _connectionFactory.CreateConnection();

        public AuthRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ── Users ──────────────────────────────────────────────────────────────

        public async Task<AuthUser?> GetUserByUsernameAsync(string username)
        {
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<AuthUser>(@"
                SELECT u.*, r.RoleName
                FROM Auth_Users u
                LEFT JOIN Auth_Roles r ON r.Id = u.RoleId
                WHERE u.Username = @Username", new { Username = username });
        }

        public async Task<AuthUser?> GetUserByIdAsync(int id)
        {
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<AuthUser>(@"
                SELECT u.*, r.RoleName
                FROM Auth_Users u
                LEFT JOIN Auth_Roles r ON r.Id = u.RoleId
                WHERE u.Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<AuthUser>> GetAllUsersAsync()
        {
            using var db = Conn();
            return await db.QueryAsync<AuthUser>(@"
                SELECT u.*, r.RoleName
                FROM Auth_Users u
                LEFT JOIN Auth_Roles r ON r.Id = u.RoleId
                ORDER BY u.FullName");
        }

        public async Task<int> CreateUserAsync(AuthUser user)
        {
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(@"
                INSERT INTO Auth_Users (FullName, Username, PasswordHash, RoleId, IsAdmin, IsActive)
                VALUES (@FullName, @Username, @PasswordHash, @RoleId, 0, 1);
                SELECT SCOPE_IDENTITY();", user);
        }

        public async Task UpdateUserAsync(AuthUser user)
        {
            using var db = Conn();
            await db.ExecuteAsync(@"
                UPDATE Auth_Users
                SET FullName = @FullName, RoleId = @RoleId, IsActive = @IsActive
                WHERE Id = @Id", user);
        }

        public async Task UpdatePasswordAsync(int userId, string passwordHash)
        {
            using var db = Conn();
            await db.ExecuteAsync(@"
                UPDATE Auth_Users SET PasswordHash = @Hash WHERE Id = @Id",
                new { Hash = passwordHash, Id = userId });
        }

        public async Task DeleteUserAsync(int id)
        {
            using var db = Conn();
            await db.ExecuteAsync("DELETE FROM Auth_Users WHERE Id = @Id AND IsAdmin = 0", new { Id = id });
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Auth_Users WHERE Username = @Username",
                new { Username = username }) > 0;
        }

        // ── Roles ──────────────────────────────────────────────────────────────

        public async Task<IEnumerable<AuthRole>> GetAllRolesAsync()
        {
            using var db = Conn();
            return await db.QueryAsync<AuthRole>(
                "SELECT * FROM Auth_Roles ORDER BY RoleName");
        }

        public async Task<AuthRole?> GetRoleByIdAsync(int id)
        {
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<AuthRole>(
                "SELECT * FROM Auth_Roles WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateRoleAsync(AuthRole role)
        {
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(@"
                INSERT INTO Auth_Roles (RoleName, Description)
                VALUES (@RoleName, @Description);
                SELECT SCOPE_IDENTITY();", role);
        }

        public async Task UpdateRoleAsync(AuthRole role)
        {
            using var db = Conn();
            await db.ExecuteAsync(@"
                UPDATE Auth_Roles SET RoleName = @RoleName, Description = @Description
                WHERE Id = @Id", role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            using var db = Conn();
            await db.ExecuteAsync("DELETE FROM Auth_Roles WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> GetUserCountForRoleAsync(int roleId)
        {
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Auth_Users WHERE RoleId = @RoleId", new { RoleId = roleId });
        }

        // ── Permissions ────────────────────────────────────────────────────────

        public async Task<IEnumerable<AuthPermission>> GetPermissionsForRoleAsync(int roleId)
        {
            using var db = Conn();
            return await db.QueryAsync<AuthPermission>(
                "SELECT * FROM Auth_Permissions WHERE RoleId = @RoleId", new { RoleId = roleId });
        }

        public async Task SavePermissionsAsync(int roleId, IEnumerable<AuthPermission> permissions)
        {
            using var db = Conn();
            // Delete all existing permissions for this role and re-insert
            await db.ExecuteAsync("DELETE FROM Auth_Permissions WHERE RoleId = @RoleId", new { RoleId = roleId });

            foreach (var p in permissions)
            {
                await db.ExecuteAsync(@"
                    INSERT INTO Auth_Permissions (RoleId, Module, Action, IsAllowed)
                    VALUES (@RoleId, @Module, @Action, @IsAllowed)",
                    new { RoleId = roleId, p.Module, p.Action, p.IsAllowed });
            }
        }

        // ── Sessions ───────────────────────────────────────────────────────────

        public async Task<string> CreateSessionAsync(int userId)
        {
            var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            using var db = Conn();
            await db.ExecuteAsync(@"
                INSERT INTO Auth_Sessions (UserId, SessionToken, IsActive)
                VALUES (@UserId, @Token, 1)", new { UserId = userId, Token = token });
            return token;
        }

        public async Task<AuthUser?> GetUserBySessionTokenAsync(string token)
        {
            using var db = Conn();
            return await db.QueryFirstOrDefaultAsync<AuthUser>(@"
                SELECT u.*, r.RoleName
                FROM Auth_Sessions s
                JOIN Auth_Users u ON u.Id = s.UserId
                LEFT JOIN Auth_Roles r ON r.Id = u.RoleId
                WHERE s.SessionToken = @Token AND s.IsActive = 1 AND u.IsActive = 1",
                new { Token = token });
        }

        public async Task InvalidateSessionAsync(string token)
        {
            using var db = Conn();
            await db.ExecuteAsync(
                "UPDATE Auth_Sessions SET IsActive = 0 WHERE SessionToken = @Token",
                new { Token = token });
        }

        // ── Admin seed ─────────────────────────────────────────────────────────

        public async Task<bool> AdminExistsAsync()
        {
            using var db = Conn();
            return await db.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Auth_Users WHERE IsAdmin = 1") > 0;
        }

        public async Task SeedAdminAsync(string passwordHash, int adminRoleId)
        {
            using var db = Conn();
            await db.ExecuteAsync(@"
                INSERT INTO Auth_Users (FullName, Username, PasswordHash, RoleId, IsAdmin, IsActive)
                VALUES ('Administrator', 'admin', @PasswordHash, @RoleId, 1, 1)",
                new { PasswordHash = passwordHash, RoleId = adminRoleId });
        }
    }
}
