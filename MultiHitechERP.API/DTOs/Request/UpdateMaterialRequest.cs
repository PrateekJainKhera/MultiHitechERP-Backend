using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateMaterialRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Material name is required")]
        public string MaterialName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required")]
        public string Grade { get; set; } = string.Empty; // MaterialGrade: EN8, EN19, SS304, SS316, Alloy Steel

        [Required(ErrorMessage = "Shape is required")]
        public string Shape { get; set; } = string.Empty; // MaterialShape: Rod, Pipe, Forged

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Diameter must be greater than 0")]
        public decimal Diameter { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public decimal LengthInMM { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Density must be greater than 0")]
        public decimal Density { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be positive")]
        public decimal WeightKG { get; set; }
    }
}
