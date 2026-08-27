using System;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Admin-only edit of a dispatched challan's shipping/invoice details after dispatch.
    /// </summary>
    public class EditDispatchRequest
    {
        public int ChallanId { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DispatchDate { get; set; }   // maps to ChallanDate
        public string? VehicleNumber { get; set; }
        public string? TransportMode { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContact { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Remarks { get; set; }

        // Client-asserted admin gate (app is client-gated, mirrors CreatedBy usage elsewhere).
        public bool IsAdmin { get; set; }
        public string? PerformedBy { get; set; }
    }
}
