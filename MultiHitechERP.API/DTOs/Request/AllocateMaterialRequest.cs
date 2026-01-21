using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for allocating materials to a requisition using FIFO
    /// </summary>
    public class AllocateMaterialRequest
    {
        [Required]
        public Guid RequisitionId { get; set; }

        [Required]
        public Guid MaterialId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Required quantity must be greater than 0")]
        public decimal RequiredQuantityMM { get; set; }
    }
}
