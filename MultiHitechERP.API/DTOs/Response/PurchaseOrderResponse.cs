namespace MultiHitechERP.API.DTOs.Response
{
    public class PurchaseOrderResponse
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public int? PurchaseRequestId { get; set; }
        public string? PRNumber { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string VendorCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? TotalAmount { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public List<PurchaseOrderItemResponse> Items { get; set; } = new();
    }

    public class PurchaseOrderItemResponse
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int? PurchaseRequestItemId { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemCode { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal OrderedQty { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
    }
}
