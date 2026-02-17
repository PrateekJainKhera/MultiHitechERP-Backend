using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateVendorRequest
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string VendorName { get; set; } = string.Empty;

        [Required]
        public string VendorType { get; set; } = "Trader";

        public string? ContactPerson { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string Country { get; set; } = "India";
        public string? PinCode { get; set; }
        public string? GSTNo { get; set; }
        public string? PANNo { get; set; }

        [Range(0, int.MaxValue)]
        public int? CreditDays { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }

        public string PaymentTerms { get; set; } = "Net 30 Days";
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }
}
