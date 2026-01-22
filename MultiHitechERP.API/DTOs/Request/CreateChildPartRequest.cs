using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new ChildPart
    /// </summary>
    public class CreateChildPartRequest
    {
        [Required]
        [StringLength(50)]
        public string ChildPartCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ChildPartName { get; set; } = string.Empty;

        // Parent Product Reference
        public int? ProductId { get; set; }

        // Classification
        [StringLength(50)]
        public string? PartType { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        // Description
        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Specification { get; set; }

        // Drawing Reference
        public int? DrawingId { get; set; }

        [StringLength(50)]
        public string? DrawingNumber { get; set; }

        // Process Template
        public int? ProcessTemplateId { get; set; }

        // Material
        public int? MaterialId { get; set; }

        [StringLength(50)]
        public string? MaterialCode { get; set; }

        [StringLength(100)]
        public string? MaterialGrade { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Weight { get; set; }

        [StringLength(20)]
        public string? UOM { get; set; }

        // Quantity per Parent Product
        public int? QuantityPerProduct { get; set; }

        // Make or Buy
        [StringLength(20)]
        public string? MakeOrBuy { get; set; }

        public int? PreferredSupplierId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}
