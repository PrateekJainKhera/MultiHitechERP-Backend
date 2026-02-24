namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateOpeningStockRequest
    {
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public List<OpeningStockItemRequest> Items { get; set; } = new();
    }

    public class OpeningStockItemRequest
    {
        public int SequenceNo { get; set; }
        public string ItemType { get; set; } = "RawMaterial";  // RawMaterial / Component

        // ── Raw Material (same fields as CreateGRNLineRequest) ──
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? Grade { get; set; }
        public string? MaterialType { get; set; }   // Rod / Pipe / Sheet / Forged
        public decimal? Diameter { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? InnerDiameter { get; set; }
        public decimal? Width { get; set; }
        public decimal? Thickness { get; set; }
        public decimal MaterialDensity { get; set; } = 7.85m;
        public decimal? TotalWeightKG { get; set; }
        public int? NumberOfPieces { get; set; }
        public decimal? LengthPerPieceMM { get; set; }
        public int? WarehouseId { get; set; }

        // ── Component ──
        public int? ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public decimal? Quantity { get; set; }
        public string? UOM { get; set; }

        // ── Common ──
        public decimal? UnitCost { get; set; }
        public string? Remarks { get; set; }
    }

    public class ConfirmOpeningStockRequest
    {
        public string? ConfirmedBy { get; set; }
    }
}
