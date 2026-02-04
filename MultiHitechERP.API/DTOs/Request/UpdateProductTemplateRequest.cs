using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProductTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }

        [Required]
        public int ProcessTemplateId { get; set; }

        // Final Product Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? CoreDiameter { get; set; }
        public decimal? ShaftDiameter { get; set; }
        public decimal? Weight { get; set; }
        public string? DimensionUnit { get; set; }

        public string? TechnicalNotes { get; set; }
        public string? QualityCheckpoints { get; set; }

        // BOM Items
        public List<ProductTemplateBOMItemRequest> BomItems { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public string? UpdatedBy { get; set; }
    }
}
