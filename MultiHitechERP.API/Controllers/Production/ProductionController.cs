using System;
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
        /// Production dashboard — all orders with at least one scheduled job card (LEGACY)
        /// </summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _productionService.GetOrdersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// GET /api/production/order-items
        /// Production dashboard — all order items with at least one scheduled job card (NEW)
        /// </summary>
        [HttpGet("order-items")]
        public async Task<IActionResult> GetOrderItems()
        {
            var result = await _productionService.GetOrderItemsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// GET /api/production/orders/{orderId}
        /// Full production detail for one order: child parts → steps → assembly (LEGACY)
        /// </summary>
        [HttpGet("orders/{orderId:int}")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var result = await _productionService.GetOrderDetailAsync(orderId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// GET /api/production/order-items/{orderItemId}
        /// Full production detail for one order item: child parts → steps → assembly (NEW)
        /// </summary>
        [HttpGet("order-items/{orderItemId:int}")]
        public async Task<IActionResult> GetOrderItemDetail(int orderItemId)
        {
            var result = await _productionService.GetOrderItemDetailAsync(orderItemId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// POST /api/production/job-cards/{jobCardId}/action
        /// Operator action: { "action": "start" | "pause" | "resume" | "complete" | "direct-complete", ... }
        /// </summary>
        [HttpPost("job-cards/{jobCardId:int}/action")]
        public async Task<IActionResult> Action(int jobCardId, [FromBody] ProductionActionRequest request)
        {
            var result = await _productionService.HandleActionAsync(jobCardId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// GET /api/production/execution-view
        /// Process-based execution view: ProcessCategory → ChildPart → Orders (all scheduled job cards)
        /// </summary>
        [HttpGet("execution-view")]
        public async Task<IActionResult> GetExecutionView()
        {
            try
            {
                var result = await _productionService.GetExecutionViewAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
