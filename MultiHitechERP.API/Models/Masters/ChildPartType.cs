using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class ChildPartType
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
