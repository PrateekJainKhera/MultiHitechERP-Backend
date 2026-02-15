using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Links child part drawings to products
    /// Each product can have multiple child parts, each with its own drawing
    /// </summary>
    public class ProductChildPartDrawing
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ChildPartTemplateId { get; set; }
        public int DrawingId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
