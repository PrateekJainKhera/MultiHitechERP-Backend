using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IStoresIssueWindowService
    {
        // Get all Approved requisitions ready for cutting planning
        Task<IEnumerable<IssueWindowRequisitionResponse>> GetApprovedRequisitionsAsync();

        // Get material groups (grouped + qty-expanded cut rows) for selected requisitions
        Task<IEnumerable<MaterialGroupResponse>> GetMaterialGroupsAsync(IEnumerable<int> requisitionIds);

        // Get available pieces for a specific material (for bar-select dropdown)
        Task<IEnumerable<IssueWindowAvailablePieceResponse>> GetAvailablePiecesAsync(int materialId, string? grade, decimal? diameterMM);

        // Save a draft cutting plan (reserves pieces immediately)
        Task<IssueWindowDraftDetailResponse> SaveDraftAsync(SaveDraftRequest request);

        // List all Draft-status drafts (Cutting Planning page)
        Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetDraftsAsync();

        // List all Finalized drafts (Issue List page)
        Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetFinalizedDraftsAsync();

        // Get draft detail
        Task<IssueWindowDraftDetailResponse?> GetDraftByIdAsync(int id);

        // Finalize a draft: lock it and move it to Issue List (pieces stay Reserved)
        Task<bool> FinalizeDraftAsync(int id);

        // Issue a finalized draft: cuts bars, marks requisitions Issued, updates stock
        Task<IEnumerable<IssueWindowIssueResultResponse>> IssueDraftAsync(int draftId, IssueDraftRequest request);

        // Generate 3 cutting plan suggestions for the selected cuts
        Task<IEnumerable<CuttingPlanResponse>> SuggestCuttingPlanAsync(SuggestCuttingPlanRequest request);

        // Delete a draft (only if status = Draft — releases reserved pieces)
        Task<bool> DeleteDraftAsync(int id);
    }
}
