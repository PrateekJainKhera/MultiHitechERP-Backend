using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents a material requisition request
    /// </summary>
    public class MaterialRequisition
    {
        public int Id { get; set; }
        public string RequisitionNo { get; set; } = string.Empty;
        public DateTime RequisitionDate { get; set; }

        // Reference
        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; } // For multi-product orders
        public string? ItemSequence { get; set; } // A, B, C, D...
        public string? CustomerName { get; set; }

        // Status
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }

        // Approval
        public string? RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
