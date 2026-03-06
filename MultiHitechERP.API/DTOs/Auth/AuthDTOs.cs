namespace MultiHitechERP.API.DTOs.Auth
{
    // ── Login ──────────────────────────────────────────────────────────────────

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string SessionToken { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsAdmin { get; set; }
        // permissions[module][action] = true/false
        public Dictionary<string, Dictionary<string, bool>> Permissions { get; set; } = new();
    }

    // ── Users ──────────────────────────────────────────────────────────────────

    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
    }

    public class UpdateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    // ── Roles ──────────────────────────────────────────────────────────────────

    public class RoleResponse
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // ── Permissions ────────────────────────────────────────────────────────────

    public class PermissionEntry
    {
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
    }

    public class SavePermissionsRequest
    {
        public List<PermissionEntry> Permissions { get; set; } = new();
    }
}
