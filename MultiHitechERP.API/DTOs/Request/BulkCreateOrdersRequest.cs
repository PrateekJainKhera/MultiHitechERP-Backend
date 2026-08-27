using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>Bulk order creation — one CreateOrderRequest per order, each tagged with the
    /// spreadsheet's Order Ref so results can be reported row-by-row.</summary>
    public class BulkCreateOrdersRequest
    {
        public List<BulkCreateOrderItem> Orders { get; set; } = new();
    }

    public class BulkCreateOrderItem
    {
        /// <summary>Order Ref from the uploaded sheet (for reporting only).</summary>
        public string? Ref { get; set; }
        public CreateOrderRequest Order { get; set; } = new();
    }

    public class BulkOrderResult
    {
        public string? Ref { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
