using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ChildPartTemplateResponse
    {
        public int Id { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string ChildPartType { get; set; } = string.Empty;
        public string RollerType { get; set; } = string.Empty;
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }

        // Manufacturing Process
        public int? ProcessTemplateId { get; set; }
        public bool IsPurchased { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? Thickness { get; set; }
        public string DimensionUnit { get; set; } = "mm";

        // Notes
        public string? Description { get; set; }
        public string? TechnicalNotes { get; set; }

        // Sub-entities
        public List<ChildPartTemplateMaterialRequirementResponse> MaterialRequirements { get; set; } = new();
        public List<ChildPartTemplateProcessStepResponse> ProcessSteps { get; set; } = new();

        // Metadata
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ChildPartTemplateMaterialRequirementResponse
    {
        public int Id { get; set; }
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;
        public decimal QuantityRequired { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal WastagePercent { get; set; }
    }

    public class ChildPartTemplateProcessStepResponse
    {
        public int Id { get; set; }
        public int? ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public int StepNumber { get; set; }
        public string? MachineName { get; set; }
        public decimal StandardTimeHours { get; set; }
        public decimal? RestTimeHours { get; set; }
        public string? Description { get; set; }
    }
}
