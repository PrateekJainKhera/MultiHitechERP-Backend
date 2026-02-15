using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateProductRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string PartCode { get; set; } = string.Empty;

        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Machine model is required")]
        public int ModelId { get; set; }

        [Required]
        public string RollerType { get; set; } = string.Empty;

        [Required]
        public decimal Diameter { get; set; }

        [Required]
        public decimal Length { get; set; }

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

        // Drawing linkage
        public int? AssemblyDrawingId { get; set; }
        public int? CustomerProvidedDrawingId { get; set; }
    }
}
