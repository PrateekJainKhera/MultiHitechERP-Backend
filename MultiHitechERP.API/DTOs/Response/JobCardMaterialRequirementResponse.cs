using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Material requirement response for a job card
    /// </summary>
    public class JobCardMaterialRequirementResponse
    {
        public int Id { get; set; }
        public int JobCardId { get; set; }
        public string? JobCardNo { get; set; }

        // Material Reference
        public int? RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
        public string MaterialGrade { get; set; } = string.Empty;

        // Quantity Required
        public decimal RequiredQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal WastageMM { get; set; }
        public decimal TotalQuantityWithWastage { get; set; }

        // Source tracking
        public string Source { get; set; } = string.Empty;

        // Confirmation
        public string ConfirmedBy { get; set; } = string.Empty;
        public DateTime ConfirmedAt { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
