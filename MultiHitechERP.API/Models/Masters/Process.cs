using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a manufacturing process master record
    /// </summary>
    public class Process
    {
        public int Id { get; set; }
        public string ProcessCode { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string? Category { get; set; }

        // Process Category (links process to a category for capacity scheduling)
        public int? ProcessCategoryId { get; set; }

        // Time fields used for scheduling
        public int? StandardSetupTimeMin { get; set; }           // One-time setup per job (minutes)
        public decimal? CycleTimePerPieceHours { get; set; }     // Time per piece (hours) — used in scheduling
        public decimal? RestTimeHours { get; set; }              // Optional cooling/rest after process (hours)

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
        public string? ProcessCategoryName { get; set; }
    }
}
