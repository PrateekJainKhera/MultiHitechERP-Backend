using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    public class GRNApprovalRequest
    {
        public string ActionBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class CreateGRNRequest
    {
        public string GRNNo { get; set; } = string.Empty;
        public DateTime GRNDate { get; set; }

        // Supplier
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierBatchNo { get; set; }

        // Purchase Reference
        public string? PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }

        // Lines
        public List<CreateGRNLineRequest> Lines { get; set; } = new();

        // General
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class CreateGRNLineRequest
    {
        public int SequenceNo { get; set; }
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public string? Grade { get; set; }

        // Material Type & Dimensions
        public string MaterialType { get; set; } = "Rod"; // Rod, Pipe, Sheet, Forged
        public decimal? Diameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Thickness { get; set; }

        // Material Properties
        public decimal MaterialDensity { get; set; } = 7.85m; // Default MS/EN8

        // Quantities
        public decimal TotalWeightKG { get; set; }           // Actual weight (from piece dimensions × density)
        public decimal? BilledWeightKG { get; set; }         // Vendor invoice weight for this batch
        public int NumberOfPieces { get; set; }
        public decimal? LengthPerPieceMM { get; set; }       // Actual measured length per piece

        // Storage
        public int? WarehouseId { get; set; }

        // Pricing
        public decimal? UnitPrice { get; set; }
        public string? Remarks { get; set; }
    }
}
