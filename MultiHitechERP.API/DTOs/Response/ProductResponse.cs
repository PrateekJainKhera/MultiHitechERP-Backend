using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string RollerType { get; set; } = string.Empty;
        public decimal Diameter { get; set; }
        public decimal Length { get; set; }
        public string? MaterialGrade { get; set; }

        // Legacy drawing fields (kept for backward compatibility)
        public string? DrawingNo { get; set; }
        public string? RevisionNo { get; set; }
        public string? RevisionDate { get; set; }

        // Product-Level Drawing Review (NEW)
        public int? AssemblyDrawingId { get; set; }
        public int? CustomerProvidedDrawingId { get; set; }
        public string DrawingReviewStatus { get; set; } = "Pending";
        public string? DrawingReviewedBy { get; set; }
        public DateTime? DrawingReviewedAt { get; set; }
        public string? DrawingReviewNotes { get; set; }
        public DateTime? DrawingRequestedAt { get; set; }
        public string? DrawingRequestedBy { get; set; }

        public int? NumberOfTeeth { get; set; }
        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }
        public int? ProductTemplateId { get; set; }
        public int ProcessTemplateId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
