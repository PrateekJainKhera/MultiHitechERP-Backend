namespace MultiHitechERP.API.Models.Stores;

public class IssueWindowDraft
{
    public int Id { get; set; }
    public string DraftNo { get; set; } = string.Empty;
    public string RequisitionIds { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string? IssuedBy { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
}
