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

        // Classification
        public string? ProcessType { get; set; }
        public string? Category { get; set; }
        public string? Department { get; set; }

        // Description
        public string? Description { get; set; }
        public string? ProcessDetails { get; set; }

        // Machine Requirements
        public string? MachineType { get; set; }
        public int? DefaultMachineId { get; set; }
        public string? DefaultMachineName { get; set; }

        // Time Standards
        public int? StandardSetupTimeMin { get; set; }
        public int? StandardCycleTimeMin { get; set; }
        public decimal? StandardCycleTimePerPiece { get; set; }

        // Skill Requirements
        public string? SkillLevel { get; set; }
        public int? OperatorsRequired { get; set; }

        // Cost
        public decimal? HourlyRate { get; set; }
        public decimal? StandardCostPerPiece { get; set; }

        // Quality
        public bool RequiresQC { get; set; }
        public string? QCCheckpoints { get; set; }

        // Outsourced
        public bool IsOutsourced { get; set; }
        public string? PreferredVendor { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
