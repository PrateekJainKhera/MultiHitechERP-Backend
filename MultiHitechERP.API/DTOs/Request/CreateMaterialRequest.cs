using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateMaterialRequest
    {
        // MaterialCode is auto-generated based on Grade-Shape-Diameter

        [Required(ErrorMessage = "Material name is required")]
        public string MaterialName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Material type is required")]
        public string MaterialType { get; set; } = string.Empty; // MaterialType: Steel, Stainless Steel, Aluminum, Other

        [Required(ErrorMessage = "Grade is required")]
        public string Grade { get; set; } = string.Empty; // MaterialGrade: EN8, EN19, SS304, SS316, Alloy Steel

        [Required(ErrorMessage = "Shape is required")]
        public string Shape { get; set; } = string.Empty; // MaterialShape: Rod, Pipe, Forged, Sheet

        // Diameter: required for Rod, Pipe (outer), Forged. Validated in service.
        public decimal Diameter { get; set; }
        public decimal? InnerDiameter { get; set; }     // Pipe only
        public decimal? Width { get; set; }             // Sheet only

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public decimal LengthInMM { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Density must be greater than 0")]
        public decimal Density { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be positive")]
        public decimal WeightKG { get; set; }

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }
}
