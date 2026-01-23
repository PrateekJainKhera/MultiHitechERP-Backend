using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Product Template - Complete manufacturing recipe for a roller type
    /// Combines child parts BOM + process sequence in one template
    /// </summary>
    public class ProductTemplate
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string RollerType { get; set; } = string.Empty;

        // Process Reference
        public int ProcessTemplateId { get; set; }
        public string ProcessTemplateName { get; set; } = string.Empty;

        // Child parts (loaded separately via join)
        public List<ProductTemplateChildPart>? ChildParts { get; set; }

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
