using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a raw material master record
    /// </summary>
    public class Material
    {
        public Guid Id { get; set; }
        public string MaterialCode { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;

        // Classification
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? MaterialType { get; set; }

        // Specifications
        public string? Grade { get; set; }
        public string? Specification { get; set; }
        public string? Description { get; set; }
        public string? HSNCode { get; set; }

        // Dimensions (for rods, pipes, sheets)
        public decimal? StandardLength { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }

        // Unit of Measure
        public string? PrimaryUOM { get; set; } = "KG";
        public string? SecondaryUOM { get; set; }
        public decimal? ConversionFactor { get; set; }

        // Weight
        public decimal? WeightPerMeter { get; set; }
        public decimal? WeightPerPiece { get; set; }
        public decimal? Density { get; set; }

        // Pricing
        public decimal? StandardCost { get; set; }
        public decimal? LastPurchasePrice { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        // Inventory Control
        public decimal? MinStockLevel { get; set; }
        public decimal? MaxStockLevel { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? ReorderQuantity { get; set; }
        public int? LeadTimeDays { get; set; }

        // Supplier
        public Guid? PreferredSupplierId { get; set; }
        public string? PreferredSupplierName { get; set; }

        // Storage
        public string? StorageLocation { get; set; }
        public string? StorageConditions { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
