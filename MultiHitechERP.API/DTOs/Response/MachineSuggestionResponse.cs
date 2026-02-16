using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for machine capacity suggestions
    /// </summary>
    public class MachineSuggestionResponse
    {
        public int MachineId { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? MachineType { get; set; }
        public string? Location { get; set; }
        public string? Department { get; set; }

        // Process Category
        public int ProcessCategoryId { get; set; }
        public string ProcessCategoryName { get; set; } = string.Empty;

        // Capacity Information
        public decimal DailyCapacityHours { get; set; }
        public decimal ScheduledHours { get; set; }
        public decimal AvailableHours { get; set; }
        public decimal UtilizationPercent { get; set; }

        // Status
        public string CapacityStatus { get; set; } = string.Empty; // Available, Moderate, Busy, Overloaded
        public bool IsBusy { get; set; }

        // Breakdown of scheduled work
        public int TotalJobCards { get; set; }
        public List<string> ScheduledJobCardNumbers { get; set; } = new List<string>();
    }

    /// <summary>
    /// Response wrapper for machine suggestions
    /// </summary>
    public class MachineSuggestionsResponse
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public int? ProcessCategoryId { get; set; }
        public string? ProcessCategoryName { get; set; }
        public string TargetDate { get; set; } = string.Empty;
        public List<MachineSuggestionResponse> Machines { get; set; } = new List<MachineSuggestionResponse>();
        public int TotalMachinesAvailable { get; set; }
        public int AvailableMachinesCount { get; set; }
        public int BusyMachinesCount { get; set; }
    }
}
