using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Material requirement for a job card (used during job card creation)
    /// </summary>
    public class JobCardMaterialRequirementRequest
    {
        // Material Reference
        public int? RawMaterialId { get; set; }

        [Required(ErrorMessage = "Material name is required")]
        public string RawMaterialName { get; set; } = string.Empty;

        public string MaterialGrade { get; set; } = string.Empty;

        // Quantity Required
        [Required(ErrorMessage = "Required quantity is required")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal RequiredQuantity { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; } = string.Empty;

        // Wastage (in millimeters)
        [Range(0, 1000, ErrorMessage = "Wastage must be between 0 and 1000 mm")]
        public decimal WastageMM { get; set; } = 5;

        // Source tracking
        public string Source { get; set; } = "Template"; // Template | Manual

        // Confirmation
        public string ConfirmedBy { get; set; } = string.Empty;
    }
}
