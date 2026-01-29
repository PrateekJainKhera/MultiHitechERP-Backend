using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MaterialResponse
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty; // MaterialGrade: EN8, EN19, SS304, SS316, Alloy Steel
        public string Shape { get; set; } = string.Empty; // MaterialShape: Rod, Pipe, Forged
        public decimal Diameter { get; set; }
        public decimal LengthInMM { get; set; }
        public decimal Density { get; set; }
        public decimal WeightKG { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
