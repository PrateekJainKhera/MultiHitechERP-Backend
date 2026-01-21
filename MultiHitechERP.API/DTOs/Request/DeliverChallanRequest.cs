using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for marking challan as delivered
    /// </summary>
    public class DeliverChallanRequest
    {
        [Required]
        [StringLength(100)]
        public string ReceivedBy { get; set; } = string.Empty;

        [StringLength(500)]
        public string? DeliveryRemarks { get; set; }
    }
}
