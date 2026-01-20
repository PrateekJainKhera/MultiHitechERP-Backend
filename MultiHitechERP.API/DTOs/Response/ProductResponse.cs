using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? ProductType { get; set; }
        public string? Specification { get; set; }
        public string? Description { get; set; }
        public string? HSNCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Weight { get; set; }
        public string? UOM { get; set; }
        public Guid? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }
        public Guid? BOMId { get; set; }
        public Guid? ProcessRouteId { get; set; }
        public decimal? StandardCost { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? MaterialGrade { get; set; }
        public string? MaterialSpecification { get; set; }
        public int? StandardBatchSize { get; set; }
        public int? MinOrderQuantity { get; set; }
        public int? LeadTimeDays { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
