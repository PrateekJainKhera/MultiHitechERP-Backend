using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    public class GetJobCardsForOrdersRequest
    {
        public List<int> OrderIds { get; set; } = new();
    }

    public class GetCategoryMachineSuggestionsRequest
    {
        public List<int> JobCardIds { get; set; } = new();
        public DateTime TargetDate { get; set; }
    }

    public class CreateScheduleV2Request
    {
        public int JobCardId { get; set; }
        public int? MachineId { get; set; }
        public int? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public int EstimatedDurationMinutes { get; set; }
        public bool IsOsp { get; set; }
        public bool IsManual { get; set; }
        public string? CreatedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class BatchCreateSchedulesV2Request
    {
        public List<CreateScheduleV2Request> Schedules { get; set; } = new();
    }

    public class BatchRescheduleRequest
    {
        public List<int> ScheduleIds { get; set; } = new();
        public int ShiftId { get; set; }
        public DateTime NewDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? RescheduledBy { get; set; }
    }

    public class CreateReworkJobCardRequest
    {
        public int ReworkQty { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }
}
