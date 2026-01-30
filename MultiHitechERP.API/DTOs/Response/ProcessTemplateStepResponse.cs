namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessTemplateStepResponse
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public int StepNo { get; set; }

        public int ProcessId { get; set; }
        public string? ProcessName { get; set; }

        public bool IsMandatory { get; set; }
        public bool CanBeParallel { get; set; }
    }
}
