using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string RollerType { get; set; } = string.Empty;

        // Dimensions
        public decimal? Diameter { get; set; }
        public decimal? Length { get; set; }
        public decimal? Weight { get; set; }

        // Material & Finish
        public string? MaterialGrade { get; set; }
        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }

        // Drawing Reference
        public string? DrawingNo { get; set; }
        public string? RevisionNo { get; set; }
        public int? DrawingId { get; set; }

        // Templates
        public int? ProcessTemplateId { get; set; }
        public int? ProductTemplateId { get; set; }

        // Pricing & Costing
        public decimal? StandardCost { get; set; }
        public decimal? SellingPrice { get; set; }

        // Production Planning
        public int? StandardLeadTimeDays { get; set; }
        public int? MinOrderQuantity { get; set; }

        // Classification
        public string? Category { get; set; }
        public string? ProductType { get; set; }

        // Additional Info
        public string? Description { get; set; }
        public string? HSNCode { get; set; }
        public string? UOM { get; set; }
        public string? Remarks { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
