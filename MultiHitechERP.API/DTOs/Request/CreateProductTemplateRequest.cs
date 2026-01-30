using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProductTemplateRequest
    {
        // TemplateCode is auto-generated based on RollerType

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string RollerType { get; set; } = string.Empty;

        [Required]
        public int ProcessTemplateId { get; set; }

        public List<CreateProductTemplateChildPartRequest> ChildParts { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }

    public class CreateProductTemplateChildPartRequest
    {
        [Required]
        public string ChildPartName { get; set; } = string.Empty;

        public string? ChildPartCode { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public string Unit { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Required]
        public int SequenceNo { get; set; }

        public int? ChildPartTemplateId { get; set; }
    }
}
