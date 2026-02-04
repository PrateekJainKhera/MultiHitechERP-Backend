using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class DrawingResponse
    {
        public int Id { get; set; }
        public string DrawingNumber { get; set; } = string.Empty;
        public string DrawingName { get; set; } = string.Empty;
        public string DrawingType { get; set; } = string.Empty;
        public string? RevisionNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
