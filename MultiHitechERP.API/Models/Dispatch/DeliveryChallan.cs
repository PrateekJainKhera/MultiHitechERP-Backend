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
        public DateTime? InvoiceDate { get; set; }

        // Acknowledgment
        public string? ReceivedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string? DeliveryRemarks { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
