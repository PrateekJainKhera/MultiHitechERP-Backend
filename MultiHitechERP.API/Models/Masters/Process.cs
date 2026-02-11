using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a manufacturing process master record (simplified schema)
    /// </summary>
    public class Process
    {
        public int Id { get; set; }
        public string ProcessCode { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string? Category { get; set; }

        // Legacy field - kept for backward compatibility
        public string? DefaultMachine { get; set; }

        // New FK fields for machine relationship
        public int? DefaultMachineId { get; set; }
        public decimal? DefaultSetupTimeHours { get; set; }
        public decimal? DefaultCycleTimePerPieceHours { get; set; }

        public int? StandardSetupTimeMin { get; set; }
        public decimal? RestTimeHours { get; set; }
        public string? Description { get; set; }
        public bool IsOutsourced { get; set; }
        public bool IsManual { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation properties (populated by JOIN queries)
        public string? DefaultMachineName { get; set; }
        public string? DefaultMachineCode { get; set; }
    }
}
