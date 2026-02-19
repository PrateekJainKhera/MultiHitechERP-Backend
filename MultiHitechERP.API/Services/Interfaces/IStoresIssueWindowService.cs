using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IStoresIssueWindowService
    {
        // Get all Approved requisitions ready for issuing
        Task<IEnumerable<IssueWindowRequisitionResponse>> GetApprovedRequisitionsAsync();

        // Get material groups (grouped + qty-expanded cut rows) for selected requisitions
        Task<IEnumerable<MaterialGroupResponse>> GetMaterialGroupsAsync(IEnumerable<int> requisitionIds);

        // Get available pieces for a specific material (for bar-select dropdown)
        Task<IEnumerable<IssueWindowAvailablePieceResponse>> GetAvailablePiecesAsync(int materialId, string? grade, decimal? diameterMM);

        // Save a draft cutting plan
        Task<IssueWindowDraftDetailResponse> SaveDraftAsync(SaveDraftRequest request);

        // List all drafts
        Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetDraftsAsync();

        // Get draft detail
        Task<IssueWindowDraftDetailResponse?> GetDraftByIdAsync(int id);

        // Issue a saved draft: cuts bars, marks requisitions Issued, updates stock
        Task<IEnumerable<IssueWindowIssueResultResponse>> IssueDraftAsync(int draftId, IssueDraftRequest request);

        // Generate 3 cutting plan suggestions for the selected cuts
        Task<IEnumerable<CuttingPlanResponse>> SuggestCuttingPlanAsync(SuggestCuttingPlanRequest request);

        // Finalize (issue) multiple drafts at once
        Task<IEnumerable<IssueWindowIssueResultResponse>> FinalizeMultipleDraftsAsync(FinalizeMultipleDraftsRequest request);

        // Delete a draft (only if status = Draft)
        Task<bool> DeleteDraftAsync(int id);
    }
}
