namespace MultiHitechERP.API.Models.Auth
{
    public class AuthPermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsAllowed { get; set; }
    }
}
