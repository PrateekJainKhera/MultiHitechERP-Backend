using System;

namespace MultiHitechERP.API.Models.Orders
{
    /// <summary>
    /// Represents an individual product line item within an order
    /// Supports multi-product orders where one order can have multiple items
    /// </summary>
    public class OrderItem
    {
        // Identity
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ItemSequence { get; set; } = string.Empty; // A, B, C, D...

        // Product Reference
        public int ProductId { get; set; }
        public string? ProductName { get; set; } // Denormalized for performance

        // Quantities
        public int Quantity { get; set; }
        public int QtyCompleted { get; set; }
        public int QtyRejected { get; set; }
        public int QtyInProgress { get; set; }
        public int QtyScrap { get; set; }

        // Item-Specific Scheduling
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent

        // Item-Specific Status
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Dispatched
        public string PlanningStatus { get; set; } = "Not Planned";

        // Drawing Linkage (Item-specific) - DEPRECATED: Moving to Product level
        public int? PrimaryDrawingId { get; set; }
        public string? DrawingSource { get; set; } // 'customer' or 'company'

        // Template Linkage
        public int? LinkedProductTemplateId { get; set; }

        // Production Tracking
        public string? CurrentProcess { get; set; }
        public string? CurrentMachine { get; set; }
        public string? CurrentOperator { get; set; }
        public DateTime? ProductionStartDate { get; set; }
        public DateTime? ProductionEndDate { get; set; }

        // Material Approval (Item-specific)
        public bool MaterialGradeApproved { get; set; }
        public DateTime? MaterialGradeApprovalDate { get; set; }
        public string? MaterialGradeApprovedBy { get; set; }
        public string? MaterialGradeRemark { get; set; }

        // Item-Specific Notes
        public string? Remarks { get; set; } // Optional remarks/notes for this product

        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
