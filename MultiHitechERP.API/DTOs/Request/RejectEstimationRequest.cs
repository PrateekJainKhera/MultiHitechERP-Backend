using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class RejectEstimationRequest
    {
        [Required]
        public string RejectedBy { get; set; } = "Admin";
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
