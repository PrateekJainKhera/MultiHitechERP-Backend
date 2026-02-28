using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateMaterialRequest
    {
        [Required]
        public int Id { get; set; }

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

        // Length and Weight are NOT stored in master - actual values come from inventory
        // These are kept for backwards compatibility but set to 0
        [Range(0, double.MaxValue, ErrorMessage = "Length must be non-negative")]
        public decimal LengthInMM { get; set; } = 0;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Density must be greater than 0")]
        public decimal Density { get; set; }

        // Weight not stored in master - calculated from actual length in inventory
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be non-negative")]
        public decimal WeightKG { get; set; } = 0;

        // Scrap threshold: pieces shorter than this (in mm) are classified as scrap after cutting
        [Range(1, 9999, ErrorMessage = "Min length must be between 1 and 9999 mm")]
        public int MinLengthMM { get; set; } = 300;

        public bool IsActive { get; set; } = true;
    }
}
