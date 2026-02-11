using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Full scheduling tree for an order: Order → Child Parts → Process Steps
    /// </summary>
    public class OrderSchedulingTreeResponse
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public int TotalSteps { get; set; }
        public int ScheduledSteps { get; set; }   // steps that have a machine assigned
        public int PendingSteps { get; set; }     // steps still needing machine assignment
        public List<ChildPartGroupResponse> Groups { get; set; } = new();
    }

    public class ChildPartGroupResponse
    {
        public string GroupName { get; set; } = string.Empty;  // child part name or "Assembly"
        public string CreationType { get; set; } = string.Empty; // "ChildPart" | "Assembly"
        public int TotalSteps { get; set; }
        public int ScheduledSteps { get; set; }
        public List<ProcessStepSchedulingItem> Steps { get; set; } = new();
    }

    public class ProcessStepSchedulingItem
    {
        // Job Card identity
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;

        // Process step info
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }
        public int? StepNo { get; set; }

        // OSP flag — true when this step is outsourced (no machine needed)
        public bool IsOsp { get; set; }
        // Manual flag — true when this step is a manual in-house process (no machine needed)
        public bool IsManual { get; set; }

        // Production info
        public int Quantity { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string JobCardStatus { get; set; } = string.Empty;

        // Machine assignment (null = not yet assigned; 0 = OSP, no machine)
        public int? ScheduleId { get; set; }
        public int? AssignedMachineId { get; set; }
        public string? AssignedMachineCode { get; set; }
        public string? AssignedMachineName { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public string? ScheduleStatus { get; set; }
        public int? EstimatedDurationMinutes { get; set; }
    }
}
