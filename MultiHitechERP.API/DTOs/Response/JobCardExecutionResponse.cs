using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for job card execution data
    /// </summary>
    public class JobCardExecutionResponse
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public string? OrderNo { get; set; }

        public int? MachineId { get; set; }
        public string? MachineName { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? PausedTime { get; set; }
        public DateTime? ResumedTime { get; set; }
        public int? TotalTimeMin { get; set; }
        public int? IdleTimeMin { get; set; }

        public int? QuantityStarted { get; set; }
        public int? QuantityCompleted { get; set; }
        public int? QuantityRejected { get; set; }
        public int? QuantityInProgress { get; set; }

        public string ExecutionStatus { get; set; } = string.Empty;

        public string? Notes { get; set; }
        public string? IssuesEncountered { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
