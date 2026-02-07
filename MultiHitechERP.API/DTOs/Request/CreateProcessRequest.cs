using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProcessRequest
    {
        // ProcessCode is auto-generated based on Category

        [Required(ErrorMessage = "Process name is required")]
        public string ProcessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;

        // Legacy field - kept for backward compatibility
        public string? DefaultMachine { get; set; }

        // New FK field for machine relationship
        public int? DefaultMachineId { get; set; }
        public decimal? DefaultSetupTimeHours { get; set; }
        public decimal? DefaultCycleTimePerPieceHours { get; set; }

        [Required(ErrorMessage = "Setup time is required")]
        public int StandardSetupTimeMin { get; set; }

        public decimal? RestTimeHours { get; set; }

        public string? Description { get; set; }

        public bool IsOutsourced { get; set; }

        public string? CreatedBy { get; set; }
    }
}
