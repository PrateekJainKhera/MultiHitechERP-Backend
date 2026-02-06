using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// GRN Line Item - Material details in a GRN
    /// Stored in Stores_GRNLines table
    /// </summary>
    public class GRNLine
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public int SequenceNo { get; set; }

        // Material Reference
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }

        // Material Type & Dimensions
        public string MaterialType { get; set; } = string.Empty; // Rod, Pipe, Sheet, Forged
        public decimal? Diameter { get; set; } // For Rod (solid)
        public decimal? OuterDiameter { get; set; } // For Pipe
        public decimal? InnerDiameter { get; set; } // For Pipe
        public decimal? Width { get; set; } // For Sheet
        public decimal? Thickness { get; set; } // For Sheet

        // Material Properties
        public decimal? MaterialDensity { get; set; } // g/cm³ (7.85 for MS/EN8, 7.9 for SS)

        // Quantities
        public decimal TotalWeightKG { get; set; }
        public decimal? CalculatedLengthMM { get; set; }
        public decimal? WeightPerMeterKG { get; set; }

        // Piece Breakdown
        public int NumberOfPieces { get; set; }
        public decimal? LengthPerPieceMM { get; set; }

        // Pricing
        public decimal? UnitPrice { get; set; } // Price per KG
        public decimal? LineTotal { get; set; }

        // Remarks
        public string? Remarks { get; set; }
    }
}
