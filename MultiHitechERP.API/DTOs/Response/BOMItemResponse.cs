using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for BOM Item
    /// </summary>
    public class BOMItemResponse
    {
        public int Id { get; set; }
        public int BOMId { get; set; }
        public int LineNo { get; set; }

        // Item Type
        public string ItemType { get; set; } = string.Empty;

        // Material Reference (if ItemType = Material)
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }

        // Child Part Reference (if ItemType = Child Part)
        public int? ChildPartId { get; set; }
        public string? ChildPartCode { get; set; }
        public string? ChildPartName { get; set; }

        // Quantity
        public decimal QuantityRequired { get; set; }
        public string? UOM { get; set; }

        // Length-Based (for rods/pipes)
        public decimal? LengthRequiredMM { get; set; }

        // Scrap Allowance
        public decimal? ScrapPercentage { get; set; }
        public decimal? ScrapQuantity { get; set; }

        // Wastage
        public decimal? WastageMM { get; set; }

        // Net Quantity (including scrap + wastage)
        public decimal? NetQuantityRequired { get; set; }

        // Reference
        public string? ReferenceDesignator { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
