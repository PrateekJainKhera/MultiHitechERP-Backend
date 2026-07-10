using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Piece-level stock reconciliation for a raw material.
    /// Removals = whole bars removed (count correction). LengthChanges = a bar physically shorter (reconcile consumption).
    /// </summary>
    public class ReconcilePiecesRequest
    {
        public int MaterialId { get; set; }
        public string? PerformedBy { get; set; }
        public string? Remarks { get; set; }
        public List<PieceRemovalItem> Removals { get; set; } = new();
        public List<PieceLengthChangeItem> LengthChanges { get; set; } = new();
    }

    public class PieceRemovalItem
    {
        public decimal LengthMM { get; set; }  // remove bars of this length
        public int Count { get; set; }          // how many bars
    }

    public class PieceLengthChangeItem
    {
        public int PieceId { get; set; }
        public decimal NewLengthMM { get; set; } // reduced length (must be < current)
    }
}
