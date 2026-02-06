using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Goods Receipt Note - Header table for material receipts
    /// Stored in Stores_GRN table
    /// </summary>
    public class GRN
    {
        public int Id { get; set; }
        public string GRNNo { get; set; } = string.Empty;
        public DateTime GRNDate { get; set; }

        // Supplier Info
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierBatchNo { get; set; }

        // Purchase Reference
        public string? PONo { get; set; }
        public DateTime? PODate { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }

        // Totals
        public int TotalPieces { get; set; } = 0;
        public decimal? TotalWeight { get; set; }
        public decimal? TotalValue { get; set; }

        // Status: Draft, Received, Verified, Cancelled
        public string Status { get; set; } = "Draft";

        // Quality Check
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
    }
}
