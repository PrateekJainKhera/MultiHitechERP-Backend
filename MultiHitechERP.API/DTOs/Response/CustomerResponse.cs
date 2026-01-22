using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty; // 'Direct', 'Agent', 'Dealer'

        // Contact Information
        public string? ContactPerson { get; set; }
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
        public int CreditDays { get; set; }
        public decimal CreditLimit { get; set; }
        public string? PaymentTerms { get; set; }

        // Status
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
