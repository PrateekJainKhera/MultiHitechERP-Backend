using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    /// <summary>
    /// ChildPart API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChildPartController : ControllerBase
    {
        private readonly IChildPartService _service;

        public ChildPartController(IChildPartService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all child parts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child part by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<ChildPartResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get child part by code
        /// </summary>
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse>>> GetByCode(string code)
        {
            var response = await _service.GetByCodeAsync(code);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<ChildPartResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get child parts by product ID
        /// </summary>
        [HttpGet("by-product/{productId:guid}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByProductId(int productId)
        {
            var response = await _service.GetByProductIdAsync(productId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by product code
        /// </summary>
        [HttpGet("by-product-code/{productCode}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByProductCode(string productCode)
        {
            var response = await _service.GetByProductCodeAsync(productCode);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by material ID
        /// </summary>
        [HttpGet("by-material/{materialId:guid}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByMaterialId(int materialId)
        {
            var response = await _service.GetByMaterialIdAsync(materialId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by part type
        /// </summary>
        [HttpGet("by-part-type/{partType}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByPartType(string partType)
        {
            var response = await _service.GetByPartTypeAsync(partType);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByCategory(string category)
        {
            var response = await _service.GetByCategoryAsync(category);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get active child parts
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetActive()
        {
            var response = await _service.GetActiveAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by make or buy
        /// </summary>
        [HttpGet("by-make-or-buy/{makeOrBuy}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByMakeOrBuy(string makeOrBuy)
        {
            var response = await _service.GetByMakeOrBuyAsync(makeOrBuy);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by drawing ID
        /// </summary>
        [HttpGet("by-drawing/{drawingId:guid}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByDrawingId(int drawingId)
        {
            var response = await _service.GetByDrawingIdAsync(drawingId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child parts by process template ID
        /// </summary>
        [HttpGet("by-process-template/{processTemplateId:guid}")]
        public async Task<ActionResult<ApiResponse<ChildPartResponse[]>>> GetByProcessTemplateId(int processTemplateId)
        {
            var response = await _service.GetByProcessTemplateIdAsync(processTemplateId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<ChildPartResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Create a new child part
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateChildPartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var childPart = new ChildPart
            {
                ChildPartCode = request.ChildPartCode,
                ChildPartName = request.ChildPartName,
                ProductId = request.ProductId,
                PartType = request.PartType,
                Category = request.Category,
                Description = request.Description,
                Specification = request.Specification,
                DrawingId = request.DrawingId,
                DrawingNumber = request.DrawingNumber,
                ProcessTemplateId = request.ProcessTemplateId,
                MaterialId = request.MaterialId,
                MaterialCode = request.MaterialCode,
                MaterialGrade = request.MaterialGrade,
                Length = request.Length,
                Diameter = request.Diameter,
                Weight = request.Weight,
                UOM = request.UOM,
                QuantityPerProduct = request.QuantityPerProduct,
                MakeOrBuy = request.MakeOrBuy,
                PreferredSupplierId = request.PreferredSupplierId,
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            var response = await _service.CreateChildPartAsync(childPart);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update an existing child part
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] UpdateChildPartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            if (id != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            var childPart = new ChildPart
            {
                Id = request.Id,
                ChildPartCode = request.ChildPartCode,
                ChildPartName = request.ChildPartName,
                ProductId = request.ProductId,
                PartType = request.PartType,
                Category = request.Category,
                Description = request.Description,
                Specification = request.Specification,
                DrawingId = request.DrawingId,
                DrawingNumber = request.DrawingNumber,
                ProcessTemplateId = request.ProcessTemplateId,
                MaterialId = request.MaterialId,
                MaterialCode = request.MaterialCode,
                MaterialGrade = request.MaterialGrade,
                Length = request.Length,
                Diameter = request.Diameter,
                Weight = request.Weight,
                UOM = request.UOM,
                QuantityPerProduct = request.QuantityPerProduct,
                MakeOrBuy = request.MakeOrBuy,
                PreferredSupplierId = request.PreferredSupplierId,
                IsActive = request.IsActive ?? true,
                Status = request.Status,
                Remarks = request.Remarks,
                UpdatedBy = request.UpdatedBy
            };

            var response = await _service.UpdateChildPartAsync(childPart);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a child part
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _service.DeleteChildPartAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update child part status
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Helper Method
        private static ChildPartResponse MapToResponse(ChildPart childPart)
        {
            return new ChildPartResponse
            {
                Id = childPart.Id,
                ChildPartCode = childPart.ChildPartCode,
                ChildPartName = childPart.ChildPartName,
                ProductId = childPart.ProductId,
                ProductCode = childPart.ProductCode,
                ProductName = childPart.ProductName,
                PartType = childPart.PartType,
                Category = childPart.Category,
                Description = childPart.Description,
                Specification = childPart.Specification,
                DrawingId = childPart.DrawingId,
                DrawingNumber = childPart.DrawingNumber,
                ProcessTemplateId = childPart.ProcessTemplateId,
                MaterialId = childPart.MaterialId,
                MaterialCode = childPart.MaterialCode,
                MaterialGrade = childPart.MaterialGrade,
                Length = childPart.Length,
                Diameter = childPart.Diameter,
                Weight = childPart.Weight,
                UOM = childPart.UOM,
                QuantityPerProduct = childPart.QuantityPerProduct,
                MakeOrBuy = childPart.MakeOrBuy,
                PreferredSupplierId = childPart.PreferredSupplierId,
                IsActive = childPart.IsActive,
                Status = childPart.Status,
                Remarks = childPart.Remarks,
                CreatedAt = childPart.CreatedAt,
                CreatedBy = childPart.CreatedBy,
                UpdatedAt = childPart.UpdatedAt,
                UpdatedBy = childPart.UpdatedBy
            };
        }
    }
}
