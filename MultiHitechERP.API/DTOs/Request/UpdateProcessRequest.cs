using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProcessRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Process code is required")]
        public string ProcessCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Process name is required")]
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
        public bool IsActive { get; set; }
        public string? Status { get; set; }

        public string? Remarks { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
