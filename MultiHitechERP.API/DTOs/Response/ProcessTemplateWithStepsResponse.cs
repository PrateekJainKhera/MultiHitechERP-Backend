using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessTemplateWithStepsResponse
    {
        public ProcessTemplateResponse Template { get; set; } = new();
        public List<ProcessTemplateStepResponse> Steps { get; set; } = new();
    }
}
