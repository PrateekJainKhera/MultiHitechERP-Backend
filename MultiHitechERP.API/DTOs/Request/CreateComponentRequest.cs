using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateComponentRequest
    {
        // PartNumber is auto-generated based on Category

        [Required(ErrorMessage = "Component name is required")]
        [StringLength(200, ErrorMessage = "Component name cannot exceed 200 characters")]
        public string ComponentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Manufacturer cannot exceed 200 characters")]
        public string? Manufacturer { get; set; }

        [StringLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
        public string? SupplierName { get; set; }

        public string? Specifications { get; set; }

        [Required(ErrorMessage = "Lead time days is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Lead time days must be a positive value")]
        public int LeadTimeDays { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public string Unit { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Minimum stock must be 0 or greater")]
        public decimal MinimumStock { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }
}
