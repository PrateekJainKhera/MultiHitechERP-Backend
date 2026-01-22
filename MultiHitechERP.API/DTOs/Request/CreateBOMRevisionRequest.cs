using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for creating a BOM revision
    /// </summary>
    public class CreateBOMRevisionRequest
    {
        [Required]
        public Guid BOMId { get; set; }

        [Required]
        [StringLength(20)]
        public string RevisionNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
