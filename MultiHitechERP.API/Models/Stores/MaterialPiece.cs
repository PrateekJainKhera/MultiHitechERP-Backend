using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents a physical piece of raw material with length tracking
    /// </summary>
    public class MaterialPiece
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string PieceNo { get; set; } = string.Empty;

        // Length Tracking (CRITICAL for steel rods/pipes)
        public decimal OriginalLengthMM { get; set; }
        public decimal CurrentLengthMM { get; set; }
        public decimal OriginalWeightKG { get; set; }
        public decimal CurrentWeightKG { get; set; }

        // Status
        public string Status { get; set; } = "Available";
        public int? AllocatedToRequisitionId { get; set; }
        public int? IssuedToJobCardId { get; set; }

        // Location
        public string? StorageLocation { get; set; }
        public string? BinNumber { get; set; }
        public string? RackNumber { get; set; }

        // GRN
        public string? GRNNo { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? SupplierBatchNo { get; set; }
        public int? SupplierId { get; set; }
        public decimal? UnitCost { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
