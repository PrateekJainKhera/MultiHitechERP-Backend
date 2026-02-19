namespace MultiHitechERP.API.DTOs.Request
{
    // Request to get material groups for selected requisitions
    public class GetMaterialGroupsRequest
    {
        public List<int> RequisitionIds { get; set; } = new();
    }

    // One cut assigned to a bar
    public class CutRequest
    {
        public int RequisitionItemId { get; set; }
        public int? RequisitionId { get; set; }
        public int CutIndex { get; set; }
        public decimal CutLengthMM { get; set; }
        public string? PartName { get; set; }
        public string? JobCardNo { get; set; }
        public string? RequisitionNo { get; set; }
        public int? MaterialId { get; set; }
    }

    // One bar with its assigned cuts
    public class BarAssignmentRequest
    {
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialCode { get; set; }
        public string? Grade { get; set; }
        public decimal? DiameterMM { get; set; }
        public int PieceId { get; set; }
        public string? PieceNo { get; set; }
        public decimal PieceCurrentLengthMM { get; set; }
        public List<CutRequest> Cuts { get; set; } = new();
    }

    // Save a draft (full cutting plan)
    public class SaveDraftRequest
    {
        public List<int> RequisitionIds { get; set; } = new();
        public List<BarAssignmentRequest> BarAssignments { get; set; } = new();
        public string? Notes { get; set; }
    }

    // Issue a saved draft
    public class IssueDraftRequest
    {
        public string IssuedBy { get; set; } = "Admin";
        public string ReceivedBy { get; set; } = "Operator";
    }

    // One cut to include in the suggest request
    public class SuggestCutItem
    {
        public int RequisitionItemId { get; set; }
        public int? RequisitionId { get; set; }
        public int CutIndex { get; set; }
        public decimal CutLengthMM { get; set; }
        public string? PartName { get; set; }
        public string? JobCardNo { get; set; }
        public string? RequisitionNo { get; set; }
        public int? MaterialId { get; set; }
    }

    // Request to generate cutting plan suggestions for selected cuts
    public class SuggestCuttingPlanRequest
    {
        public List<SuggestCutItem> Cuts { get; set; } = new();
        public int? MaterialId { get; set; }
        public string? Grade { get; set; }
        public decimal? DiameterMM { get; set; }
    }

    // Finalize (issue) multiple drafts at once
    public class FinalizeMultipleDraftsRequest
    {
        public List<int> DraftIds { get; set; } = new();
        public string IssuedBy { get; set; } = "Admin";
        public string ReceivedBy { get; set; } = "Operator";
    }
}
