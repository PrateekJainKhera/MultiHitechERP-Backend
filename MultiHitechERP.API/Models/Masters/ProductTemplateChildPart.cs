using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Child part requirement in a product template
    /// </summary>
    public class ProductTemplateChildPart
    {
        public int Id { get; set; }
        public int ProductTemplateId { get; set; }
        public string ChildPartName { get; set; } = string.Empty;
        public string? ChildPartCode { get; set; }
        public string? ChildPartType { get; set; } // Type of child part (SHAFT, BEARING, etc.)
        public decimal Quantity { get; set; } // Quantity required per roller
        public string Unit { get; set; } = string.Empty; // e.g., "pcs", "kg", "m"
        public string? Notes { get; set; }
        public int SequenceNo { get; set; } // Order in which parts are needed
        public int? ChildPartTemplateId { get; set; } // Optional link to Child Part Template
    }
}
