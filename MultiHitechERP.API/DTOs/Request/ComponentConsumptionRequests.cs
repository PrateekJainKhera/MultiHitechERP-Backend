using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    public class ReserveComponentsRequest
    {
        public int OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string? OrderNo { get; set; }
        public string? ReservedBy { get; set; }
        public List<ComponentQtyItem> Components { get; set; } = new();
    }

    public class ComponentQtyItem
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
    }

    public class ConsumeComponentsRequest
    {
        public string? ConsumedBy { get; set; }
        public List<ConsumeComponentItem> Items { get; set; } = new();
    }

    public class ConsumeComponentItem
    {
        public int OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string? OrderNo { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
    }
}
