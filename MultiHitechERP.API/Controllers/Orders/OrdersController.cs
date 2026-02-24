using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;
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
        private readonly IOrderCustomerDrawingRepository _customerDrawingRepo;
        private readonly ILogger<OrdersController> _logger;
        private readonly IS3Service _s3Service;

        public OrdersController(
            IOrderService orderService,
            IDrawingService drawingService,
            IOrderCustomerDrawingRepository customerDrawingRepo,
            ILogger<OrdersController> logger,
            IS3Service s3Service)
        {
            _orderService = orderService;
            _drawingService = drawingService;
            _customerDrawingRepo = customerDrawingRepo;
            _logger = logger;
            _s3Service = s3Service;
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

        // ─── Customer Drawing endpoints ──────────────────────────────────────

        /// <summary>Get all customer drawings for an order</summary>
        [HttpGet("{orderId:int}/customer-drawings")]
        public async Task<IActionResult> GetCustomerDrawings(int orderId)
        {
            var drawings = await _customerDrawingRepo.GetByOrderIdAsync(orderId);
            var response = drawings.Select(d => MapDrawing(d, Request)).ToList();
            return Ok(ApiResponse<List<OrderCustomerDrawingResponse>>.SuccessResponse(response));
        }

        /// <summary>Upload a customer drawing file for an order</summary>
        [HttpPost("{orderId:int}/customer-drawings")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCustomerDrawing(
            int orderId,
            IFormFile file,
            [FromForm] string drawingType = "other",
            [FromForm] string? notes = null,
            [FromForm] string? uploadedBy = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.ErrorResponse("No file provided"));

            var ext = Path.GetExtension(file.FileName);
            var storedName = $"{Guid.NewGuid()}{ext}";
            var s3Key = $"multihitech/order-drawings/{storedName}";

            using var fileStream = file.OpenReadStream();
            var fileUrl = await _s3Service.UploadAsync(fileStream, s3Key, file.ContentType);

            var drawing = new OrderCustomerDrawing
            {
                OrderId = orderId,
                OriginalFileName = file.FileName,
                StoredFileName = storedName,
                FilePath = s3Key,
                FileSize = file.Length,
                MimeType = file.ContentType,
                DrawingType = drawingType,
                Notes = notes,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = uploadedBy ?? "Admin",
            };

            var id = await _customerDrawingRepo.InsertAsync(drawing);
            drawing.Id = id;
            return Ok(ApiResponse<OrderCustomerDrawingResponse>.SuccessResponse(
                MapDrawing(drawing, Request), "Drawing uploaded"));
        }

        /// <summary>Delete a customer drawing</summary>
        [HttpDelete("{orderId:int}/customer-drawings/{drawingId:int}")]
        public async Task<IActionResult> DeleteCustomerDrawing(int orderId, int drawingId)
        {
            var drawing = await _customerDrawingRepo.GetByIdAsync(drawingId);
            if (drawing == null || drawing.OrderId != orderId)
                return NotFound(ApiResponse<object>.ErrorResponse("Drawing not found"));

            // Delete from S3 (FilePath now stores the s3Key)
            try { await _s3Service.DeleteAsync(drawing.FilePath); } catch { /* ignore if already gone */ }

            await _customerDrawingRepo.DeleteAsync(drawingId);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Drawing deleted"));
        }

        private static OrderCustomerDrawingResponse MapDrawing(OrderCustomerDrawing d, HttpRequest request)
        {
            // FilePath stores the S3 key (e.g. "order-drawings/guid.pdf")
            // Build the full public S3 URL for download
            var s3BaseUrl = "https://indas-analytics-prod.s3.ap-south-1.amazonaws.com";
            return new OrderCustomerDrawingResponse
            {
                Id = d.Id,
                OrderId = d.OrderId,
                OriginalFileName = d.OriginalFileName,
                DrawingType = d.DrawingType,
                Notes = d.Notes,
                FileSize = d.FileSize,
                MimeType = d.MimeType,
                DownloadUrl = $"{s3BaseUrl}/{d.FilePath}",
                UploadedAt = d.UploadedAt,
                UploadedBy = d.UploadedBy,
            };
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
