using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces;

public interface IIssueWindowDraftRepository
{
    Task<int> SaveDraftAsync(SaveDraftRequest request);
    Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetDraftsAsync();
    Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetFinalizedDraftsAsync();
    Task<IssueWindowDraftDetailResponse?> GetDraftByIdAsync(int id);
    Task<bool> FinalizeDraftAsync(int id);
    Task<bool> MarkIssuedAsync(int draftId, string issuedBy, string receivedBy);
    Task<bool> DeleteDraftAsync(int id);
}
