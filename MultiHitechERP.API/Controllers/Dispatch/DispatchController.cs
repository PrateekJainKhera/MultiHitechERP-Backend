using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using Microsoft.AspNetCore.Http;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Dispatch;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Dispatch
{
    /// <summary>
    /// Dispatch API endpoints
    /// Handles delivery challan creation, dispatch, and delivery tracking
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DispatchController : ControllerBase
    {
        private readonly IDispatchService _service;

        public DispatchController(IDispatchService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all delivery challans
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get delivery challan by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<DeliveryChallanResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get delivery challan by challan number
        /// </summary>
        [HttpGet("by-challan-no/{challanNo}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse>>> GetByChallanNo(string challanNo)
        {
            var response = await _service.GetByChallanNoAsync(challanNo);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<DeliveryChallanResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get delivery challans by order ID
        /// </summary>
        [HttpGet("by-order/{orderId:guid}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetByOrderId(int orderId)
        {
            var response = await _service.GetByOrderIdAsync(orderId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get delivery challans by customer ID
        /// </summary>
        [HttpGet("by-customer/{customerId:guid}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetByCustomerId(int customerId)
        {
            var response = await _service.GetByCustomerIdAsync(customerId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get delivery challans by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get delivery challans by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = await _service.GetByDateRangeAsync(startDate, endDate);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get delivery challans by vehicle number
        /// </summary>
        [HttpGet("by-vehicle/{vehicleNumber}")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetByVehicleNumber(string vehicleNumber)
        {
            var response = await _service.GetByVehicleNumberAsync(vehicleNumber);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get pending delivery challans
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetPendingChallans()
        {
            var response = await _service.GetPendingChallansAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get dispatched delivery challans
        /// </summary>
        [HttpGet("dispatched")]
        public async Task<ActionResult<ApiResponse<DeliveryChallanResponse[]>>> GetDispatchedChallans()
        {
            var response = await _service.GetDispatchedChallansAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<DeliveryChallanResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Create a new delivery challan
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateChallan([FromBody] CreateDispatchChallanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.CreateDispatchChallanAsync(
                request.OrderId,
                request.QuantityDispatched,
                request.DeliveryAddress,
                request.TransportMode,
                request.VehicleNumber,
                request.DriverName,
                request.DriverContact);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Dispatch a delivery challan
        /// </summary>
        [HttpPost("{id:guid}/dispatch")]
        public async Task<ActionResult<ApiResponse<bool>>> DispatchChallan(int id)
        {
            var response = await _service.DispatchChallanAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Mark delivery challan as delivered
        /// </summary>
        [HttpPost("{id:guid}/deliver")]
        public async Task<ActionResult<ApiResponse<bool>>> DeliverChallan(int id, [FromBody] DeliverChallanRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.DeliverChallanAsync(id, request.ReceivedBy);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update delivery challan status
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateQCStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a delivery challan
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _service.DeleteChallanAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get items ready to dispatch (completed but not yet fully dispatched)
        /// </summary>
        [HttpGet("ready")]
        public async Task<ActionResult<ApiResponse<List<ReadyToDispatchItem>>>> GetReadyToDispatch()
        {
            var response = await _service.GetReadyToDispatchAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Simple dispatch: create challan directly from an order item with optional invoice document
        /// </summary>
        [HttpPost("simple-dispatch")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponse<int>>> SimpleDispatch(
            [FromForm] int orderItemId,
            [FromForm] int qtyToDispatch,
            [FromForm] DateTime dispatchDate,
            [FromForm] string? invoiceNo,
            [FromForm] DateTime? invoiceDate,
            [FromForm] string? remarks,
            IFormFile? invoiceDocument)
        {
            string? invoiceDocPath = null;

            if (invoiceDocument != null && invoiceDocument.Length > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "dispatch");
                Directory.CreateDirectory(uploadDir);
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(invoiceDocument.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await invoiceDocument.CopyToAsync(stream);
                invoiceDocPath = $"/uploads/dispatch/{fileName}";
            }

            var response = await _service.SimpleDispatchAsync(
                orderItemId, qtyToDispatch, dispatchDate,
                invoiceNo, invoiceDate, invoiceDocPath, remarks);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Helper Methods
        private static DeliveryChallanResponse MapToResponse(Models.Dispatch.DeliveryChallan challan)
        {
            return new DeliveryChallanResponse
            {
                Id = challan.Id,
                ChallanNo = challan.ChallanNo,
                ChallanDate = challan.ChallanDate,
                OrderId = challan.OrderId,
                OrderNo = challan.OrderNo,
                CustomerId = challan.CustomerId,
                CustomerName = challan.CustomerName,
                ProductId = challan.ProductId,
                ProductName = challan.ProductName,
                QuantityDispatched = challan.QuantityDispatched,
                DeliveryDate = challan.DeliveryDate,
                DeliveryAddress = challan.DeliveryAddress,
                TransportMode = challan.TransportMode,
                VehicleNumber = challan.VehicleNumber,
                DriverName = challan.DriverName,
                DriverContact = challan.DriverContact,
                NumberOfPackages = challan.NumberOfPackages,
                PackagingType = challan.PackagingType,
                TotalWeight = challan.TotalWeight,
                Status = challan.Status,
                DispatchedAt = challan.DispatchedAt,
                DeliveredAt = challan.DeliveredAt,
                InvoiceNo = challan.InvoiceNo,
                InvoiceDate = challan.InvoiceDate,
                ReceivedBy = challan.ReceivedBy,
                AcknowledgedAt = challan.AcknowledgedAt,
                DeliveryRemarks = challan.DeliveryRemarks,
                Remarks = challan.Remarks,
                CreatedAt = challan.CreatedAt,
                CreatedBy = challan.CreatedBy
            };
        }
    }
}
