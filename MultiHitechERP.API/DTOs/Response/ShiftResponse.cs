namespace MultiHitechERP.API.DTOs.Response
{
    public class ShiftResponse
    {
        public int Id { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;   // "08:00"
        public string EndTime { get; set; } = string.Empty;     // "16:00"
        public decimal RegularHours { get; set; }
        public decimal MaxOvertimeHours { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateShiftRequest
    {
        public string ShiftName { get; set; } = string.Empty;
        public string StartTime { get; set; } = "08:00";   // HH:mm
        public string EndTime { get; set; } = "16:00";     // HH:mm
        public decimal RegularHours { get; set; } = 8.0m;
        public decimal MaxOvertimeHours { get; set; } = 3.0m;
        public bool IsActive { get; set; } = true;
    }
}
