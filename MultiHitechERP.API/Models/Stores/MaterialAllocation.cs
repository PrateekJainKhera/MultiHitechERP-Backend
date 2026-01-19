using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents material allocation linking specific material pieces to requisitions
    /// </summary>
    public class MaterialAllocation
    {
        public Guid Id { get; set; }
        public Guid RequisitionId { get; set; }
        public Guid RequisitionItemId { get; set; }

        // Material Piece
        public Guid MaterialPieceId { get; set; }
        public string? PieceNo { get; set; }

        // Allocated Quantity
        public decimal AllocatedQuantity { get; set; }
        public decimal? AllocatedLengthMM { get; set; }
        public decimal? AllocatedWeightKG { get; set; }

        // Status
        public string Status { get; set; } = "Allocated";
        public DateTime AllocatedAt { get; set; }
        public string? AllocatedBy { get; set; }

        // Issue Status
        public bool IsIssued { get; set; }
        public DateTime? IssuedAt { get; set; }

        // Consumption Tracking
        public decimal? ActualConsumedQuantity { get; set; }
        public decimal? ActualConsumedLengthMM { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public decimal? RemainingLengthMM { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
