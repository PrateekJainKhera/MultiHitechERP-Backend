using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateJobCardRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Job card number is required")]
        public string JobCardNo { get; set; } = string.Empty;

        public string CreationType { get; set; } = "Manual";

        // Order Reference
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Drawing
        public int? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }
        public string DrawingSelectionType { get; set; } = "auto";

        // Child Part
        public int? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }
        public int? ChildPartTemplateId { get; set; }

        // Process
        [Required(ErrorMessage = "Process ID is required")]
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public int? StepNo { get; set; }
        public int? ProcessTemplateId { get; set; }

        // Quantities
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public int InProgressQty { get; set; }

        // Status
        public string Status { get; set; } = "Pending";

        // Assignments
        public int? AssignedMachineId { get; set; }
        public string? AssignedMachineName { get; set; }
        public int? AssignedOperatorId { get; set; }
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

        // Manufacturing Dimensions
        public string? ManufacturingDimensions { get; set; }

        // Priority
        public string Priority { get; set; } = "Medium";

        // Scheduling
        public string ScheduleStatus { get; set; } = "Not Scheduled";
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }

        // Rework
        public bool IsRework { get; set; }
        public int? ReworkOrderId { get; set; }
        public int? ParentJobCardId { get; set; }

        public string? UpdatedBy { get; set; }

        // Version for optimistic locking
        public int Version { get; set; }
    }
}
