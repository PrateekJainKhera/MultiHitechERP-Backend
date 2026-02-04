using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class Machine
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? MachineType { get; set; }
        public string? Location { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; } = "Idle";
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
