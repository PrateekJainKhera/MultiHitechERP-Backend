using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a Bill of Materials (BOM) for a product
    /// </summary>
    public class BOM
    {
        public Guid Id { get; set; }
        public string BOMNo { get; set; } = string.Empty;

        // Product Reference
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        // Revision
        public string? RevisionNumber { get; set; }
        public DateTime? RevisionDate { get; set; }
        public bool IsLatestRevision { get; set; } = true;

        // Type
        public string? BOMType { get; set; } = "Manufacturing";

        // Quantities
        public decimal? BaseQuantity { get; set; } = 1;
        public string? BaseUOM { get; set; } = "PCS";

        // Status
        public bool IsActive { get; set; } = true;
        public string? Status { get; set; } = "Active";

        // Approval
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
