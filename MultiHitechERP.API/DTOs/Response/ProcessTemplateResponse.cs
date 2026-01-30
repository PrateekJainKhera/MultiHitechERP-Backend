using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessTemplateResponse
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public List<string> ApplicableTypes { get; set; } = new(); // PRINTING, MAGNETIC

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
