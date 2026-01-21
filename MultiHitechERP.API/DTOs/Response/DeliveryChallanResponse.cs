using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for Delivery Challan
    /// </summary>
    public class DeliveryChallanResponse
    {
        public Guid Id { get; set; }
        public string ChallanNo { get; set; } = string.Empty;
        public DateTime ChallanDate { get; set; }

        // Order Reference
        public Guid OrderId { get; set; }
        public string? OrderNo { get; set; }

        // Customer & Product
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid ProductId { get; set; }
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
        public string Status { get; set; } = string.Empty;
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
