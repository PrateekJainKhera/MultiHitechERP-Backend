using System;

namespace MultiHitechERP.API.Models.Orders
{
    /// <summary>
    /// Represents a customer order in the manufacturing system
    /// </summary>
    public class Order
    {
        // Identification
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? AdjustedDueDate { get; set; }

        // Customer & Product
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

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

        // Order Source & Agent
        public string OrderSource { get; set; } = "Direct"; // Direct, Agent, Dealer, Distributor
        public int? AgentCustomerId { get; set; } // FK to Customers (for agent orders)
        public decimal? AgentCommission { get; set; }

        // Scheduling
        public string SchedulingStrategy { get; set; } = "Due Date"; // Due Date, Priority Flag, Customer Importance, Resource Availability

        // Drawing Review (GATE)
        public string DrawingReviewStatus { get; set; } = "Pending";
        public string? DrawingReviewedBy { get; set; }
        public DateTime? DrawingReviewedAt { get; set; }
        public string? DrawingReviewNotes { get; set; }

        // Drawing Linkage
        public int? PrimaryDrawingId { get; set; } // FK to Drawings
        public string? DrawingSource { get; set; } // 'customer' or 'company'

        // Template Linkage
        public int? LinkedProductTemplateId { get; set; } // FK to ProductTemplates

        // Customer Requirements
        public string? CustomerMachine { get; set; } // e.g., "Flexo 8-Color"
        public string? MaterialGradeRemark { get; set; } // e.g., "A Grade", "B Grade"

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
