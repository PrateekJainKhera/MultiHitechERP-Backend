using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateChildPartTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string TemplateCode { get; set; } = string.Empty;

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        [Required]
        public string ChildPartType { get; set; } = string.Empty;

        [Required]
        public string RollerType { get; set; } = string.Empty;

        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? Thickness { get; set; }

        [Required]
        public string DimensionUnit { get; set; } = "mm";

        // Manufacturing
        public List<CreateChildPartTemplateMaterialRequirementRequest> MaterialRequirements { get; set; } = new();
        public List<CreateChildPartTemplateProcessStepRequest> ProcessSteps { get; set; } = new();
        public decimal TotalStandardTimeHours { get; set; }

        // Notes
        public string? Description { get; set; }
        public string? TechnicalNotes { get; set; }
        public List<string> QualityCheckpoints { get; set; } = new();

        // Metadata
        public bool IsActive { get; set; } = true;
    }
}
