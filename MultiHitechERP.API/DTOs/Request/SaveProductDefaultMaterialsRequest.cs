using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class SaveProductDefaultMaterialsRequest
    {
        [Required]
        public List<ProductDefaultMaterialItem> Materials { get; set; } = new();
        public string? UpdatedBy { get; set; }
    }

    public class ProductDefaultMaterialItem
    {
        public int? ChildPartTemplateId { get; set; }
        public int? RawMaterialId { get; set; }

        [Required]
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;

        [Range(0.0001, double.MaxValue)]
        public decimal RequiredQuantity { get; set; }
        public string Unit { get; set; } = "mm";

        [Range(0, 1000)]
        public decimal WastageMM { get; set; } = 5;
        public string? Notes { get; set; }
    }
}
