using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Audit record for a change made to a material requisition line's material/spec
    /// before it is issued (planner substitution or stores substitution at issue).
    /// </summary>
    public class RequisitionMaterialChangeLog
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public string? RequisitionNo { get; set; }
        public int ItemId { get; set; }
        public int? LineNo { get; set; }
        public string? JobCardNo { get; set; }
        public string? OrderNo { get; set; }

        public string ChangeType { get; set; } = "MaterialChange"; // MaterialChange | SpecChange

        public int? FromMaterialId { get; set; }
        public string? FromMaterialCode { get; set; }
        public string? FromMaterialName { get; set; }
        public int? ToMaterialId { get; set; }
        public string? ToMaterialCode { get; set; }
        public string? ToMaterialName { get; set; }

        public decimal? FromLengthMM { get; set; }
        public decimal? ToLengthMM { get; set; }
        public int? FromPieces { get; set; }
        public int? ToPieces { get; set; }

        public bool ReApprovalTriggered { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ChangedBy { get; set; }
        public string? ChangedByRole { get; set; } // Planner | Stores
        public DateTime CreatedAt { get; set; }
    }
}
