using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProcessTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Template name is required")]
        public string TemplateName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "At least one roller type is required")]
        public List<string> ApplicableTypes { get; set; } = new(); // PRINTING, MAGNETIC

        public List<CreateProcessTemplateStepRequest> Steps { get; set; } = new();

        // System fields
        public bool IsActive { get; set; } = true;
    }
}
