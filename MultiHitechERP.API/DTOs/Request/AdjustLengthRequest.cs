using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class AdjustLengthRequest
    {
        [Required]
        [Range(1, 99999, ErrorMessage = "New length must be between 1 and 99999 mm")]
        public decimal NewLengthMM { get; set; }

        [Required(ErrorMessage = "Remark is required")]
        [MinLength(3, ErrorMessage = "Remark must be at least 3 characters")]
        public string Remark { get; set; } = string.Empty;

        public string AdjustedBy { get; set; } = "Admin";
    }
}
