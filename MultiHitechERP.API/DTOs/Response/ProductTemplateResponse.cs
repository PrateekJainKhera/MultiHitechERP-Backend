using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductTemplateResponse
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string RollerType { get; set; } = string.Empty;

        // BOM Items - child parts required for this product
        public List<ProductTemplateBOMItemResponse> BomItems { get; set; } = new();

        // Process sequence
        public int ProcessTemplateId { get; set; }
        public string ProcessTemplateName { get; set; } = string.Empty;

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ProductTemplateBOMItemResponse
    {
        public int Id { get; set; }
        public int ChildPartTemplateId { get; set; }
        public string ChildPartTemplateName { get; set; } = string.Empty;
        public string? ChildPartTemplateCode { get; set; }
        public string? ChildPartType { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public int? SequenceNumber { get; set; }
    }
}
