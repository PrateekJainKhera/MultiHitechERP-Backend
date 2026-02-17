using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class ApproveEstimationRequest
    {
        [Required]
        public string ApprovedBy { get; set; } = "Admin";
    }
}
