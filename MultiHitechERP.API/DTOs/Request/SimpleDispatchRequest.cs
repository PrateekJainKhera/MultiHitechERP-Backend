using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class SimpleDispatchRequest
    {
        [Required]
        public int OrderItemId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int QtyToDispatch { get; set; }

        [Required]
        public DateTime DispatchDate { get; set; }

        public string? InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? InvoiceDocument { get; set; } // stored filename after upload

        public string? Remarks { get; set; }
    }
}
