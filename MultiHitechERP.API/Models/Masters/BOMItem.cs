using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents individual items/components in a BOM
    /// </summary>
    public class BOMItem
    {
        public Guid Id { get; set; }
        public Guid BOMId { get; set; }
        public int LineNo { get; set; }

        // Item Type (Material, Sub-Assembly, Child Part)
        public string ItemType { get; set; } = "Material";

        // Material Reference (if ItemType = Material)
        public Guid? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }

        // Child Part Reference (if ItemType = Child Part)
        public Guid? ChildPartId { get; set; }
        public string? ChildPartCode { get; set; }
        public string? ChildPartName { get; set; }

        // Quantity
        public decimal QuantityRequired { get; set; }
        public string? UOM { get; set; } = "KG";

        // Length-Based (for rods/pipes)
        public decimal? LengthRequiredMM { get; set; }

        // Scrap Allowance
        public decimal? ScrapPercentage { get; set; }
        public decimal? ScrapQuantity { get; set; }

        // Wastage
        public decimal? WastagePercentage { get; set; }

        // Net Quantity (including scrap + wastage)
        public decimal? NetQuantityRequired { get; set; }

        // Reference
        public string? ReferenceDesignator { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
