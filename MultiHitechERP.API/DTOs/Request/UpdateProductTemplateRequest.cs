using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProductTemplateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string RollerType { get; set; } = string.Empty;

        [Required]
        public int ProcessTemplateId { get; set; }

        public List<CreateProductTemplateChildPartRequest> ChildParts { get; set; } = new();

        public bool IsActive { get; set; } = true;
    }
}
