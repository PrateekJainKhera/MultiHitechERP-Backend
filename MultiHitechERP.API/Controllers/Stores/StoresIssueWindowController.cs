using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/stores/issue-window")]
    [Produces("application/json")]
    public class StoresIssueWindowController : ControllerBase
    {
        private readonly IStoresIssueWindowService _service;

        public StoresIssueWindowController(IStoresIssueWindowService service)
        {
            _service = service;
        }

        // GET /api/stores/issue-window/approved-requisitions
        [HttpGet("approved-requisitions")]
        public async Task<IActionResult> GetApprovedRequisitions()
        {
            var result = await _service.GetApprovedRequisitionsAsync();
            return Ok(ApiResponse<IEnumerable<IssueWindowRequisitionResponse>>.SuccessResponse(result));
        }

        // POST /api/stores/issue-window/material-groups
        [HttpPost("material-groups")]
        public async Task<IActionResult> GetMaterialGroups([FromBody] GetMaterialGroupsRequest request)
        {
            if (request.RequisitionIds == null || !request.RequisitionIds.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No requisition IDs provided"));

            var result = await _service.GetMaterialGroupsAsync(request.RequisitionIds);
            return Ok(ApiResponse<IEnumerable<MaterialGroupResponse>>.SuccessResponse(result));
        }

        // GET /api/stores/issue-window/available-pieces?materialId=1&grade=EN8&diameterMM=50
        [HttpGet("available-pieces")]
        public async Task<IActionResult> GetAvailablePieces(
            [FromQuery] int materialId,
            [FromQuery] string? grade,
            [FromQuery] decimal? diameterMM)
        {
            var result = await _service.GetAvailablePiecesAsync(materialId, grade, diameterMM);
            return Ok(ApiResponse<IEnumerable<IssueWindowAvailablePieceResponse>>.SuccessResponse(result));
        }

        // POST /api/stores/issue-window/drafts — Save draft (reserves pieces immediately)
        [HttpPost("drafts")]
        public async Task<IActionResult> SaveDraft([FromBody] SaveDraftRequest request)
        {
            if (request.RequisitionIds == null || !request.RequisitionIds.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No requisitions in draft"));
            if (request.BarAssignments == null || !request.BarAssignments.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No bar assignments in draft"));

            var result = await _service.SaveDraftAsync(request);
            var response = ApiResponse<IssueWindowDraftDetailResponse>.SuccessResponse(result);
            response.Message = $"Draft {result.DraftNo} saved — materials reserved";
            return Ok(response);
        }

        // GET /api/stores/issue-window/drafts — Draft-status drafts (Cutting Planning page)
        [HttpGet("drafts")]
        public async Task<IActionResult> GetDrafts()
        {
            var result = await _service.GetDraftsAsync();
            return Ok(ApiResponse<IEnumerable<IssueWindowDraftSummaryResponse>>.SuccessResponse(result));
        }

        // GET /api/stores/issue-window/issue-list — Finalized drafts (Issue List page)
        [HttpGet("issue-list")]
        public async Task<IActionResult> GetIssueList()
        {
            var result = await _service.GetFinalizedDraftsAsync();
            return Ok(ApiResponse<IEnumerable<IssueWindowDraftSummaryResponse>>.SuccessResponse(result));
        }

        // GET /api/stores/issue-window/drafts/{id}
        [HttpGet("drafts/{id:int}")]
        public async Task<IActionResult> GetDraftById(int id)
        {
            var result = await _service.GetDraftByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResponse($"Draft {id} not found"));
            return Ok(ApiResponse<IssueWindowDraftDetailResponse>.SuccessResponse(result));
        }

        // POST /api/stores/issue-window/drafts/{id}/finalize — Lock draft, move to Issue List
        [HttpPost("drafts/{id:int}/finalize")]
        public async Task<IActionResult> FinalizeDraft(int id)
        {
            var finalized = await _service.FinalizeDraftAsync(id);
            if (!finalized)
                return BadRequest(ApiResponse<object>.ErrorResponse($"Draft {id} not found or is not in Draft status"));
            return Ok(ApiResponse<object>.SuccessResponse(null!, $"Draft {id} finalized — it will appear in Issue List"));
        }

        // POST /api/stores/issue-window/drafts/{id}/issue — Issue a finalized draft
        [HttpPost("drafts/{id:int}/issue")]
        public async Task<IActionResult> IssueDraft(int id, [FromBody] IssueDraftRequest request)
        {
            try
            {
                var results = await _service.IssueDraftAsync(id, request);
                var allSuccess = results.All(r => r.Success);

                var response = ApiResponse<IEnumerable<IssueWindowIssueResultResponse>>.SuccessResponse(results);
                response.Message = allSuccess
                    ? $"Successfully issued {results.Count()} requisition(s)"
                    : "Some requisitions failed to issue";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        // POST /api/stores/issue-window/suggest-cutting-plan
        [HttpPost("suggest-cutting-plan")]
        public async Task<IActionResult> SuggestCuttingPlan([FromBody] SuggestCuttingPlanRequest request)
        {
            if (request.Cuts == null || !request.Cuts.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No cuts provided for suggestion"));

            var plans = await _service.SuggestCuttingPlanAsync(request);
            return Ok(ApiResponse<IEnumerable<CuttingPlanResponse>>.SuccessResponse(plans));
        }

        // DELETE /api/stores/issue-window/drafts/{id} — Delete Draft-status draft (releases reserved pieces)
        [HttpDelete("drafts/{id:int}")]
        public async Task<IActionResult> DeleteDraft(int id)
        {
            var deleted = await _service.DeleteDraftAsync(id);
            if (!deleted)
                return BadRequest(ApiResponse<object>.ErrorResponse($"Draft {id} not found or is Finalized/Issued (cannot delete)"));
            return Ok(ApiResponse<object>.SuccessResponse(null!, $"Draft {id} deleted — reserved materials released"));
        }
    }
}
