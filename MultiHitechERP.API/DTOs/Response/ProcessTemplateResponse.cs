using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessTemplateResponse
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
