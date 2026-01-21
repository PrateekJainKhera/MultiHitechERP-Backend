using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for rejecting QC result
    /// </summary>
    public class RejectQCRequest
    {
        [Required]
        [StringLength(500)]
        public string RejectionReason { get; set; } = string.Empty;

        [StringLength(500)]
        public string? CorrectiveAction { get; set; }
    }
}
