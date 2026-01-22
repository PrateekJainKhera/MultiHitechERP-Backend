using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MaterialResponse
    {
        public int Id { get; set; }
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

        // Dimensions
        public decimal? StandardLength { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }

        // Unit of Measure
        public string? PrimaryUOM { get; set; }
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
        public int? PreferredSupplierId { get; set; }
        public string? PreferredSupplierName { get; set; }

        // Storage
        public string? StorageLocation { get; set; }
        public string? StorageConditions { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
