using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a process routing template for a product
    /// </summary>
    public class ProcessTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;

        // Product Reference
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        // Child Part Reference (if template is for a child part)
        public int? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }

        // Description
        public string? Description { get; set; }

        // Type
        public string? TemplateType { get; set; } = "Standard";

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";
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
