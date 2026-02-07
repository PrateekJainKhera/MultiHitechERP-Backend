using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Simplified Update Process request DTO
    /// </summary>
    public class UpdateProcessRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Process code is required")]
        public string ProcessCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Process name is required")]
        public string ProcessName { get; set; } = string.Empty;

        public string? Category { get; set; }

        // Legacy field - kept for backward compatibility
        public string? DefaultMachine { get; set; }

        // New FK field for machine relationship
        public int? DefaultMachineId { get; set; }
        public decimal? DefaultSetupTimeHours { get; set; }
        public decimal? DefaultCycleTimePerPieceHours { get; set; }

        public int? StandardSetupTimeMin { get; set; }
        public decimal? RestTimeHours { get; set; }
        public string? Description { get; set; }
        public bool IsOutsourced { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
