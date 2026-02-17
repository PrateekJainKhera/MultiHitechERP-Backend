using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class ApprovePurchaseRequestRequest
    {
        public string ApprovedBy { get; set; } = "Admin";
        public List<ApproveItemRequest> Items { get; set; } = new();
    }

    public class ApproveItemRequest
    {
        [Required]
        public int ItemId { get; set; }

        public string Status { get; set; } = "Approved";  // Approved or Rejected

        [Range(0, double.MaxValue)]
        public decimal? ApprovedQty { get; set; }

        public int? VendorId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? EstimatedUnitCost { get; set; }

        public string? Notes { get; set; }
    }
}
