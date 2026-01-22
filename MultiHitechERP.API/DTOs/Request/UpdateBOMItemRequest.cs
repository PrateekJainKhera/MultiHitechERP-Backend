using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for updating a BOM item
    /// </summary>
    public class UpdateBOMItemRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Range(1, int.MaxValue)]
        public int LineNo { get; set; }

        [StringLength(50)]
        public string? ItemType { get; set; }

        public Guid? MaterialId { get; set; }

        public Guid? ChildPartId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal QuantityRequired { get; set; }

        [StringLength(10)]
        public string? UOM { get; set; }

        public decimal? LengthRequiredMM { get; set; }

        [Range(0, 100)]
        public decimal? ScrapPercentage { get; set; }

        public decimal? ScrapQuantity { get; set; }

        [Range(0, 100)]
        public decimal? WastagePercentage { get; set; }

        [StringLength(50)]
        public string? ReferenceDesignator { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
