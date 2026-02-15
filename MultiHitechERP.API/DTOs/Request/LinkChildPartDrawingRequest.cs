using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class LinkChildPartDrawingRequest
    {
        [Required]
        public int ChildPartTemplateId { get; set; }

        [Required]
        public int DrawingId { get; set; }

        public string? CreatedBy { get; set; }
    }
}
