namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents individual process steps in a process template
    /// </summary>
    public class ProcessTemplateStep
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int StepNo { get; set; }

        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }

        public bool IsMandatory { get; set; } = true;
        public bool CanBeParallel { get; set; } = false;
    }
}
