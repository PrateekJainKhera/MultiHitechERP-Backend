namespace MultiHitechERP.API.DTOs.Response
{
    public class PurchaseRequestResponse
    {
        public int Id { get; set; }
        public string PRNumber { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseRequestItemResponse> Items { get; set; } = new();
    }

    public class PurchaseRequestItemResponse
    {
        public int Id { get; set; }
        public int PurchaseRequestId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemCode { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal RequestedQty { get; set; }
        public decimal? ApprovedQty { get; set; }
        public int? VendorId { get; set; }
        public string? VendorName { get; set; }
        public decimal? EstimatedUnitCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<CuttingListItemResponse> CuttingList { get; set; } = new();
    }

    public class CuttingListItemResponse
    {
        public int Id { get; set; }
        public int PRItemId { get; set; }
        public decimal LengthMeter { get; set; }
        public int Pieces { get; set; }
        public decimal TotalLengthMeter { get; set; }
        public string? Notes { get; set; }
    }
}
