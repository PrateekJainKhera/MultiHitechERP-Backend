using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class ProductDefaultMaterial
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? ChildPartTemplateId { get; set; } // NULL = main assembly material
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;
        public decimal RequiredQuantity { get; set; }
        public string Unit { get; set; } = "mm";
        public decimal WastageMM { get; set; } = 5;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
