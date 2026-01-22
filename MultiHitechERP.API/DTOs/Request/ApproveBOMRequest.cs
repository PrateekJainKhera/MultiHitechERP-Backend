using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for approving a BOM
    /// </summary>
    public class ApproveBOMRequest
    {
        [Required]
        [StringLength(100)]
        public string ApprovedBy { get; set; } = string.Empty;
    }
}
