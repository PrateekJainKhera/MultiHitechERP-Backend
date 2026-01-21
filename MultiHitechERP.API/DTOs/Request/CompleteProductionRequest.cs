using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for completing production execution
    /// </summary>
    public class CompleteProductionRequest
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity completed cannot be negative")]
        public int QuantityCompleted { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity rejected cannot be negative")]
        public int? QuantityRejected { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? IssuesEncountered { get; set; }
    }
}
