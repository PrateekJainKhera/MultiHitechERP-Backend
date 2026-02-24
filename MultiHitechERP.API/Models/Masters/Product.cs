using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Represents a product master record (rollers for flexo printing)
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string PartCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }

        // Machine Model
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty; // Kept for backward compatibility

        public string RollerType { get; set; } = string.Empty;

        // Dimensions
        public decimal? Diameter { get; set; }
        public decimal? Length { get; set; }

        // Material & Finish
        public string? MaterialGrade { get; set; }
        public string? SurfaceFinish { get; set; }
        public string? Hardness { get; set; }

        // Drawing Reference (Legacy fields - kept for backward compatibility)
        public string? DrawingNo { get; set; }
        public string? RevisionNo { get; set; }
        public string? RevisionDate { get; set; }

        // Drawing Linkage (Product-Level)
        public int? AssemblyDrawingId { get; set; } // FK to Masters_Drawings - Main assembly drawing
        public int? CustomerProvidedDrawingId { get; set; } // FK to Masters_Drawings - Optional customer reference drawing

        // Drawing Review Status (PRODUCT-LEVEL GATE)
        public string DrawingReviewStatus { get; set; } = "Pending"; // Pending, UnderReview, Approved, Rejected, RevisionRequired
        public string? DrawingReviewedBy { get; set; }
        public DateTime? DrawingReviewedAt { get; set; }
        public string? DrawingReviewNotes { get; set; }

        // Drawing Request Tracking (NEW WORKFLOW)
        public DateTime? DrawingRequestedAt { get; set; } // When drawing was requested from team
        public string? DrawingRequestedBy { get; set; } // Who requested the drawing

        // Additional Properties
        public int NumberOfTeeth { get; set; }

        // Template References
        public int? ProductTemplateId { get; set; } // Which product template was used
        public int ProcessTemplateId { get; set; } // Inherited from product template

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
