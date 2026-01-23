using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateChildPartTemplateRequest
    {
        [Required(ErrorMessage = "Template name is required")]
        public string TemplateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Child part type is required")]
        public string ChildPartType { get; set; } = string.Empty;

        // Description
        public string? Description { get; set; }
        public string? Category { get; set; }

        // Dimensions
        public decimal? Length { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        // Material & Process
        public string? MaterialType { get; set; }
        public string? MaterialGrade { get; set; }
        public int? ProcessTemplateId { get; set; }
        public string? ProcessTemplateName { get; set; }

        // Estimates
        public decimal? EstimatedCost { get; set; }
        public int? EstimatedLeadTimeDays { get; set; }
        public decimal? EstimatedWeight { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";
        public bool IsDefault { get; set; }

        // Approval
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
    }
}
