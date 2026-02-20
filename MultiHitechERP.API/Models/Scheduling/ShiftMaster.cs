using System;

namespace MultiHitechERP.API.Models.Scheduling
{
    public class ShiftMaster
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal RegularHours { get; set; } = 8.0m;
        public decimal MaxOvertimeHours { get; set; } = 3.0m;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
