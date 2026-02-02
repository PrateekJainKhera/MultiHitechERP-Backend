using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a child part template for quick child part creation
    /// Standardizes child parts like shafts, sleeves, cores with predefined specs
    /// </summary>
    public class ChildPartTemplate
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string ChildPartType { get; set; } = string.Empty;
        public string RollerType { get; set; } = string.Empty;
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? Thickness { get; set; }
        public string DimensionUnit { get; set; } = "mm";

        // Manufacturing Process
        public int? ProcessTemplateId { get; set; }
        public bool IsPurchased { get; set; } = false;

        // Notes
        public string? Description { get; set; }
        public string? TechnicalNotes { get; set; }

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
