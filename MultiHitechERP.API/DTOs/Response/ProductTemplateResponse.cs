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

        // Child parts required for this product
        public List<ProductTemplateChildPartResponse> ChildParts { get; set; } = new();

        // Process sequence
        public int ProcessTemplateId { get; set; }
        public string ProcessTemplateName { get; set; } = string.Empty;

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ProductTemplateChildPartResponse
    {
        public int Id { get; set; }
        public int ProductTemplateId { get; set; }
        public string ChildPartName { get; set; } = string.Empty;
        public string? ChildPartCode { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int SequenceNo { get; set; }
        public int? ChildPartTemplateId { get; set; }
    }
}
