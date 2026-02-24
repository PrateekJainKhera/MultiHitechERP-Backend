namespace MultiHitechERP.API.DTOs.Response
{
    public class OpeningStockSummaryResponse
    {
        public int Id { get; set; }
        public string EntryNo { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedBy { get; set; }
        public int TotalPieces { get; set; }
        public int TotalComponents { get; set; }
        public int RawMaterialLines { get; set; }
        public int ComponentLines { get; set; }
    }

    public class OpeningStockDetailResponse
    {
        public int Id { get; set; }
        public string EntryNo { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedBy { get; set; }
        public int? TotalPieces { get; set; }
        public int? TotalComponents { get; set; }
        public List<OpeningStockItemResponse> Items { get; set; } = new();
    }

    public class OpeningStockItemResponse
    {
        public int Id { get; set; }
        public int SequenceNo { get; set; }
        public string ItemType { get; set; } = string.Empty;

        // Raw Material
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }
        public string? MaterialType { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? MaterialDensity { get; set; }
        public decimal? TotalWeightKG { get; set; }
        public decimal? CalculatedLengthMM { get; set; }
        public decimal? WeightPerMeterKG { get; set; }
        public int? NumberOfPieces { get; set; }
        public decimal? LengthPerPieceMM { get; set; }
        public int? WarehouseId { get; set; }

        // Component
        public int? ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public decimal? Quantity { get; set; }
        public string? UOM { get; set; }

        // Common
        public decimal? UnitCost { get; set; }
        public decimal? LineTotal { get; set; }
        public string? Remarks { get; set; }
    }
}
