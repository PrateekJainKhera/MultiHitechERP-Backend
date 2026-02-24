using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/opening-stock")]
    [Produces("application/json")]
    public class OpeningStockController : ControllerBase
    {
        private readonly IOpeningStockService _service;

        public OpeningStockController(IOpeningStockService service)
        {
            _service = service;
        }

        // GET /api/opening-stock
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<OpeningStockSummaryResponse>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        // GET /api/opening-stock/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(ApiResponse<object>.ErrorResponse($"Entry {id} not found"));
                return Ok(ApiResponse<OpeningStockDetailResponse>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        // POST /api/opening-stock — Create draft
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOpeningStockRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                var response = ApiResponse<OpeningStockDetailResponse>.SuccessResponse(result);
                response.Message = $"Draft {result.EntryNo} created";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Create failed: {ex.Message}"));
            }
        }

        // PUT /api/opening-stock/{id} — Update draft items
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOpeningStockRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                var response = ApiResponse<OpeningStockDetailResponse>.SuccessResponse(result);
                response.Message = $"Entry {result.EntryNo} updated";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Update failed: {ex.Message}"));
            }
        }

        // POST /api/opening-stock/{id}/confirm — Confirm: creates pieces + updates stock
        [HttpPost("{id:int}/confirm")]
        public async Task<IActionResult> Confirm(int id, [FromBody] ConfirmOpeningStockRequest request)
        {
            try
            {
                var result = await _service.ConfirmAsync(id, request);
                var response = ApiResponse<OpeningStockDetailResponse>.SuccessResponse(result);
                response.Message = $"Entry {result.EntryNo} confirmed — {result.TotalPieces} pieces and {result.TotalComponents} component(s) added to stock";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Confirm failed: {ex.Message}"));
            }
        }

        // DELETE /api/opening-stock/{id} — Delete draft only
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Entry {id} not found or already confirmed (cannot delete)"));
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Entry deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse($"Delete failed: {ex.Message}"));
            }
        }
    }
}
