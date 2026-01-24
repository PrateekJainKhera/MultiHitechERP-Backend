using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Material requirement (BOM) for a child part template
    /// </summary>
    public class ChildPartTemplateMaterialRequirement
    {
        public int Id { get; set; }
        public int ChildPartTemplateId { get; set; }
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;
        public decimal QuantityRequired { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal WastagePercent { get; set; }
    }
}
