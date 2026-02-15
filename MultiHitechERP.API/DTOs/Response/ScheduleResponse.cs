using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for machine schedule
    /// </summary>
    public class ScheduleResponse
    {
        public int Id { get; set; }

        // Job Card info
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }

        // Order info (for multi-product order support)
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }

        // Machine info (null for OSP steps)
        public int? MachineId { get; set; }
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }

        // Schedule times
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public int EstimatedDurationMinutes { get; set; }

        // Actual times
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? ActualDurationMinutes { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;

        // Scheduling metadata
        public string SchedulingMethod { get; set; } = string.Empty;
        public bool SuggestedBySystem { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        // Process info
        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }
        public string? ProcessCode { get; set; }

        // Rescheduling
        public bool IsRescheduled { get; set; }
        public int? RescheduledFromId { get; set; }
        public string? RescheduledReason { get; set; }

        // Notes
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
