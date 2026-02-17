namespace MultiHitechERP.API.DTOs.Response
{
    public class LowStockAlertResponse
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string Rack { get; set; } = string.Empty;
        public string RackNo { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;

        // Current available stock
        public int CurrentPieces { get; set; }
        public decimal CurrentLengthMM { get; set; }

        // Thresholds
        public int MinStockPieces { get; set; }
        public decimal MinStockLengthMM { get; set; }

        // Alert flags (set in service layer)
        public bool PiecesAlert { get; set; }
        public bool LengthAlert { get; set; }
        public bool IsAlert => PiecesAlert || LengthAlert;
    }
}
