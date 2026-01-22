using System;
using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class UpdateCustomerRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Customer code is required")]
        public string CustomerCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer name is required")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer type is required")]
        public string CustomerType { get; set; } = string.Empty; // 'Direct', 'Agent', 'Dealer'

        // Contact Information
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        // Address
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PinCode { get; set; }

        // Business Details
        public string? GSTNo { get; set; }
        public string? PANNo { get; set; }

        // Payment Terms
        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? PaymentTerms { get; set; }

        // Status
        public bool IsActive { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
