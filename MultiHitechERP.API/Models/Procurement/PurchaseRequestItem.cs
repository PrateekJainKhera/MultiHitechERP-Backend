namespace MultiHitechERP.API.Models.Procurement
{
    public class PurchaseRequestItem
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
        public decimal? EstimatedUnitCost { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public List<PRItemCuttingListEntry> CuttingList { get; set; } = new();
    }
}
