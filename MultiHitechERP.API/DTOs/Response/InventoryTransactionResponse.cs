using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for Inventory Transaction
    /// </summary>
    public class InventoryTransactionResponse
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }

        // Transaction Details
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }

        // Quantity
        public decimal Quantity { get; set; }
        public string UOM { get; set; } = "KG";

        // Reference
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceNo { get; set; }

        // Location
        public string? FromLocation { get; set; }
        public string? ToLocation { get; set; }

        // Cost
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }

        // Balance After Transaction
        public decimal BalanceQuantity { get; set; }

        // Additional Details
        public string? Remarks { get; set; }
        public string? PerformedBy { get; set; }

        // References
        public int? JobCardId { get; set; }
        public int? RequisitionId { get; set; }
        public int? SupplierId { get; set; }
        public string? GRNNo { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
