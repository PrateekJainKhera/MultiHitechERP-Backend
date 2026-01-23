using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProductTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Template name is required")]
        public string TemplateName { get; set; } = string.Empty;

        // Product Classification
        public string? ProductType { get; set; }
        public string? Category { get; set; }
        public string? RollerType { get; set; }

        // Description
        public string? Description { get; set; }

        // Process Reference
        public int? ProcessTemplateId { get; set; }
        public string? ProcessTemplateName { get; set; }

        // Estimates
        public int? EstimatedLeadTimeDays { get; set; }
        public decimal? StandardCost { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public bool IsDefault { get; set; }

        // Approval
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
