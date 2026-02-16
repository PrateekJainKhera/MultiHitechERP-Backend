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

        // Process Category - REQUIRED for capacity-based scheduling
        [Required(ErrorMessage = "Process category is required")]
        public int ProcessCategoryId { get; set; }

        [Required(ErrorMessage = "Setup time is required")]
        public int StandardSetupTimeMin { get; set; }

        [Required(ErrorMessage = "Cycle time per piece is required")]
        public decimal CycleTimePerPieceHours { get; set; }

        public decimal? RestTimeHours { get; set; }

        public string? Description { get; set; }

        public bool IsOutsourced { get; set; }

        public bool IsActive { get; set; }

        public string? Status { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
