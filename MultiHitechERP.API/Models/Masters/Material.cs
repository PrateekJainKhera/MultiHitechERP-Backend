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
        public string MaterialName { get; set; } = string.Empty;

        // Material Properties
        public string Grade { get; set; } = string.Empty; // MaterialGrade: EN8, EN19, SS304, SS316, Alloy Steel
        public string Shape { get; set; } = string.Empty; // MaterialShape: Rod, Pipe, Forged

        // Dimensions
        public decimal Diameter { get; set; }
        public decimal LengthInMM { get; set; }

        // Physical Properties
        public decimal Density { get; set; } // Density in g/cm³
        public decimal WeightKG { get; set; } // Weight in KG

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
