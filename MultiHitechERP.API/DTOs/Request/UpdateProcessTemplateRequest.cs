using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProcessTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Template name is required")]
        public string TemplateName { get; set; } = string.Empty;

        // Product Reference
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        // Child Part Reference
        public int? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }

        // Description
        public string? Description { get; set; }

        // Type
        public string? TemplateType { get; set; }

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
