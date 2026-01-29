using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateProductRequest
    {
        // PartCode is auto-generated based on RollerType

        public string? CustomerName { get; set; }

        [Required]
        public string ModelName { get; set; } = string.Empty;

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
        public int? NumberOfTeeth { get; set; }
        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }

        [Required]
        public int ProcessTemplateId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}
