using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    // ── Step 1: Schedulable Orders ──────────────────────────────────────────────

    public class SchedulableOrderV2Response
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public int TotalJobCards { get; set; }
        public int MaterialIssuedCount { get; set; }   // JCs with material issued
        public int AlreadyScheduledCount { get; set; } // JCs already machine-assigned
        public int ReadyToScheduleCount { get; set; }  // MaterialIssued - AlreadyScheduled
    }

    // ── Step 2: Job Cards Grouped by Child Part ─────────────────────────────────

    public class ChildPartJobGroupResponse
    {
        public string ChildPartName { get; set; } = string.Empty;
        public string CreationType { get; set; } = "ChildPart";
        public List<JobCardForSchedulingResponse> JobCards { get; set; } = new();
    }

    public class JobCardForSchedulingResponse
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ChildPartName { get; set; }
        public string? CreationType { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string? ProcessCode { get; set; }
        public int? StepNo { get; set; }
        public int? ProcessCategoryId { get; set; }
        public string? ProcessCategoryName { get; set; }
        public bool IsOsp { get; set; }
        public bool IsManual { get; set; }
        public int Quantity { get; set; }
        public string Priority { get; set; } = "Medium";
        public bool MaterialIssued { get; set; }
        public bool IsAlreadyScheduled { get; set; }
        public bool IsRework { get; set; }
        public string JobCardType { get; set; } = "Normal";
        // Duration pre-calculated from process master
        public int SetupTimeMinutes { get; set; }
        public decimal CycleTimeMinutesPerPiece { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }

    // ── Step 4: Category Machine Suggestions ────────────────────────────────────

    public class CategoryMachineSuggestionResponse
    {
        public string CategoryKey { get; set; } = string.Empty;   // unique key for this category
        public int? ProcessCategoryId { get; set; }
        public string ProcessCategoryName { get; set; } = string.Empty;
        public bool IsOsp { get; set; }
        public bool IsManual { get; set; }
        public List<int> JobCardIds { get; set; } = new();
        public int TotalJobCards { get; set; }
        public int TotalEstimatedMinutes { get; set; }
        public decimal TotalEstimatedHours { get; set; }
        public List<MachineSuggestionResponse> SuggestedMachines { get; set; } = new();
    }

    // ── Step 5: Batch Create Results ────────────────────────────────────────────

    public class BatchScheduleV2Result
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public string? OrderNo { get; set; }
        public bool Success { get; set; }
        public int? ScheduleId { get; set; }
        public string? MachineName { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string? Error { get; set; }
    }
}
