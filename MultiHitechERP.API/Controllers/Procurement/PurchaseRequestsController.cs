using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Procurement
{
    [ApiController]
    [Route("api/purchase-requests")]
    public class PurchaseRequestsController : ControllerBase
    {
        private readonly IPurchaseRequestService _service;

        public PurchaseRequestsController(IPurchaseRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
            => Ok(await _service.GetByStatusAsync(status));

        [HttpGet("by-type/{itemType}")]
        public async Task<IActionResult> GetByType(string itemType)
            => Ok(await _service.GetByItemTypeAsync(itemType));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseRequestRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/submit")]
        public async Task<IActionResult> Submit(int id)
        {
            var result = await _service.SubmitAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/start-review")]
        public async Task<IActionResult> StartReview(int id)
        {
            var result = await _service.StartReviewAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] ApprovePurchaseRequestRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.ApproveAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectPurchaseRequestRequest request)
        {
            var result = await _service.RejectAsync(id, request.RejectionReason, request.RejectedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:int}/items")]
        public async Task<IActionResult> AddItem(int id, [FromBody] CreatePurchaseRequestItemRequest item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.AddItemAsync(id, item);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}/items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(int id, int itemId)
        {
            var result = await _service.RemoveItemAsync(id, itemId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    public class RejectPurchaseRequestRequest
    {
        public string RejectionReason { get; set; } = string.Empty;
        public string RejectedBy { get; set; } = "Admin";
    }
}
