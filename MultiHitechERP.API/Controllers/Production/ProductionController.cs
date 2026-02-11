using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Production
{
    [ApiController]
    [Route("api/production")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;
        }

        /// <summary>
        /// GET /api/production/orders
        /// Production dashboard — all orders with at least one scheduled job card
        /// </summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _productionService.GetOrdersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// GET /api/production/orders/{orderId}
        /// Full production detail for one order: child parts → steps → assembly
        /// </summary>
        [HttpGet("orders/{orderId:int}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var result = await _productionService.GetOrderDetailAsync(orderId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// POST /api/production/job-cards/{jobCardId}/action
        /// Operator action: { "action": "start" | "pause" | "resume" | "complete", ... }
        /// </summary>
        [HttpPost("job-cards/{jobCardId:int}/action")]
        public async Task<IActionResult> Action(int jobCardId, [FromBody] ProductionActionRequest request)
        {
            var result = await _productionService.HandleActionAsync(jobCardId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
