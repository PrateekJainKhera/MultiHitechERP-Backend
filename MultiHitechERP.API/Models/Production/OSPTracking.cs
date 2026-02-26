namespace MultiHitechERP.API.Models.Production
{
    public class OSPTracking
    {
        public int Id { get; set; }

        // Job Card Reference
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = "";

        // Order Reference
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }

        // Part / Process Info
        public string? ChildPartName { get; set; }
        public string? ProcessName { get; set; }

        // Vendor (VendorName joined from Masters_Vendors)
        public int VendorId { get; set; }
        public string? VendorName { get; set; }

        // Quantities
        public int Quantity { get; set; }       // total sent
        public int ReceivedQty { get; set; }    // cumulative received back
        public int RejectedQty { get; set; }    // cumulative rejected by vendor

        // Dates
        public DateTime SentDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        // Status: Sent | Received
        public string Status { get; set; } = "Sent";

        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
