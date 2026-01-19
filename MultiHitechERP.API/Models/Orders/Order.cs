using System;

namespace MultiHitechERP.API.Models.Orders
{
    /// <summary>
    /// Represents a customer order in the manufacturing system
    /// </summary>
    public class Order
    {
        // Identification
        public Guid Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? AdjustedDueDate { get; set; }

        // Customer & Product
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }

        // Quantities
        public int Quantity { get; set; }
        public int OriginalQuantity { get; set; }
        public int QtyCompleted { get; set; }
        public int QtyRejected { get; set; }
        public int QtyInProgress { get; set; }
        public int QtyScrap { get; set; }

        // Status
        public string Status { get; set; } = "Pending";
        public string Priority { get; set; } = "Medium";
        public string PlanningStatus { get; set; } = "Not Planned";

        // Drawing Review (GATE)
        public string DrawingReviewStatus { get; set; } = "Pending";
        public string? DrawingReviewedBy { get; set; }
        public DateTime? DrawingReviewedAt { get; set; }
        public string? DrawingReviewNotes { get; set; }

        // Production Tracking
        public string? CurrentProcess { get; set; }
        public string? CurrentMachine { get; set; }
        public string? CurrentOperator { get; set; }
        public DateTime? ProductionStartDate { get; set; }
        public DateTime? ProductionEndDate { get; set; }

        // Rescheduling
        public string? DelayReason { get; set; }
        public int RescheduleCount { get; set; }

        // Material
        public bool MaterialGradeApproved { get; set; }
        public DateTime? MaterialGradeApprovalDate { get; set; }
        public string? MaterialGradeApprovedBy { get; set; }

        // Financial
        public decimal? OrderValue { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? BalancePayment { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Version { get; set; } = 1; // Optimistic locking
    }
}
