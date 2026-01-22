using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for recording quality inspection
    /// </summary>
    public class RecordInspectionRequest
    {
        [Required]
        public int JobCardId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity inspected must be greater than zero")]
        public int QuantityInspected { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityPassed { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int QuantityRejected { get; set; }

        [Range(0, int.MaxValue)]
        public int? QuantityRework { get; set; }

        [Required]
        [StringLength(100)]
        public string InspectedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string InspectionType { get; set; } = "In-Process";

        [StringLength(1000)]
        public string? DefectDescription { get; set; }

        [StringLength(100)]
        public string? DefectCategory { get; set; }

        [StringLength(500)]
        public string? MeasurementData { get; set; }

        [StringLength(500)]
        public string? CorrectiveAction { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
