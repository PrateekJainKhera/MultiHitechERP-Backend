using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateDrawingRequest
    {
        [StringLength(100, ErrorMessage = "Drawing number cannot exceed 100 characters")]
        public string DrawingNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Drawing name is required")]
        [StringLength(200, ErrorMessage = "Drawing name cannot exceed 200 characters")]
        public string DrawingName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Drawing type is required")]
        [StringLength(50, ErrorMessage = "Drawing type cannot exceed 50 characters")]
        public string DrawingType { get; set; } = string.Empty; // shaft, pipe, final, gear, bushing, roller, other

        [StringLength(20, ErrorMessage = "Revision cannot exceed 20 characters")]
        public string? Revision { get; set; }

        public DateTime? RevisionDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "draft"; // draft, approved, obsolete

        // File Information (file will be uploaded separately)
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileUrl { get; set; }
        public decimal? FileSize { get; set; }

        // Manufacturing Dimensions as JSON string
        public string? ManufacturingDimensionsJSON { get; set; }

        // Linking to other entities
        public int? LinkedPartId { get; set; }
        public int? LinkedProductId { get; set; }
        public int? LinkedCustomerId { get; set; }
        public int? LinkedOrderId { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public string? Notes { get; set; }
    }
}
