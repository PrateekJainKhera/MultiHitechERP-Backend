namespace MultiHitechERP.API.DTOs.Response
{
    public class VendorResponse
    {
        public int Id { get; set; }
        public string VendorCode { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string VendorType { get; set; } = string.Empty;
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
        public string PaymentTerms { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
