using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ProcessResponse
    {
        public int Id { get; set; }
        public string ProcessCode { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;

        // Process Category
        public int? ProcessCategoryId { get; set; }
        public string? ProcessCategoryName { get; set; }

        // Time fields for scheduling
        public int? StandardSetupTimeMin { get; set; }
        public decimal? CycleTimePerPieceHours { get; set; }
        public decimal? RestTimeHours { get; set; }

        public string? Description { get; set; }
        public bool IsOutsourced { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
