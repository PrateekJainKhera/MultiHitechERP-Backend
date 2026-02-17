namespace MultiHitechERP.API.Models.Procurement
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public int? PurchaseRequestId { get; set; }
        public int VendorId { get; set; }
        public string Status { get; set; } = "Draft";
        public decimal? TotalAmount { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public List<PurchaseOrderItem> Items { get; set; } = new();
    }
}
