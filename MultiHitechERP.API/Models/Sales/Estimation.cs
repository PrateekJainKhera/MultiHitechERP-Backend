namespace MultiHitechERP.API.Models.Sales
{
    public class Estimation
    {
        public int Id { get; set; }
        public string EstimateNo { get; set; } = string.Empty;
        public string BaseEstimateNo { get; set; } = string.Empty;
        public int RevisionNumber { get; set; } = 1;
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string Status { get; set; } = "Draft";
        public decimal SubTotal { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ValidUntil { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
        public int? ConvertedOrderId { get; set; }
        public DateTime? ConvertedAt { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public List<EstimationItem> Items { get; set; } = new();
    }
}
