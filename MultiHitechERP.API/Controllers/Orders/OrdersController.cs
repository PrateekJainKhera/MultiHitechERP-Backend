using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Orders
{
    /// <summary>
    /// API Controller for Order management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IDrawingService _drawingService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IOrderService orderService,
            IDrawingService drawingService,
            ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _drawingService = drawingService;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all orders");

            var response = await _orderService.GetAllAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation("Getting order with ID: {OrderId}", id);

            var response = await _orderService.GetByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get order by order number
        /// </summary>
        [HttpGet("by-order-no/{orderNo}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByOrderNo(string orderNo)
        {
            _logger.LogInformation("Getting order: {OrderNo}", orderNo);

            var response = await _orderService.GetByOrderNoAsync(orderNo);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get orders by customer ID
        /// </summary>
        [HttpGet("by-customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            _logger.LogInformation("Getting orders for customer: {CustomerId}", customerId);

            var response = await _orderService.GetByCustomerIdAsync(customerId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            _logger.LogInformation("Getting orders with status: {Status}", status);

            var response = await _orderService.GetByStatusAsync(status);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get orders pending drawing review
        /// </summary>
        [HttpGet("pending-drawing-review")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetPendingDrawingReview()
        {
            _logger.LogInformation("Getting orders pending drawing review");

            var response = await _orderService.GetPendingDrawingReviewAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get orders ready for planning
        /// </summary>
        [HttpGet("ready-for-planning")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetReadyForPlanning()
        {
            _logger.LogInformation("Getting orders ready for planning");

            var response = await _orderService.GetReadyForPlanningAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get in-progress orders
        /// </summary>
        [HttpGet("in-progress")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetInProgress()
        {
            _logger.LogInformation("Getting in-progress orders");

            var response = await _orderService.GetInProgressOrdersAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get delayed orders
        /// </summary>
        [HttpGet("delayed")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
        public async Task<IActionResult> GetDelayed()
        {
            _logger.LogInformation("Getting delayed orders");

            var response = await _orderService.GetDelayedOrdersAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            _logger.LogInformation("Creating new order");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _orderService.CreateOrderAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.Data },
                response
            );
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderRequest request)
        {
            _logger.LogInformation("Updating order: {OrderId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            var response = await _orderService.UpdateOrderAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting order: {OrderId}", id);

            var response = await _orderService.DeleteOrderAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Update drawing review status (GATE)
        /// </summary>
        [HttpPost("{id}/drawing-review")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateDrawingReview(
            int id,
            [FromBody] UpdateDrawingReviewRequest request)
        {
            _logger.LogInformation("Updating drawing review for order: {OrderId}", id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.OrderId)
            {
                return BadRequest("ID mismatch");
            }

            var response = await _orderService.UpdateDrawingReviewStatusAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Approve drawing review
        /// </summary>
        [HttpPost("{id}/drawing-review/approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApproveDrawingReview(
            int id,
            [FromBody] ApproveDrawingReviewRequest request)
        {
            _logger.LogInformation("Approving drawing review for order: {OrderId}", id);

            var response = await _orderService.ApproveDrawingReviewAsync(
                id,
                request.ReviewedBy,
                request.Notes,
                request.LinkedProductTemplateId
            );

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Reject drawing review
        /// </summary>
        [HttpPost("{id}/drawing-review/reject")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RejectDrawingReview(
            int id,
            [FromBody] RejectDrawingReviewRequest request)
        {
            _logger.LogInformation("Rejecting drawing review for order: {OrderId}", id);

            var response = await _orderService.RejectDrawingReviewAsync(
                id,
                request.ReviewedBy,
                request.Reason
            );

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Check if order can generate job cards
        /// </summary>
        [HttpGet("{id}/can-generate-job-cards")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> CanGenerateJobCards(int id)
        {
            _logger.LogInformation("Checking if order {OrderId} can generate job cards", id);

            var response = await _orderService.CanGenerateJobCardsAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Generate next order number
        /// </summary>
        [HttpGet("generate-order-no")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> GenerateOrderNo()
        {
            _logger.LogInformation("Generating order number");

            var response = await _orderService.GenerateOrderNoAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Get all drawings linked to an order
        /// </summary>
        [HttpGet("{id}/drawings")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DTOs.Response.DrawingResponse>>), 200)]
        public async Task<IActionResult> GetOrderDrawings(int id)
        {
            _logger.LogInformation("Getting drawings for order: {OrderId}", id);

            var response = await _drawingService.GetDrawingsByOrderIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }

    // Helper DTOs for specific actions
    public class ApproveDrawingReviewRequest
    {
        [Required(ErrorMessage = "ReviewedBy is required")]
        public string ReviewedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Business Rule: Product template must be linked before approval
        [Required(ErrorMessage = "LinkedProductTemplateId is required for approval")]
        public int LinkedProductTemplateId { get; set; }
    }

    public class RejectDrawingReviewRequest
    {
        public string ReviewedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
