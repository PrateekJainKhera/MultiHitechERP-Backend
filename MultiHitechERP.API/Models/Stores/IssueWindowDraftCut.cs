namespace MultiHitechERP.API.Models.Stores;

public class IssueWindowDraftCut
{
    public int Id { get; set; }
    public int BarAssignmentId { get; set; }
    public int DraftId { get; set; }
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
