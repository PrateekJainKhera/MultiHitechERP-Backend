using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateMachineModelRequest
    {
        [Required(ErrorMessage = "Model name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Model name must be between 2 and 200 characters")]
        public string ModelName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = "System";
    }
}
