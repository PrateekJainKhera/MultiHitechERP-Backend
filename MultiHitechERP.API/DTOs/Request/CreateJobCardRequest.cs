using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateJobCardRequest
    {
        [Required(ErrorMessage = "Job card number is required")]
        public string JobCardNo { get; set; } = string.Empty;

        public string CreationType { get; set; } = "Manual";

        // Order Reference
        [Required(ErrorMessage = "Order ID is required")]
        public Guid OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Drawing (REQUIRED for manufacturing)
        public Guid? DrawingId { get; set; }
        public string? DrawingNumber { get; set; }
        public string? DrawingRevision { get; set; }
        public string DrawingSelectionType { get; set; } = "auto";

        // Child Part (optional - for assemblies)
        public Guid? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }
        public Guid? ChildPartTemplateId { get; set; }

        // Process
        [Required(ErrorMessage = "Process ID is required")]
        public Guid ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public int? StepNo { get; set; }
        public Guid? ProcessTemplateId { get; set; }

        // Quantity
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Optional Assignments
        public Guid? AssignedMachineId { get; set; }
        public string? AssignedMachineName { get; set; }
        public Guid? AssignedOperatorId { get; set; }
        public string? AssignedOperatorName { get; set; }

        // Time Estimates
        public int? EstimatedSetupTimeMin { get; set; }
        public int? EstimatedCycleTimeMin { get; set; }
        public int? EstimatedTotalTimeMin { get; set; }

        // Material Status
        public string MaterialStatus { get; set; } = "Pending";

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

        public string? CreatedBy { get; set; }

        // Dependencies (list of prerequisite job card IDs)
        public List<Guid>? PrerequisiteJobCardIds { get; set; }
    }
}
