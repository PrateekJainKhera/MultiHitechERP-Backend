using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating a ChildPart
    /// </summary>
    public class UpdateChildPartRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ChildPartCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ChildPartName { get; set; } = string.Empty;

        // Parent Product Reference
        public Guid? ProductId { get; set; }

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
        public Guid? DrawingId { get; set; }

        [StringLength(50)]
        public string? DrawingNumber { get; set; }

        // Process Template
        public Guid? ProcessTemplateId { get; set; }

        // Material
        public Guid? MaterialId { get; set; }

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

        public Guid? PreferredSupplierId { get; set; }

        // Status
        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
