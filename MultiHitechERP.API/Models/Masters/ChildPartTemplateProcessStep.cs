using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Manufacturing process step for a child part template
    /// </summary>
    public class ChildPartTemplateProcessStep
    {
        public int Id { get; set; }
        public int ChildPartTemplateId { get; set; }
        public int? ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public int StepNumber { get; set; }
        public string? MachineName { get; set; }
        public decimal StandardTimeHours { get; set; }
        public decimal? RestTimeHours { get; set; }
        public string? Description { get; set; }
    }
}
