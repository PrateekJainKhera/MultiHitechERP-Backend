using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for issuing materials to production
    /// </summary>
    public class IssueMaterialRequest
    {
        [Required]
        public Guid RequisitionId { get; set; }

        [Required]
        public Guid JobCardId { get; set; }

        [Required]
        [StringLength(100)]
        public string IssuedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ReceivedBy { get; set; } = string.Empty;
    }
}
