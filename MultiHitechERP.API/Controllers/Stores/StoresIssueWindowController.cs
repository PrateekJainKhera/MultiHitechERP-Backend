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
        // Body: { requisitionIds: [1,2,3] }
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

        // POST /api/stores/issue-window/drafts
        [HttpPost("drafts")]
        public async Task<IActionResult> SaveDraft([FromBody] SaveDraftRequest request)
        {
            if (request.RequisitionIds == null || !request.RequisitionIds.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No requisitions in draft"));
            if (request.BarAssignments == null || !request.BarAssignments.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No bar assignments in draft"));

            var result = await _service.SaveDraftAsync(request);
            var response = ApiResponse<IssueWindowDraftDetailResponse>.SuccessResponse(result);
            response.Message = $"Draft {result.DraftNo} saved successfully";
            return Ok(response);
        }

        // GET /api/stores/issue-window/drafts
        [HttpGet("drafts")]
        public async Task<IActionResult> GetDrafts()
        {
            var result = await _service.GetDraftsAsync();
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

        // POST /api/stores/issue-window/drafts/{id}/issue
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
        // Body: SuggestCuttingPlanRequest { cuts, materialId, grade, diameterMM }
        [HttpPost("suggest-cutting-plan")]
        public async Task<IActionResult> SuggestCuttingPlan([FromBody] SuggestCuttingPlanRequest request)
        {
            if (request.Cuts == null || !request.Cuts.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No cuts provided for suggestion"));

            var plans = await _service.SuggestCuttingPlanAsync(request);
            return Ok(ApiResponse<IEnumerable<CuttingPlanResponse>>.SuccessResponse(plans));
        }

        // DELETE /api/stores/issue-window/drafts/{id}
        [HttpDelete("drafts/{id:int}")]
        public async Task<IActionResult> DeleteDraft(int id)
        {
            var deleted = await _service.DeleteDraftAsync(id);
            if (!deleted)
                return BadRequest(ApiResponse<object>.ErrorResponse($"Draft {id} not found or already issued (cannot delete)"));
            return Ok(ApiResponse<object>.SuccessResponse(null!, $"Draft {id} deleted successfully"));
        }

        // POST /api/stores/issue-window/finalize-multiple
        // Body: FinalizeMultipleDraftsRequest { draftIds, issuedBy, receivedBy }
        [HttpPost("finalize-multiple")]
        public async Task<IActionResult> FinalizeMultiple([FromBody] FinalizeMultipleDraftsRequest request)
        {
            if (request.DraftIds == null || !request.DraftIds.Any())
                return BadRequest(ApiResponse<object>.ErrorResponse("No draft IDs provided"));

            try
            {
                var results = await _service.FinalizeMultipleDraftsAsync(request);
                var allSuccess = results.All(r => r.Success);
                var response = ApiResponse<IEnumerable<IssueWindowIssueResultResponse>>.SuccessResponse(results);
                response.Message = allSuccess
                    ? $"All {request.DraftIds.Count} draft(s) issued successfully"
                    : "Some drafts failed to issue";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
