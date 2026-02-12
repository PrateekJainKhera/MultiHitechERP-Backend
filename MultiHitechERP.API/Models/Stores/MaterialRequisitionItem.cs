using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents individual line items in a material requisition
    /// </summary>
    public class MaterialRequisitionItem
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int LineNo { get; set; }

        // Material Details (raw material — set when item is for a raw material rod/bar)
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialGrade { get; set; }

        // Component Details (purchased component — mutually exclusive with MaterialId)
        public int? ComponentId { get; set; }
        public string? ComponentCode { get; set; }
        public string? ComponentName { get; set; }

        // Quantity Required
        public decimal QuantityRequired { get; set; }
        public string? UOM { get; set; } = "KG";

        // Length-Based (for rods/pipes)
        public decimal? LengthRequiredMM { get; set; }
        public decimal? DiameterMM { get; set; }
        public int? NumberOfPieces { get; set; }

        // Allocation Status
        public decimal? QuantityAllocated { get; set; }
        public decimal? QuantityIssued { get; set; }
        public decimal? QuantityPending { get; set; }

        // Status
        public string Status { get; set; } = "Pending";
        public DateTime? AllocatedAt { get; set; }
        public DateTime? IssuedAt { get; set; }

        // Reference
        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public int? ProcessId { get; set; }
        public string? ProcessName { get; set; }

        // Pre-selected pieces (stored as comma-separated IDs)
        /// <summary>
        /// Comma-separated list of pre-selected material piece IDs.
        /// If provided, these specific pieces should be allocated instead of auto-allocation.
        /// </summary>
        public string? SelectedPieceIds { get; set; }

        /// <summary>
        /// Comma-separated list of cut quantities (in MM) for each selected piece.
        /// Order matches SelectedPieceIds. E.g., "300,500,200" means cut 300mm from piece 1, 500mm from piece 2, etc.
        /// </summary>
        public string? SelectedPieceQuantities { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
