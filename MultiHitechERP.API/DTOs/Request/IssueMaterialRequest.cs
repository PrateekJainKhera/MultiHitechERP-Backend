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
        public int RequisitionId { get; set; }

        [Required]
        public int JobCardId { get; set; }

        [Required]
        [StringLength(100)]
        public string IssuedBy { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ReceivedBy { get; set; } = string.Empty;
    }
}
