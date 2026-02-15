using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request DTO for requesting drawing creation from drawing team
    /// Used when product is created and needs drawings to be prepared
    /// </summary>
    public class RequestDrawingRequest
    {
        [Required(ErrorMessage = "Requested by is required")]
        public string RequestedBy { get; set; } = string.Empty;
    }
}
