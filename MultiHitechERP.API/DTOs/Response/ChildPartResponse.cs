using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for ChildPart
    /// </summary>
    public class ChildPartResponse
    {
        public Guid Id { get; set; }
        public string ChildPartCode { get; set; } = string.Empty;
        public string ChildPartName { get; set; } = string.Empty;

        // Parent Product Reference
        public Guid? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        // Classification
        public string? PartType { get; set; }
        public string? Category { get; set; }

        // Description
        public string? Description { get; set; }
        public string? Specification { get; set; }

        // Drawing Reference
        public Guid? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }

        // Process Template
        public Guid? ProcessTemplateId { get; set; }

        // Material
        public Guid? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialGrade { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Weight { get; set; }
        public string? UOM { get; set; }

        // Quantity per Parent Product
        public int? QuantityPerProduct { get; set; }

        // Make or Buy
        public string? MakeOrBuy { get; set; }
        public Guid? PreferredSupplierId { get; set; }

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
