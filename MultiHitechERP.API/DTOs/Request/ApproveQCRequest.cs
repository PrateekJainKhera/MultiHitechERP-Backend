using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for approving QC result
    /// </summary>
    public class ApproveQCRequest
    {
        [Required]
        [StringLength(100)]
        public string ApprovedBy { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
