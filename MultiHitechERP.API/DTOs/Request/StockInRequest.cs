using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for recording stock in transaction
    /// </summary>
    public class StockInRequest
    {
        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string GRNNo { get; set; } = string.Empty;

        public Guid? SupplierId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitCost { get; set; }

        [Required]
        [StringLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
