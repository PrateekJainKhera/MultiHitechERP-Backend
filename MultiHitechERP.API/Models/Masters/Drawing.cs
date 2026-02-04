using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class Drawing
    {
        public int Id { get; set; }
        public string DrawingNumber { get; set; } = string.Empty;
        public string DrawingName { get; set; } = string.Empty;
        public string DrawingType { get; set; } = string.Empty;
        public string? RevisionNumber { get; set; }
        public string Status { get; set; } = "Draft";
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
