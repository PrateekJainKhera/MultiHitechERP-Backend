using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for machine scheduling suggestions (semi-automatic scheduling)
    /// </summary>
    public class MachineSuggestionResponse
    {
        public int MachineId { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;

        // Capability info
        public decimal SetupTimeHours { get; set; }
        public decimal CycleTimePerPieceHours { get; set; }
        public int PreferenceLevel { get; set; }  // 1 = Best, 5 = Last Resort
        public decimal EfficiencyRating { get; set; }
        public bool IsPreferredMachine { get; set; }

        // Calculated times for this job
        public int EstimatedSetupMinutes { get; set; }
        public int EstimatedCycleMinutes { get; set; }
        public int TotalEstimatedMinutes { get; set; }

        // Next available time slot
        public DateTime? NextAvailableStart { get; set; }
        public DateTime? SuggestedStart { get; set; }
        public DateTime? SuggestedEnd { get; set; }

        // Current machine status
        public bool IsCurrentlyAvailable { get; set; }
        public int ScheduledJobsCount { get; set; }
        public string? CurrentStatus { get; set; }

        // Upcoming schedules (for visualization)
        public List<ScheduleSlot>? UpcomingSchedules { get; set; }
    }

    public class ScheduleSlot
    {
        public int ScheduleId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
