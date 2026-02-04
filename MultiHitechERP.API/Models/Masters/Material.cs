using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a raw material master record (not inventory)
    /// This is master data defining what raw materials can be used
    /// </summary>
    public class Material
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; } = string.Empty; // Auto-generated
        public string MaterialName { get; set; } = string.Empty;

        // Material Properties
        public string MaterialType { get; set; } = string.Empty; // MaterialType: Steel, Stainless Steel, Aluminum, Other
        public string Grade { get; set; } = string.Empty; // MaterialGrade: EN8, EN19, SS304, SS316, Alloy Steel
        public string Shape { get; set; } = string.Empty; // MaterialShape: Rod, Pipe, Forged, Sheet

        // Dimensions (shape-dependent)
        public decimal Diameter { get; set; }           // Rod, Pipe (outer), Forged
        public decimal? InnerDiameter { get; set; }     // Pipe only
        public decimal? Width { get; set; }             // Sheet only
        public decimal LengthInMM { get; set; }

        // Physical Properties
        public decimal Density { get; set; } // Density in g/cm³
        public decimal WeightKG { get; set; } // Weight in KG

        // Status
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
