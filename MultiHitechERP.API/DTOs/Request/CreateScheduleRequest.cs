using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request body for POST /api/schedules/cross-order-groups
    /// </summary>
    public class CrossOrderGroupsRequest
    {
        public List<int> OrderIds { get; set; } = new();
    }

    /// <summary>
    /// Batch schedule request — one entry per job card
    /// </summary>
    public class BatchScheduleRequest
    {
        public List<CreateScheduleRequest> Schedules { get; set; } = new();
    }

    /// <summary>
    /// Request DTO for creating a new machine schedule
    /// </summary>
    public class CreateScheduleRequest
    {
        [Required(ErrorMessage = "Job Card ID is required")]
        public int JobCardId { get; set; }

        // MachineId = 0 when IsOsp = true (no machine for outside service processes)
        public int MachineId { get; set; }

        [Required(ErrorMessage = "Scheduled start time is required")]
        public DateTime ScheduledStartTime { get; set; }

        [Required(ErrorMessage = "Scheduled end time is required")]
        public DateTime ScheduledEndTime { get; set; }

        [Required(ErrorMessage = "Estimated duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute")]
        public int EstimatedDurationMinutes { get; set; }

        public string? SchedulingMethod { get; set; } = "Semi-Automatic";
        public bool SuggestedBySystem { get; set; }
        // True for Outside Service Process (OSP) steps — skips machine lookup and conflict check
        public bool IsOsp { get; set; }
        // True for manual (no-machine) in-house processes — same scheduling behaviour as OSP
        public bool IsManual { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }
}
