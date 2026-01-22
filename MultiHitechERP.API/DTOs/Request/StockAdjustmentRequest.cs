using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for recording stock adjustment
    /// </summary>
    public class StockAdjustmentRequest
    {
        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        public decimal Quantity { get; set; } // Can be positive or negative

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PerformedBy { get; set; } = string.Empty;
    }
}
