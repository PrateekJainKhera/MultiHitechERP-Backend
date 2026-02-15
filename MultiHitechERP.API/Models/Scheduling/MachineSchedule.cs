using System;

namespace MultiHitechERP.API.Models.Scheduling
{
    /// <summary>
    /// Represents a time slot allocated to a machine for a job card
    /// </summary>
    public class MachineSchedule
    {
        public int Id { get; set; }

        // Job Card linkage
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }

        // Order linkage (for multi-product order support)
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }

        // Machine assignment (null = OSP — no machine assigned for outside service processes)
        public int? MachineId { get; set; }
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }

        // Scheduling information
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public int EstimatedDurationMinutes { get; set; }

        // Actual time tracking (populated during production)
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? ActualDurationMinutes { get; set; }

        // Status
        public string Status { get; set; } = "Scheduled";  // Scheduled | InProgress | Completed | Cancelled | Rescheduled

        // Scheduling metadata
        public string SchedulingMethod { get; set; } = "Semi-Automatic";
        public bool SuggestedBySystem { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        // Process info (denormalized)
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }

        // Rescheduling tracking
        public bool IsRescheduled { get; set; }
        public int? RescheduledFromId { get; set; }
        public string? RescheduledReason { get; set; }
        public DateTime? RescheduledAt { get; set; }
        public string? RescheduledBy { get; set; }

        // Notes
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
