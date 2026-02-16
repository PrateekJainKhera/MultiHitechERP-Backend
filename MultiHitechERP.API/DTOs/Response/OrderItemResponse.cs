using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemSequence { get; set; } = string.Empty;  // A, B, C, D...

        // Product Info
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PartCode { get; set; }

        // Quantities
        public int Quantity { get; set; }
        public int OriginalQuantity { get; set; }
        public int QtyCompleted { get; set; }
        public int QtyRejected { get; set; }
        public int QtyInProgress { get; set; }
        public int QtyScrap { get; set; }

        // Dates & Priority
        public DateTime DueDate { get; set; }
        public DateTime? AdjustedDueDate { get; set; }
        public string Priority { get; set; } = "Medium";

        // Status
        public string Status { get; set; } = "Pending";

        // Drawing & Template Linkage (item-specific) - DEPRECATED: Moving to Product level
        public int? PrimaryDrawingId { get; set; }
        public int? LinkedProductTemplateId { get; set; }

        // Material Approval (item-specific)
        public bool MaterialGradeApproved { get; set; }
        public DateTime? MaterialGradeApprovalDate { get; set; }
        public string? MaterialGradeApprovedBy { get; set; }
        public string? MaterialGradeRemark { get; set; }

        // Item Notes
        public string? Remarks { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
