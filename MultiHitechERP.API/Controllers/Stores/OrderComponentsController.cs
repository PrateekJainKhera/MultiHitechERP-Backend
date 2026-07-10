using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/order-components")]
    public class OrderComponentsController : ControllerBase
    {
        private readonly IComponentConsumptionService _service;

        public OrderComponentsController(IComponentConsumptionService service)
        {
            _service = service;
        }

        // Reserve floor stock for an order-item's planned components (called at job-card generation).
        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveComponentsRequest request)
        {
            var result = await _service.ReserveForOrderItemAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Consume one-or-many (order × component) pairs from the shop floor.
        [HttpPost("consume")]
        public async Task<IActionResult> Consume([FromBody] ConsumeComponentsRequest request)
        {
            var result = await _service.ConsumeAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Components reserved/consumed against an order (for order detail).
        [HttpGet("by-order/{orderId:int}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var result = await _service.GetByOrderAsync(orderId);
            return Ok(result);
        }
    }
}
