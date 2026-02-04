using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProductTemplateRequest
    {
        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string? TemplateCode { get; set; }

        public string? Description { get; set; }

        [Required]
        public string RollerType { get; set; } = string.Empty;

        [Required]
        public int ProcessTemplateId { get; set; }

        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }

        // Final Product Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? CoreDiameter { get; set; }
        public decimal? ShaftDiameter { get; set; }
        public decimal? Weight { get; set; }
        public string? DimensionUnit { get; set; } = "mm";

        public string? TechnicalNotes { get; set; }
        public string? QualityCheckpoints { get; set; }

        // BOM Items - references to Child Part Templates
        public List<ProductTemplateBOMItemRequest> BomItems { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// BOM Item - Reference to a Child Part Template with quantity
    /// </summary>
    public class ProductTemplateBOMItemRequest
    {
        [Required]
        public int ChildPartTemplateId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public string? Notes { get; set; }

        public int? SequenceNumber { get; set; }
    }
}
