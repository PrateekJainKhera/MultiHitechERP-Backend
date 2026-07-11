using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Request to change a requisition line's material (and optionally its length / diameter /
    /// piece count) before it is issued. A material change resets the requisition to Pending
    /// (re-approval). A reason is compulsory for audit.
    /// </summary>
    public class ChangeRequisitionItemMaterialRequest
    {
        [Required]
        public int MaterialId { get; set; }

        // Optional spec overrides. When null, the existing value on the line is kept.
        public decimal? LengthRequiredMM { get; set; }
        public int? NumberOfPieces { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        public string Reason { get; set; } = string.Empty;

        public string? ChangedBy { get; set; }

        /// <summary>"Planner" (requisition page) or "Stores" (issue window).</summary>
        public string? ChangedByRole { get; set; }
    }
}
