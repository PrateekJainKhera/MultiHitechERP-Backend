using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateMachineRequest
    {
        [Required(ErrorMessage = "Machine name is required")]
        public string MachineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Machine type is required")]
        public string MachineType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        public string? Department { get; set; }
        public string? Status { get; set; } = "Idle";
        public string? Notes { get; set; }

        // Capacity & Scheduling
        [Range(0.1, 24.0, ErrorMessage = "Daily capacity must be between 0.1 and 24 hours")]
        public decimal DailyCapacityHours { get; set; } = 8.0m;
        public decimal? MaxLengthMM { get; set; }

        // Process Categories
        public List<int> ProcessCategoryIds { get; set; } = new List<int>();
    }

    public class UpdateMachineRequest : CreateMachineRequest
    {
        [Required]
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
