using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for adding a BOM item
    /// </summary>
    public class AddBOMItemRequest
    {
        [Required]
        public int BOMId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LineNo { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemType { get; set; } = "Material";

        // For Material items
        public int? MaterialId { get; set; }

        // For Child Part items
        public int? ChildPartId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal QuantityRequired { get; set; }

        [StringLength(10)]
        public string? UOM { get; set; } = "KG";

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
