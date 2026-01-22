using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for stock reconciliation/physical count
    /// </summary>
    public class StockReconciliationRequest
    {
        [Required]
        public int MaterialId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ActualQuantity { get; set; }

        [Required]
        [StringLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }
}
