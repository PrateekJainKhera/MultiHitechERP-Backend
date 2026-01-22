using System;

namespace MultiHitechERP.API.Models.Production
{
    /// <summary>
    /// Represents execution tracking for a job card
    /// </summary>
    public class JobCardExecution
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public string? OrderNo { get; set; }

        // Machine & Operator
        public int? MachineId { get; set; }
        public string? MachineName { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }

        // Time Tracking
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PausedTime { get; set; }
        public DateTime? ResumedTime { get; set; }
        public int? TotalTimeMin { get; set; }
        public int? IdleTimeMin { get; set; }

        // Quantity Tracking
        public int? QuantityStarted { get; set; }
        public int? QuantityCompleted { get; set; }
        public int? QuantityRejected { get; set; }
        public int? QuantityInProgress { get; set; }

        // Status
        public string ExecutionStatus { get; set; } = "Started";

        // Notes
        public string? Notes { get; set; }
        public string? IssuesEncountered { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
