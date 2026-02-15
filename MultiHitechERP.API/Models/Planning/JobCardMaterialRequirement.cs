using System;

namespace MultiHitechERP.API.Models.Planning
{
    /// <summary>
    /// Material requirements for a specific job card
    /// Confirmed by planner during job card creation
    /// </summary>
    public class JobCardMaterialRequirement
    {
        public int Id { get; set; }

        // Job Card Reference
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }

        // Material Reference
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;

        // Quantity Required
        public decimal RequiredQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;

        // Wastage calculation (in millimeters)
        public decimal WastageMM { get; set; } = 5;
        public decimal TotalQuantityWithWastage { get; set; }

        // Source tracking
        public string Source { get; set; } = "Template"; // Template | Manual

        // Confirmation tracking (planner confirms during planning)
        public string ConfirmedBy { get; set; } = string.Empty;
        public DateTime ConfirmedAt { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
