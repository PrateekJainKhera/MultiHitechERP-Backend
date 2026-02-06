using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Material Usage History - Tracks cutting and consumption of material pieces
    /// Stored in Stores_MaterialUsageHistory table
    /// </summary>
    public class MaterialUsageHistory
    {
        public int Id { get; set; }

        // Piece Reference
        public int MaterialPieceId { get; set; }
        public string? PieceNo { get; set; }

        // Order & Part Reference
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? ChildPartId { get; set; }
        public string? ChildPartName { get; set; }
        public string? ProductName { get; set; }

        // Job Card Reference
        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }

        // Usage Details (stored in MM)
        public decimal LengthUsedMM { get; set; }
        public decimal? LengthRemainingMM { get; set; }
        public decimal WastageGeneratedMM { get; set; } = 0;

        // Cutting Details
        public DateTime CuttingDate { get; set; }
        public string? CutByOperator { get; set; }
        public int? CutByOperatorId { get; set; }
        public string? MachineUsed { get; set; }
        public int? MachineId { get; set; }

        // Additional Info
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
