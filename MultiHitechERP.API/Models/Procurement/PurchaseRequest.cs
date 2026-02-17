namespace MultiHitechERP.API.Models.Procurement
{
    public class PurchaseRequest
    {
        public int Id { get; set; }
        public string PRNumber { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;  // Component or RawMaterial
        public string RequestedBy { get; set; } = "Admin";
        public DateTime RequestDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Draft";
        public string? RejectionReason { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseRequestItem> Items { get; set; } = new();
    }
}
