using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ChildPartTemplateResponse
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
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
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public bool IsDefault { get; set; }

        // Approval
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
