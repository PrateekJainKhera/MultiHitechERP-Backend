using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>One row in the Production Dashboard orders list</summary>
    public class ProductionOrderSummary
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? ProductName { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }

        // Step progress
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public int InProgressSteps { get; set; }
        public int ReadySteps { get; set; }

        // Child part summary
        public int TotalChildParts { get; set; }
        public int CompletedChildParts { get; set; }  // parts where ReadyForAssembly = true

        // Derived status: Pending | InProgress | Completed
        public string ProductionStatus { get; set; } = "Pending";
    }

    /// <summary>Full detail for one order — used on /production/orders/[orderId]</summary>
    public class ProductionOrderDetail
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? ProductName { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }

        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public int InProgressSteps { get; set; }

        public List<ProductionChildPartGroup> ChildParts { get; set; } = new();
        public ProductionStepItem? Assembly { get; set; }
        public bool CanStartAssembly { get; set; }
    }

    public class ProductionChildPartGroup
    {
        public int? ChildPartId { get; set; }
        public string ChildPartName { get; set; } = string.Empty;
        public int TotalSteps { get; set; }
        public int CompletedSteps { get; set; }
        public bool IsReadyForAssembly { get; set; }
        public List<ProductionStepItem> Steps { get; set; } = new();
    }

    public class ProductionStepItem
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public int? StepNo { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }
        public bool IsOsp { get; set; }

        // Production execution
        public string ProductionStatus { get; set; } = "Pending";
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int Quantity { get; set; }
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }

        // From schedule
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public string? MachineName { get; set; }
        public string? MachineCode { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
    }
}
