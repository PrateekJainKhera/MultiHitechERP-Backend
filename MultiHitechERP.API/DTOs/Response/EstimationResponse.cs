namespace MultiHitechERP.API.DTOs.Response
{
    public class EstimationResponse
    {
        public int Id { get; set; }
        public string EstimateNo { get; set; } = string.Empty;
        public string BaseEstimateNo { get; set; } = string.Empty;
        public int RevisionNumber { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public string? DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ValidUntil { get; set; } = string.Empty;
        public string? ApprovedBy { get; set; }
        public string? ApprovedAt { get; set; }
        public string? RejectedBy { get; set; }
        public string? RejectedAt { get; set; }
        public string? RejectionReason { get; set; }
        public int? ConvertedOrderId { get; set; }
        public string? ConvertedAt { get; set; }
        public string? Notes { get; set; }
        public string? TermsAndConditions { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public List<EstimationItemResponse> Items { get; set; } = new();
    }

    public class EstimationItemResponse
    {
        public int Id { get; set; }
        public int EstimationId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PartCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
    }
}
