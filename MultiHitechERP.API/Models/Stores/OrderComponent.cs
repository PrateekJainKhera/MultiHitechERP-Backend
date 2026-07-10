using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// A component reserved / consumed against an order (from Shop Floor stock).
    /// </summary>
    public class OrderComponent
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string? OrderNo { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal ReservedQty { get; set; }
        public decimal ConsumedQty { get; set; }
        public string Status { get; set; } = "Reserved"; // Reserved | Partial | Consumed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
