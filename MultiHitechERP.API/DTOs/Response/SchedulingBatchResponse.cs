using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    // ── Schedulable Orders (Step 1: order selection) ──────────────────────────

    /// <summary>
    /// An order that has at least one PLANNED job card (material issued, machine not yet assigned)
    /// </summary>
    public class SchedulableOrderResponse
    {
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public int PendingJobCardCount { get; set; }  // PLANNED job cards
        public int TotalJobCardCount { get; set; }
    }

    // ── Cross-Order Groups (Step 2: child-part view across orders) ────────────

    /// <summary>
    /// Top-level response: all child parts across selected orders, grouped by part → step
    /// </summary>
    public class CrossOrderGroupsResponse
    {
        public List<CrossOrderChildPartGroup> ChildParts { get; set; } = new();
    }

    /// <summary>
    /// One child part (e.g. "Ends") — may appear in multiple orders
    /// </summary>
    public class CrossOrderChildPartGroup
    {
        public string ChildPartName { get; set; } = string.Empty;
        public string CreationType { get; set; } = string.Empty;  // "ChildPart" | "Assembly"
        public List<CrossOrderProcessStep> Steps { get; set; } = new();
    }

    /// <summary>
    /// One process step (e.g. Step 1 - Turning) under a child part, listing ALL orders that need it
    /// </summary>
    public class CrossOrderProcessStep
    {
        public int StepNo { get; set; }
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string? ProcessCode { get; set; }
        public bool IsOsp { get; set; }
        public bool IsManual { get; set; }
        public int? ProcessCategoryId { get; set; }
        public string? ProcessCategoryName { get; set; }
        /// <summary>All job cards (from different orders) that need this step and are still PLANNED</summary>
        public List<CrossOrderJobCardItem> JobCards { get; set; } = new();
    }

    /// <summary>
    /// One job card in the cross-order view — represents one order's need for a process step
    /// </summary>
    public class CrossOrderJobCardItem
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public int Quantity { get; set; }
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        // If already scheduled (machine assigned)
        public int? ScheduleId { get; set; }
        public string? AssignedMachineName { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
    }

    // ── Batch Schedule Result ─────────────────────────────────────────────────

    public class BatchScheduleResult
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int? ScheduleId { get; set; }
        public string? Error { get; set; }
    }
}
