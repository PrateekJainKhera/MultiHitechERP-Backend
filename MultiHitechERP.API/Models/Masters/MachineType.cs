using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class MachineType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
