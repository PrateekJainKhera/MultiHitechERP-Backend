using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<Supplier>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<ApiResponse<Supplier>>> GetByCode(string code)
        {
            var response = await _service.GetByCodeAsync(code);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("by-type/{supplierType}")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetByType(string supplierType)
        {
            var response = await _service.GetByTypeAsync(supplierType);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetByCategory(string category)
        {
            var response = await _service.GetByCategoryAsync(category);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetActive()
        {
            var response = await _service.GetActiveAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("approved")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetApproved()
        {
            var response = await _service.GetApprovedAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-approval-status/{approvalStatus}")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetByApprovalStatus(string approvalStatus)
        {
            var response = await _service.GetByApprovalStatusAsync(approvalStatus);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-process-capability/{processCapability}")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetByProcessCapability(string processCapability)
        {
            var response = await _service.GetByProcessCapabilityAsync(processCapability);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("top-performing")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetTopPerforming([FromQuery] int count = 10)
        {
            var response = await _service.GetTopPerformingAsync(count);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("low-performing")]
        public async Task<ActionResult<ApiResponse<Supplier[]>>> GetLowPerforming([FromQuery] int count = 10)
        {
            var response = await _service.GetLowPerformingAsync(count);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateSupplierRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var supplier = new Supplier
            {
                SupplierCode = request.SupplierCode,
                SupplierName = request.SupplierName,
                SupplierType = request.SupplierType,
                Category = request.Category,
                ContactPerson = request.ContactPerson,
                ContactNumber = request.ContactNumber,
                Email = request.Email,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                Country = request.Country ?? "India",
                PostalCode = request.PostalCode,
                GSTNumber = request.GSTNumber,
                PaymentTerms = request.PaymentTerms,
                CreditDays = request.CreditDays,
                ProcessCapabilities = request.ProcessCapabilities,
                StandardLeadTimeDays = request.StandardLeadTimeDays,
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            var response = await _service.CreateSupplierAsync(supplier);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateSupplierRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            if (id != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            var supplier = new Supplier
            {
                Id = request.Id,
                SupplierCode = request.SupplierCode,
                SupplierName = request.SupplierName,
                SupplierType = request.SupplierType,
                Category = request.Category,
                ContactPerson = request.ContactPerson,
                ContactNumber = request.ContactNumber,
                Email = request.Email,
                AddressLine1 = request.AddressLine1,
                City = request.City,
                State = request.State,
                GSTNumber = request.GSTNumber,
                PaymentTerms = request.PaymentTerms,
                CreditDays = request.CreditDays,
                ProcessCapabilities = request.ProcessCapabilities,
                IsActive = request.IsActive ?? true,
                Remarks = request.Remarks,
                UpdatedBy = request.UpdatedBy
            };

            var response = await _service.UpdateSupplierAsync(supplier);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteSupplierAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> Approve(Guid id, [FromBody] ApproveRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.ApproveSupplierAsync(id, request.ApprovedBy);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<ActionResult<ApiResponse<bool>>> Reject(Guid id, [FromBody] RejectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.RejectSupplierAsync(id, request.Reason);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStatusAsync(id, request.Status);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id:guid}/performance")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdatePerformanceMetrics(Guid id, [FromBody] UpdatePerformanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdatePerformanceMetricsAsync(
                id,
                request.OnTimeDeliveryRate,
                request.QualityRating,
                request.TotalOrders,
                request.RejectedOrders
            );

            return response.Success ? Ok(response) : BadRequest(response);
        }
    }

    public class ApproveRequest
    {
        [Required] public string ApprovedBy { get; set; } = string.Empty;
    }

    public class RejectRequest
    {
        [Required] public string Reason { get; set; } = string.Empty;
    }

    public class UpdatePerformanceRequest
    {
        [Range(0, 100)] public decimal OnTimeDeliveryRate { get; set; }
        [Range(0, 5)] public decimal QualityRating { get; set; }
        public int TotalOrders { get; set; }
        public int RejectedOrders { get; set; }
    }
}
