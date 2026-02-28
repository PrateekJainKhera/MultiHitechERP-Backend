using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class GRNResponse
    {
        public int Id { get; set; }
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

        // Totals
        public int TotalPieces { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalValue { get; set; }

        // Status
        public string Status { get; set; } = string.Empty;

        // Approval
        public bool RequiresApproval { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionNotes { get; set; }

        // Quality
        public string? QualityCheckStatus { get; set; }
        public string? QualityCheckedBy { get; set; }
        public DateTime? QualityCheckedAt { get; set; }
        public string? QualityRemarks { get; set; }

        // General
        public string? Remarks { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Lines (optional - for detail view)
        public List<GRNLineResponse>? Lines { get; set; }
    }

    public class GRNLineResponse
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public int SequenceNo { get; set; }

        // Material
        public int MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }

        // Material Type & Dimensions
        public string MaterialType { get; set; } = string.Empty;
        public decimal? Diameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Thickness { get; set; }

        // Material Properties
        public decimal? MaterialDensity { get; set; }

        // Quantities
        public decimal TotalWeightKG { get; set; }
        public decimal? CalculatedLengthMM { get; set; }
        public decimal? WeightPerMeterKG { get; set; }

        // Piece Breakdown
        public int NumberOfPieces { get; set; }
        public decimal? LengthPerPieceMM { get; set; }       // Actual measured
        public decimal? BilledLengthPerPieceMM { get; set; } // Vendor billed
        public decimal? BilledWeightKG { get; set; }
        public decimal? LengthVariancePct { get; set; }

        // Pricing
        public decimal? UnitPrice { get; set; }
        public decimal? LineTotal { get; set; }

        // Remarks
        public string? Remarks { get; set; }
    }
}
