using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a process routing template for manufacturing
    /// </summary>
    public class ProcessTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Applicable Roller Types (stored as JSON array: ["PRINTING", "MAGNETIC"])
        public List<string>? ApplicableTypes { get; set; }

        // System fields
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
