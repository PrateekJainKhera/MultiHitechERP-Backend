using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductChildPartDrawingResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ChildPartTemplateId { get; set; }
        public string? ChildPartTemplateName { get; set; }
        public int DrawingId { get; set; }

        // Drawing Details (from Masters_Drawings)
        public string? DrawingNumber { get; set; }
        public string? DrawingName { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileUrl { get; set; }
        public decimal? FileSize { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
