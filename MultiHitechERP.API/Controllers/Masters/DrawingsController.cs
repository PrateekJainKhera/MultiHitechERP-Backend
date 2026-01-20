using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrawingsController : ControllerBase
    {
        private readonly IDrawingService _drawingService;

        public DrawingsController(IDrawingService drawingService)
        {
            _drawingService = drawingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _drawingService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _drawingService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-number/{drawingNumber}")]
        public async Task<IActionResult> GetByDrawingNumber(string drawingNumber)
        {
            var result = await _drawingService.GetByDrawingNumberAsync(drawingNumber);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveDrawings()
        {
            var result = await _drawingService.GetActiveDrawingsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("revision-history/{drawingNumber}")]
        public async Task<IActionResult> GetRevisionHistory(string drawingNumber)
        {
            var result = await _drawingService.GetRevisionHistoryAsync(drawingNumber);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("latest-revision/{drawingNumber}")]
        public async Task<IActionResult> GetLatestRevision(string drawingNumber)
        {
            var result = await _drawingService.GetLatestRevisionAsync(drawingNumber);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetByProductId(Guid productId)
        {
            var result = await _drawingService.GetByProductIdAsync(productId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-type/{drawingType}")]
        public async Task<IActionResult> GetByDrawingType(string drawingType)
        {
            var result = await _drawingService.GetByDrawingTypeAsync(drawingType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("pending-approval")]
        public async Task<IActionResult> GetPendingApproval()
        {
            var result = await _drawingService.GetPendingApprovalAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDrawingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _drawingService.CreateDrawingAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDrawingRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _drawingService.UpdateDrawingAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _drawingService.DeleteDrawingAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/mark-latest")]
        public async Task<IActionResult> MarkAsLatestRevision(Guid id)
        {
            var result = await _drawingService.MarkAsLatestRevisionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
