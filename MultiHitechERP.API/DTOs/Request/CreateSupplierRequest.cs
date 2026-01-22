using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateSupplierRequest
    {
        [Required] [StringLength(50)] public string SupplierCode { get; set; } = string.Empty;
        [Required] [StringLength(200)] public string SupplierName { get; set; } = string.Empty;
        [StringLength(50)] public string? SupplierType { get; set; }
        [StringLength(100)] public string? Category { get; set; }
        [StringLength(100)] public string? ContactPerson { get; set; }
        [StringLength(20)] public string? ContactNumber { get; set; }
        [EmailAddress] [StringLength(100)] public string? Email { get; set; }
        [StringLength(200)] public string? AddressLine1 { get; set; }
        [StringLength(200)] public string? AddressLine2 { get; set; }
        [StringLength(100)] public string? City { get; set; }
        [StringLength(50)] public string? State { get; set; }
        [StringLength(50)] public string? Country { get; set; }
        [StringLength(20)] public string? PostalCode { get; set; }
        [StringLength(20)] public string? GSTNumber { get; set; }
        [StringLength(50)] public string? PaymentTerms { get; set; }
        public int? CreditDays { get; set; }
        public string? ProcessCapabilities { get; set; }
        public int? StandardLeadTimeDays { get; set; }
        [StringLength(500)] public string? Remarks { get; set; }
        [StringLength(100)] public string? CreatedBy { get; set; }
    }
}
