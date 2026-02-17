namespace MultiHitechERP.API.DTOs.Response
{
    public class ComponentWithStockResponse
    {
        public int    Id              { get; set; }
        public string PartNumber      { get; set; } = string.Empty;
        public string ComponentName   { get; set; } = string.Empty;
        public string Category        { get; set; } = string.Empty;
        public string Unit            { get; set; } = "Pcs";

        // Stock fields from Inventory_Stock
        public decimal AvailableStock { get; set; }
        public decimal CurrentStock   { get; set; }
        public string  StockLocation  { get; set; } = string.Empty;
    }
}
