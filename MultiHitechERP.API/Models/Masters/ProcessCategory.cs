using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a process category master record (Turning 1, Turning 2, Grinding, etc.)
    /// Groups related processes together for machine capability mapping
    /// </summary>
    public class ProcessCategory
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty; // e.g., TURN-1, GRIND-2
        public string CategoryName { get; set; } = string.Empty; // e.g., Turning 1, Grinding 2
        public string? Description { get; set; }

        // Status
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
