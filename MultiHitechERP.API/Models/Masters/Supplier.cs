using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a supplier/vendor for outsourced processes and purchased materials
    /// </summary>
    public class Supplier
    {
        public Guid Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        // Classification
        public string? SupplierType { get; set; } // Raw Material, Outsourcing, Component, Service
        public string? Category { get; set; }

        // Contact Information
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        // Address
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; } = "India";
        public string? PostalCode { get; set; }

        // Tax Information
        public string? GSTNumber { get; set; }
        public string? PANNumber { get; set; }
        public string? TaxStatus { get; set; }

        // Payment Terms
        public string? PaymentTerms { get; set; } // Net 30, Net 45, COD, etc.
        public int? CreditDays { get; set; }
        public decimal? CreditLimit { get; set; }

        // Bank Details
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BankBranch { get; set; }

        // Performance Metrics
        public decimal? OnTimeDeliveryRate { get; set; } // Percentage
        public decimal? QualityRating { get; set; } // 1-5 stars
        public int? TotalOrders { get; set; }
        public int? RejectedOrders { get; set; }

        // Capabilities
        public string? ServicesOffered { get; set; } // Comma-separated list
        public string? ProcessCapabilities { get; set; } // Heat Treatment, Plating, etc.
        public string? MaterialsSupplied { get; set; } // Steel, Aluminum, etc.

        // Lead Times
        public int? StandardLeadTimeDays { get; set; }
        public int? MinimumOrderQuantity { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false;
        public string? Status { get; set; } = "Active";
        public string? ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Notes
        public string? Remarks { get; set; }
        public string? InternalNotes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
