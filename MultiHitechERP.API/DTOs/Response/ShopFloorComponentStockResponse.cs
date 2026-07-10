namespace MultiHitechERP.API.DTOs.Response
{
    public class ShopFloorComponentStockResponse
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReservedQty { get; set; }
        public decimal AvailableQty { get; set; }
    }
}
