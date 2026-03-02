using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateJobCardQuantityRequest
    {
        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int NewQuantity { get; set; }

        public string UpdatedBy { get; set; } = "Admin";
    }
}
