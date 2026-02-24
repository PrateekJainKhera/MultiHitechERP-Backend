using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MachineResponse
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string? MachineType { get; set; }
        public string? Location { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }

        // Capacity & Scheduling
        public decimal DailyCapacityHours { get; set; }
        public decimal? MaxLengthMM { get; set; }

        // Process Categories
        public List<int> ProcessCategoryIds { get; set; } = new List<int>();
        public List<string> ProcessCategoryNames { get; set; } = new List<string>();

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
