namespace MultiHitechERP.API.DTOs.Response
{
    public class OrderComponentResponse
    {
        public int OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string? OrderNo { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal ReservedQty { get; set; }
        public decimal ConsumedQty { get; set; }
        public string Status { get; set; } = "Reserved";
    }

    public class ConsumeResultResponse
    {
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public decimal Quantity { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
