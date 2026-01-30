using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProcessTemplateStepRequest
    {
        [Required]
        public int StepNo { get; set; }

        [Required]
        public int ProcessId { get; set; }

        public string? ProcessName { get; set; }

        public bool IsMandatory { get; set; } = true;

        public bool CanBeParallel { get; set; } = false;
    }
}
