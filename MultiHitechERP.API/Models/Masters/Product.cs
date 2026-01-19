using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a product master record (finished goods)
    /// </summary>
    public class Product
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        // Classification
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? ProductType { get; set; }

        // Specifications
        public string? Specification { get; set; }
        public string? Description { get; set; }
        public string? HSNCode { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Weight { get; set; }
        public string? UOM { get; set; } = "PCS";

        // Drawing Reference
        public Guid? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }

        // BOM Reference
        public Guid? BOMId { get; set; }

        // Process Route Reference
        public Guid? ProcessRouteId { get; set; }

        // Pricing
        public decimal? StandardCost { get; set; }
        public decimal? SellingPrice { get; set; }

        // Material
        public string? MaterialGrade { get; set; }
        public string? MaterialSpecification { get; set; }

        // Production
        public int? StandardBatchSize { get; set; }
        public int? MinOrderQuantity { get; set; }
        public int? LeadTimeDays { get; set; }

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
