using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for BOM
    /// </summary>
    public class BOMResponse
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
        public bool IsLatestRevision { get; set; }

        // Type
        public string? BOMType { get; set; }

        // Quantities
        public decimal? BaseQuantity { get; set; }
        public string? BaseUOM { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }

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
