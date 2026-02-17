using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreatePurchaseRequestRequest
    {
        [Required]
        public string ItemType { get; set; } = string.Empty;  // Component or RawMaterial

        public string? Notes { get; set; }
        public string RequestedBy { get; set; } = "Admin";

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreatePurchaseRequestItemRequest> Items { get; set; } = new();
    }

    public class CreatePurchaseRequestItemRequest
    {
        [Required]
        public string ItemType { get; set; } = string.Empty;
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string ItemName { get; set; } = string.Empty;
        public string? ItemCode { get; set; }
        [Required]
        public string Unit { get; set; } = string.Empty;
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal RequestedQty { get; set; }
        public string? Notes { get; set; }
        public List<CreateCuttingListItemRequest> CuttingList { get; set; } = new();
    }

    public class CreateCuttingListItemRequest
    {
        [Range(0.001, double.MaxValue, ErrorMessage = "Length must be greater than 0")]
        public decimal LengthMeter { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pieces must be at least 1")]
        public int Pieces { get; set; } = 1;

        public string? Notes { get; set; }
    }
}
