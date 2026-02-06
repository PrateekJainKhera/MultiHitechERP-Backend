using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents a physical piece of raw material with length tracking
    /// Stored in Stores_MaterialPieces table
    /// Lengths are stored in MM (millimeters), displayed as Meters in UI
    /// </summary>
    public class MaterialPiece
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string PieceNo { get; set; } = string.Empty;

        // Denormalized Material Info (for quick display)
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }
        public decimal? Diameter { get; set; } // in mm

        // Length & Weight Tracking (CRITICAL - stored in MM and KG)
        public decimal OriginalLengthMM { get; set; }
        public decimal CurrentLengthMM { get; set; }
        public decimal OriginalWeightKG { get; set; }
        public decimal CurrentWeightKG { get; set; }

        // Status: Available, Allocated, Issued, InUse, Consumed, Scrap
        public string Status { get; set; } = "Available";
        public int? AllocatedToRequisitionId { get; set; }
        public int? IssuedToJobCardId { get; set; }

        // Location
        public string? StorageLocation { get; set; }
        public string? BinNumber { get; set; }
        public string? RackNumber { get; set; }

        // GRN Reference
        public int? GRNId { get; set; }
        public string? GRNNo { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string? SupplierBatchNo { get; set; }
        public int? SupplierId { get; set; }
        public decimal? UnitCost { get; set; }

        // Wastage Tracking
        public bool IsWastage { get; set; } = false;
        public string? WastageReason { get; set; }
        public decimal? ScrapValue { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
