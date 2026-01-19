using System;

namespace MultiHitechERP.API.Models.Planning
{
    /// <summary>
    /// Represents a job card for production execution
    /// </summary>
    public class JobCard
    {
        // Identity
        public Guid Id { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public string CreationType { get; set; } = "Auto-Generated";

        // Order Reference
        public Guid OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Drawing (REQUIRED)
        public Guid? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }
        public string DrawingSelectionType { get; set; } = "auto";

        // Child Part
        public Guid? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }
        public Guid? ChildPartTemplateId { get; set; }

        // Process
        public Guid ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public int? StepNo { get; set; }
        public Guid? ProcessTemplateId { get; set; }

        // Quantities
        public int Quantity { get; set; }
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public int InProgressQty { get; set; }

        // Status
        public string Status { get; set; } = "Pending";

        // Machine & Operator Assignment
        public Guid? AssignedMachineId { get; set; }
        public string? AssignedMachineName { get; set; }
        public Guid? AssignedOperatorId { get; set; }
        public string? AssignedOperatorName { get; set; }

        // Time Tracking
        public int? EstimatedSetupTimeMin { get; set; }
        public int? EstimatedCycleTimeMin { get; set; }
        public int? EstimatedTotalTimeMin { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? ActualTimeMin { get; set; }

        // Material
        public string MaterialStatus { get; set; } = "Pending";
        public DateTime? MaterialStatusUpdatedAt { get; set; }

        // Manufacturing Dimensions (JSON string)
        public string? ManufacturingDimensions { get; set; }

        // Priority
        public string Priority { get; set; } = "Medium";

        // Scheduling
        public string ScheduleStatus { get; set; } = "Not Scheduled";
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }

        // Rework
        public bool IsRework { get; set; }
        public Guid? ReworkOrderId { get; set; }
        public Guid? ParentJobCardId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public int Version { get; set; } = 1;
    }
}
