using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class DrawingResponse
    {
        public Guid Id { get; set; }
        public string DrawingNumber { get; set; } = string.Empty;
        public string DrawingTitle { get; set; } = string.Empty;

        // Product Reference
        public Guid? ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }

        // Revision Control
        public string? RevisionNumber { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string? RevisionDescription { get; set; }

        // Classification
        public string? DrawingType { get; set; }
        public string? Category { get; set; }

        // File Information
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileFormat { get; set; }
        public long? FileSize { get; set; }

        // Approval
        public string? PreparedBy { get; set; }
        public string? CheckedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        // Manufacturing Info
        public string? MaterialSpecification { get; set; }
        public string? Finish { get; set; }
        public string? ToleranceGrade { get; set; }
        public string? TreatmentRequired { get; set; }

        // Dimensions Summary
        public decimal? OverallLength { get; set; }
        public decimal? OverallWidth { get; set; }
        public decimal? OverallHeight { get; set; }
        public decimal? Weight { get; set; }

        // Status
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public bool IsLatestRevision { get; set; }

        // Version History
        public Guid? PreviousRevisionId { get; set; }
        public int VersionNumber { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
