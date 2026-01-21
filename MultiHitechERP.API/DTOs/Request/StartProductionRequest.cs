using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for starting production on a job card
    /// </summary>
    public class StartProductionRequest
    {
        [Required]
        public Guid JobCardId { get; set; }

        [Required]
        public Guid MachineId { get; set; }

        [Required]
        public Guid OperatorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity started must be at least 1")]
        public int QuantityStarted { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
