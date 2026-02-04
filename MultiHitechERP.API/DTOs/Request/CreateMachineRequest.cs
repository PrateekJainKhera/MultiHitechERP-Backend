using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateMachineRequest
    {
        [Required(ErrorMessage = "Machine name is required")]
        public string MachineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Machine type is required")]
        public string MachineType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        public string? Department { get; set; }
        public string? Status { get; set; } = "Idle";
        public string? Notes { get; set; }
    }
}
