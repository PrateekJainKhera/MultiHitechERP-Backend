using System;

namespace MultiHitechERP.API.Models.Dispatch
{
    /// <summary>
    /// Represents a delivery challan for dispatching finished goods
    /// </summary>
    public class DeliveryChallan
    {
        public int Id { get; set; }
        public string ChallanNo { get; set; } = string.Empty;
        public DateTime ChallanDate { get; set; }

        // Order Reference
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Customer & Product
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        // Quantity
        public int QuantityDispatched { get; set; }

        // Delivery Details
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? TransportMode { get; set; }
        public string? VehicleNumber { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContact { get; set; }

        // Packaging
        public int? NumberOfPackages { get; set; }
        public string? PackagingType { get; set; }
        public decimal? TotalWeight { get; set; }

        // Status
        public string Status { get; set; } = "Pending";
        public DateTime? DispatchedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Invoice Reference
        public string? InvoiceNo { get; set; }
        public string? InvoiceDocument { get; set; }
        public DateTime? InvoiceDate { get; set; }

        // Acknowledgment
        public string? ReceivedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string? DeliveryRemarks { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }

        // True when this challan consolidates multiple orders/items onto one bill.
        // The per-line breakdown lives in Dispatch_DeliveryChallanItems.
        public bool IsConsolidated { get; set; }
    }

    /// <summary>
    /// A single line of a (usually consolidated) delivery challan — one dispatched order item.
    /// </summary>
    public class DeliveryChallanItem
    {
        public int Id { get; set; }
        public int ChallanId { get; set; }
        public int? OrderId { get; set; }
        public int? OrderItemId { get; set; }
        public string? OrderNo { get; set; }
        public string? ItemSequence { get; set; }
        public int? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public string? UOM { get; set; }
        public string? MachineModel { get; set; }
        public string? RollerType { get; set; }
        public int? NumberOfTeeth { get; set; }
        public string? Remarks { get; set; }
    }
}
