using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Dispatch several ready order items for ONE customer on a single consolidated challan / bill.
    /// </summary>
    public class ConsolidatedDispatchRequest
    {
        public int CustomerId { get; set; }
        public DateTime DispatchDate { get; set; }

        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }

        public string? DeliveryAddress { get; set; }
        public string? TransportMode { get; set; }
        public string? VehicleNumber { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContact { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }

        public List<ConsolidatedDispatchLine> Items { get; set; } = new();
    }

    public class ConsolidatedDispatchLine
    {
        public int OrderItemId { get; set; }
        public int QtyToDispatch { get; set; }
    }
}
