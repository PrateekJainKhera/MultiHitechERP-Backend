namespace MultiHitechERP.API.DTOs.Response
{
    // Approved requisition shown in the left selection panel
    public class IssueWindowRequisitionResponse
    {
        public int Id { get; set; }
        public string RequisitionNo { get; set; } = string.Empty;
        public string? OrderNo { get; set; }
        public string? JobCardNo { get; set; }
        public string? CustomerName { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }

    // Material group - groups all cuts of same material across selected requisitions
    public class MaterialGroupResponse
    {
        public int? MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public string? MaterialCode { get; set; }
        public string? Grade { get; set; }
        public decimal? DiameterMM { get; set; }
        public decimal TotalLengthNeededMM { get; set; }
        public List<MaterialGroupCutItem> Cuts { get; set; } = new();
    }

    // One individual cut row (qty expanded — 1 row per physical cut)
    public class MaterialGroupCutItem
    {
        public int RequisitionItemId { get; set; }
        public int RequisitionId { get; set; }
        public int CutIndex { get; set; }           // 0-based index within qty
        public decimal CutLengthMM { get; set; }
        public string? PartName { get; set; }
        public string? JobCardNo { get; set; }
        public string? RequisitionNo { get; set; }
        public int? MaterialId { get; set; }
    }

    // Available piece for a material (shown in bar-select dropdown)
    public class IssueWindowAvailablePieceResponse
    {
        public int Id { get; set; }
        public string PieceNo { get; set; } = string.Empty;
        public decimal CurrentLengthMM { get; set; }
        public string? StorageLocation { get; set; }
        public string? GRNNo { get; set; }
    }

    // Result after issuing a draft
    public class IssueWindowIssueResultResponse
    {
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? IssueId { get; set; }
    }

    // Draft summary for listing
    public class IssueWindowDraftSummaryResponse
    {
        public int Id { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public int RequisitionCount { get; set; }
        public int TotalBars { get; set; }
        public int TotalCuts { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinalizedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
    }

    // Draft full detail
    public class IssueWindowDraftDetailResponse
    {
        public int Id { get; set; }
        public string DraftNo { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public string RequisitionIds { get; set; } = string.Empty;
        public string? IssuedBy { get; set; }
        public string? ReceivedBy { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FinalizedAt { get; set; }
        public DateTime? IssuedAt { get; set; }
        public List<DraftBarAssignmentResponse> BarAssignments { get; set; } = new();
    }

    public class DraftBarAssignmentResponse
    {
        public int Id { get; set; }
        public int DraftId { get; set; }
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialCode { get; set; }
        public string? Grade { get; set; }
        public decimal? DiameterMM { get; set; }
        public int? PieceId { get; set; }
        public string? PieceNo { get; set; }
        public decimal? PieceCurrentLengthMM { get; set; }
        public decimal? TotalCutMM { get; set; }
        public decimal? RemainingMM { get; set; }
        public bool WillBeScrap { get; set; }
        public int SortOrder { get; set; }
        public List<DraftCutResponse> Cuts { get; set; } = new();
    }

    public class DraftCutResponse
    {
        public int Id { get; set; }
        public int RequisitionItemId { get; set; }
        public int? RequisitionId { get; set; }
        public int CutIndex { get; set; }
        public decimal CutLengthMM { get; set; }
        public string? PartName { get; set; }
        public string? JobCardNo { get; set; }
        public string? RequisitionNo { get; set; }
        public int? MaterialId { get; set; }
        public int SortOrder { get; set; }
    }

    // ── Cutting Plan Suggestion DTOs ──────────────────────────────────────────

    // One full cutting plan option (one of 3 strategies)
    public class CuttingPlanResponse
    {
        public int PlanIndex { get; set; }
        public string PlanLabel { get; set; } = string.Empty;
        public string PlanDescription { get; set; } = string.Empty;
        public int TotalBars { get; set; }
        public decimal TotalScrapMM { get; set; }
        public decimal TotalStockReturnMM { get; set; }
        public decimal TotalBarLengthUsedMM { get; set; }
        public bool IsComplete { get; set; }
        public List<BarCutPlanResponse> Bars { get; set; } = new();
    }

    // One bar and its assigned cuts within a plan
    public class BarCutPlanResponse
    {
        public int PieceId { get; set; }
        public string PieceNo { get; set; } = string.Empty;
        public decimal BarLengthMM { get; set; }
        public decimal TotalCutMM { get; set; }
        public decimal RemainingMM { get; set; }
        public bool WillBeScrap { get; set; }
        public List<PlanCutItemResponse> Cuts { get; set; } = new();
    }

    // One cut item within a bar plan
    public class PlanCutItemResponse
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
}
