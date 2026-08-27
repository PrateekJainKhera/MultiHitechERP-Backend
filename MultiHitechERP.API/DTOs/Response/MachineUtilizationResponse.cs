using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MachineUtilizationResponse
    {
        public int MachineId { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? MachineType { get; set; }
        public string MachineStatus { get; set; } = "Idle"; // master-data status (Idle/Maintenance/Breakdown/Active)

        // Real-time occupancy, derived from actual (not just planned) job progress
        public bool IsCurrentlyBusy { get; set; }
        public string? CurrentJobCardNo { get; set; }
        public string? CurrentProcessName { get; set; }
        public DateTime? CurrentJobExpectedFreeAt { get; set; }

        public decimal UtilizationPercentToday { get; set; }
        public int ScheduledJobsToday { get; set; }
        public int CompletedJobsToday { get; set; }
    }

    public class MachineScheduleJobResponse
    {
        public int ScheduleId { get; set; }
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public string? OrderNo { get; set; }
        public string? ItemSequence { get; set; }
        public string? ProcessName { get; set; }
        public string? ChildPartName { get; set; }
        public string? MachineModelName { get; set; }
        public int Quantity { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime ScheduledEndTime { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool FinishedEarly { get; set; }
    }

    public class MachineDailyScheduleResponse
    {
        public int MachineId { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? MachineType { get; set; }
        public List<MachineScheduleJobResponse> Jobs { get; set; } = new();
    }
}
