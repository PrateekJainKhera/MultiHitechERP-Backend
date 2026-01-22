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
        public int JobCardId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int OperatorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity started must be at least 1")]
        public int QuantityStarted { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
