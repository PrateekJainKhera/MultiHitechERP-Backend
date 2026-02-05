using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class DrawingResponse
    {
        public int Id { get; set; }

        // Basic Information
        public string DrawingNumber { get; set; } = string.Empty;
        public string DrawingName { get; set; } = string.Empty;
        public string DrawingType { get; set; } = string.Empty; // shaft, pipe, final, gear, bushing, roller, other
        public string? Revision { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string Status { get; set; } = string.Empty; // draft, approved, obsolete

        // File Information
        public string? FileName { get; set; }
        public string? FileType { get; set; } // pdf, image, dwg
        public string? FileUrl { get; set; }
        public decimal? FileSize { get; set; } // in KB

        // Manufacturing Dimensions (as JSON string)
        public string? ManufacturingDimensionsJSON { get; set; }

        // Linking to other entities
        public int? LinkedPartId { get; set; }
        public string? LinkedPartName { get; set; }
        public int? LinkedProductId { get; set; }
        public string? LinkedProductName { get; set; }
        public int? LinkedCustomerId { get; set; }
        public string? LinkedCustomerName { get; set; }
        public int? LinkedOrderId { get; set; }
        public string? LinkedOrderNo { get; set; }

        // Metadata
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
