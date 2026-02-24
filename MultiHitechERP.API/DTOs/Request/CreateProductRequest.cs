using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProductRequest
    {
        // PartCode is auto-generated based on RollerType

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Machine model is required")]
        public int ModelId { get; set; }

        [Required]
        public string RollerType { get; set; } = string.Empty;

        public decimal? Diameter { get; set; }

        public decimal? Length { get; set; }

        public string? MaterialGrade { get; set; }
        public string? DrawingNo { get; set; }
        public string? RevisionNo { get; set; }
        public string? RevisionDate { get; set; }

        [Required(ErrorMessage = "Number of teeth is required")]
        public int NumberOfTeeth { get; set; }

        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }

        public int? ProductTemplateId { get; set; }

        [Required]
        public int ProcessTemplateId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        // Drawing Request (optional - if true, immediately request drawing from team)
        public bool RequestDrawing { get; set; } = false;
    }
}
