using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a new machine schedule
    /// </summary>
    public class CreateScheduleRequest
    {
        [Required(ErrorMessage = "Job Card ID is required")]
        public int JobCardId { get; set; }

        [Required(ErrorMessage = "Machine ID is required")]
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
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }
}
