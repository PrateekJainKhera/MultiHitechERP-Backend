using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class MaterialPiecesByLengthResponse
    {
        public int MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public int MinUsableLengthMM { get; set; }   // pieces reduced below this become scrap
        public int TotalPieces { get; set; }
        public decimal TotalLengthMM { get; set; }
        public decimal TotalWeightKG { get; set; }
        public List<LengthGroupResponse> Groups { get; set; } = new();
    }

    public class LengthGroupResponse
    {
        public decimal LengthMM { get; set; }
        public int Count { get; set; }
        public decimal TotalWeightKG { get; set; }
        public List<PieceInfoResponse> Pieces { get; set; } = new();
    }

    public class PieceInfoResponse
    {
        public int Id { get; set; }
        public string? PieceNo { get; set; }
        public decimal LengthMM { get; set; }
        public decimal WeightKG { get; set; }
    }

    public class ReconcileLogResponse
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? PieceNo { get; set; }
        public string? ActionType { get; set; }
        public decimal? LengthBeforeMM { get; set; }
        public decimal? LengthAfterMM { get; set; }
        public decimal? LengthRemovedMM { get; set; }
        public decimal? WeightRemovedKG { get; set; }
        public string? Reason { get; set; }
        public string? Remarks { get; set; }
        public string? PerformedBy { get; set; }
        public string? CreatedAt { get; set; }
    }

    public class ReconcileResultResponse
    {
        public int BarsRemoved { get; set; }
        public int LengthsAdjusted { get; set; }
        public int MovedToScrap { get; set; }
        public decimal TotalLengthRemovedMM { get; set; }
        public decimal NewTotalLengthMM { get; set; }
        public decimal NewTotalWeightKG { get; set; }
    }
}
