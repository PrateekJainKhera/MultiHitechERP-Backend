using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for Order entity
    /// </summary>
    public class OrderResponse
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? AdjustedDueDate { get; set; }

        // Customer & Product Info (joined data)
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerCode { get; set; }

        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }

        // Quantities
        public int Quantity { get; set; }
        public int OriginalQuantity { get; set; }
        public int QtyCompleted { get; set; }
        public int QtyRejected { get; set; }
        public int QtyInProgress { get; set; }
        public int QtyScrap { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string PlanningStatus { get; set; } = string.Empty;

        // Order Source & Agent
        public string OrderSource { get; set; } = string.Empty;
        public int? AgentCustomerId { get; set; }
        public decimal? AgentCommission { get; set; }

        // Scheduling Strategy
        public string SchedulingStrategy { get; set; } = string.Empty;

        // Drawing Review (GATE)
        public string DrawingReviewStatus { get; set; } = string.Empty;
        public string? DrawingReviewedBy { get; set; }
        public DateTime? DrawingReviewedAt { get; set; }
        public string? DrawingReviewNotes { get; set; }

        // Drawing & Template Linkage
        public int? PrimaryDrawingId { get; set; }
        public string? DrawingSource { get; set; }
        public int? LinkedProductTemplateId { get; set; }

        // Customer Requirements
        public string? CustomerMachine { get; set; }
        public string? MaterialGradeRemark { get; set; }

        // Current Production Info
        public string? CurrentProcess { get; set; }
        public string? CurrentMachine { get; set; }
        public string? CurrentOperator { get; set; }
        public DateTime? ProductionStartDate { get; set; }
        public DateTime? ProductionEndDate { get; set; }

        // Scheduling
        public string? DelayReason { get; set; }
        public int RescheduleCount { get; set; }
        public bool IsDelayed { get; set; }
        public int? DaysUntilDue { get; set; }

        // Material
        public bool MaterialGradeApproved { get; set; }
        public DateTime? MaterialGradeApprovalDate { get; set; }
        public string? MaterialGradeApprovedBy { get; set; }

        // Financial
        public decimal? OrderValue { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? BalancePayment { get; set; }

        // Progress
        public decimal CompletionPercentage { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Version { get; set; }
    }
}
