namespace MultiHitechERP.API.Models.Stores;

public class IssueWindowDraftBarAssignment
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

    public List<IssueWindowDraftCut> Cuts { get; set; } = new();
}
