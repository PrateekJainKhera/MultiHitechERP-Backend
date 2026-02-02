using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Simplified Process response DTO
    /// </summary>
    public class ProcessResponse
    {
        public int Id { get; set; }
        public string ProcessCode { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DefaultMachine { get; set; }
        public int? StandardSetupTimeMin { get; set; }
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
