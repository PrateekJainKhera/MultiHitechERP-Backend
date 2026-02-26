using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateOSPTrackingRequest
    {
        [Required]
        public int JobCardId { get; set; }

        [Required]
        public int VendorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public DateTime ExpectedReturnDate { get; set; }

        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ReceiveOSPRequest
    {
        [Required]
        public DateTime ActualReturnDate { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReceivedQty { get; set; }

        [Range(0, int.MaxValue)]
        public int RejectedQty { get; set; }

        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class BatchCreateOSPRequest
    {
        [Required]
        public List<int> JobCardIds { get; set; } = new();

        [Required]
        public int VendorId { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public DateTime ExpectedReturnDate { get; set; }

        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>Available job card for OSP dropdown selection</summary>
    public class OSPJobCardOption
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = "";
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }
        public string? ChildPartName { get; set; }
        public string? ProcessName { get; set; }
        public int Quantity { get; set; }
    }
}
