using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProcessTemplateWithStepsRequest
    {
        [Required]
        public CreateProcessTemplateRequest Template { get; set; } = new();

        public List<CreateProcessTemplateStepRequest> Steps { get; set; } = new();
    }
}
