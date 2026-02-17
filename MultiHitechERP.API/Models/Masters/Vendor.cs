namespace MultiHitechERP.API.Models.Masters
{
    public class Vendor
    {
        public int Id { get; set; }
        public string VendorCode { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string VendorType { get; set; } = "Trader";
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; } = "India";
        public string? PinCode { get; set; }
        public string? GSTNo { get; set; }
        public string? PANNo { get; set; }
        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }
        public string PaymentTerms { get; set; } = "Net 30 Days";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
