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
    /// BOM (Bill of Materials) API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BOMController : ControllerBase
    {
        private readonly IBOMService _service;

        public BOMController(IBOMService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all BOMs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get BOM by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<BOMResponse>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<BOMResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get BOM by BOM number
        /// </summary>
        [HttpGet("by-bom-no/{bomNo}")]
        public async Task<ActionResult<ApiResponse<BOMResponse>>> GetByBOMNo(string bomNo)
        {
            var response = await _service.GetByBOMNoAsync(bomNo);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<BOMResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get BOMs by product ID
        /// </summary>
        [HttpGet("by-product/{productId:guid}")]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetByProductId(Guid productId)
        {
            var response = await _service.GetByProductIdAsync(productId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get latest revision BOM for a product
        /// </summary>
        [HttpGet("latest-revision/{productId:guid}")]
        public async Task<ActionResult<ApiResponse<BOMResponse>>> GetLatestRevision(Guid productId)
        {
            var response = await _service.GetLatestRevisionAsync(productId);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<BOMResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get BOMs by product code
        /// </summary>
        [HttpGet("by-product-code/{productCode}")]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetByProductCode(string productCode)
        {
            var response = await _service.GetByProductCodeAsync(productCode);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get active BOMs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetActive()
        {
            var response = await _service.GetActiveAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get BOMs by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get BOMs by BOM type
        /// </summary>
        [HttpGet("by-type/{bomType}")]
        public async Task<ActionResult<ApiResponse<BOMResponse[]>>> GetByBOMType(string bomType)
        {
            var response = await _service.GetByBOMTypeAsync(bomType);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<BOMResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Create a new BOM
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateBOMRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var bom = new BOM
            {
                ProductId = request.ProductId,
                BOMNo = request.BOMNo ?? string.Empty,
                RevisionNumber = request.RevisionNumber,
                BOMType = request.BOMType,
                BaseQuantity = request.BaseQuantity,
                BaseUOM = request.BaseUOM,
                Remarks = request.Remarks,
                CreatedBy = request.CreatedBy
            };

            var response = await _service.CreateBOMAsync(bom);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update an existing BOM
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateBOMRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            if (id != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            var bom = new BOM
            {
                Id = request.Id,
                BOMNo = request.BOMNo ?? string.Empty,
                RevisionNumber = request.RevisionNumber,
                RevisionDate = request.RevisionDate,
                BOMType = request.BOMType,
                BaseQuantity = request.BaseQuantity,
                BaseUOM = request.BaseUOM,
                IsActive = request.IsActive ?? true,
                Status = request.Status,
                Remarks = request.Remarks,
                UpdatedBy = request.UpdatedBy
            };

            var response = await _service.UpdateBOMAsync(bom);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a BOM
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteBOMAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Approve a BOM
        /// </summary>
        [HttpPost("{id:guid}/approve")]
        public async Task<ActionResult<ApiResponse<bool>>> Approve(Guid id, [FromBody] ApproveBOMRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.ApproveBOMAsync(id, request.ApprovedBy);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update BOM status
        /// </summary>
        [HttpPatch("{id:guid}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateStatusAsync(id, request.Status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Create a new BOM revision
        /// </summary>
        [HttpPost("create-revision")]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateRevision([FromBody] CreateBOMRevisionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.CreateRevisionAsync(request.BOMId, request.RevisionNumber, request.CreatedBy);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // BOM Items Endpoints

        /// <summary>
        /// Get all items for a BOM
        /// </summary>
        [HttpGet("{bomId:guid}/items")]
        public async Task<ActionResult<ApiResponse<BOMItemResponse[]>>> GetBOMItems(Guid bomId)
        {
            var response = await _service.GetBOMItemsAsync(bomId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToBOMItemResponse).ToArray();
            return Ok(ApiResponse<BOMItemResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get material items for a BOM
        /// </summary>
        [HttpGet("{bomId:guid}/items/materials")]
        public async Task<ActionResult<ApiResponse<BOMItemResponse[]>>> GetMaterialItems(Guid bomId)
        {
            var response = await _service.GetMaterialItemsAsync(bomId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToBOMItemResponse).ToArray();
            return Ok(ApiResponse<BOMItemResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get child part items for a BOM
        /// </summary>
        [HttpGet("{bomId:guid}/items/child-parts")]
        public async Task<ActionResult<ApiResponse<BOMItemResponse[]>>> GetChildPartItems(Guid bomId)
        {
            var response = await _service.GetChildPartItemsAsync(bomId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToBOMItemResponse).ToArray();
            return Ok(ApiResponse<BOMItemResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get items by type for a BOM
        /// </summary>
        [HttpGet("{bomId:guid}/items/by-type/{itemType}")]
        public async Task<ActionResult<ApiResponse<BOMItemResponse[]>>> GetItemsByType(Guid bomId, string itemType)
        {
            var response = await _service.GetItemsByTypeAsync(bomId, itemType);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToBOMItemResponse).ToArray();
            return Ok(ApiResponse<BOMItemResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Add a BOM item
        /// </summary>
        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<Guid>>> AddBOMItem([FromBody] AddBOMItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var item = new BOMItem
            {
                BOMId = request.BOMId,
                LineNo = request.LineNo,
                ItemType = request.ItemType,
                MaterialId = request.MaterialId,
                ChildPartId = request.ChildPartId,
                QuantityRequired = request.QuantityRequired,
                UOM = request.UOM,
                LengthRequiredMM = request.LengthRequiredMM,
                ScrapPercentage = request.ScrapPercentage,
                ScrapQuantity = request.ScrapQuantity,
                WastagePercentage = request.WastagePercentage,
                ReferenceDesignator = request.ReferenceDesignator,
                Notes = request.Notes
            };

            var response = await _service.AddBOMItemAsync(item);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update a BOM item
        /// </summary>
        [HttpPut("items/{itemId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateBOMItem(Guid itemId, [FromBody] UpdateBOMItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            if (itemId != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            var item = new BOMItem
            {
                Id = request.Id,
                LineNo = request.LineNo,
                ItemType = request.ItemType ?? "Material",
                MaterialId = request.MaterialId,
                ChildPartId = request.ChildPartId,
                QuantityRequired = request.QuantityRequired,
                UOM = request.UOM,
                LengthRequiredMM = request.LengthRequiredMM,
                ScrapPercentage = request.ScrapPercentage,
                ScrapQuantity = request.ScrapQuantity,
                WastagePercentage = request.WastagePercentage,
                ReferenceDesignator = request.ReferenceDesignator,
                Notes = request.Notes
            };

            var response = await _service.UpdateBOMItemAsync(item);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a BOM item
        /// </summary>
        [HttpDelete("items/{itemId:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBOMItem(Guid itemId)
        {
            var response = await _service.DeleteBOMItemAsync(itemId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete all BOM items
        /// </summary>
        [HttpDelete("{bomId:guid}/items")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAllBOMItems(Guid bomId)
        {
            var response = await _service.DeleteAllBOMItemsAsync(bomId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Helper Methods
        private static BOMResponse MapToResponse(BOM bom)
        {
            return new BOMResponse
            {
                Id = bom.Id,
                BOMNo = bom.BOMNo,
                ProductId = bom.ProductId,
                ProductCode = bom.ProductCode,
                ProductName = bom.ProductName,
                RevisionNumber = bom.RevisionNumber,
                RevisionDate = bom.RevisionDate,
                IsLatestRevision = bom.IsLatestRevision,
                BOMType = bom.BOMType,
                BaseQuantity = bom.BaseQuantity,
                BaseUOM = bom.BaseUOM,
                IsActive = bom.IsActive,
                Status = bom.Status,
                ApprovedBy = bom.ApprovedBy,
                ApprovalDate = bom.ApprovalDate,
                Remarks = bom.Remarks,
                CreatedAt = bom.CreatedAt,
                CreatedBy = bom.CreatedBy,
                UpdatedAt = bom.UpdatedAt,
                UpdatedBy = bom.UpdatedBy
            };
        }

        private static BOMItemResponse MapToBOMItemResponse(BOMItem item)
        {
            return new BOMItemResponse
            {
                Id = item.Id,
                BOMId = item.BOMId,
                LineNo = item.LineNo,
                ItemType = item.ItemType,
                MaterialId = item.MaterialId,
                MaterialCode = item.MaterialCode,
                MaterialName = item.MaterialName,
                ChildPartId = item.ChildPartId,
                ChildPartCode = item.ChildPartCode,
                ChildPartName = item.ChildPartName,
                QuantityRequired = item.QuantityRequired,
                UOM = item.UOM,
                LengthRequiredMM = item.LengthRequiredMM,
                ScrapPercentage = item.ScrapPercentage,
                ScrapQuantity = item.ScrapQuantity,
                WastagePercentage = item.WastagePercentage,
                NetQuantityRequired = item.NetQuantityRequired,
                ReferenceDesignator = item.ReferenceDesignator,
                Notes = item.Notes,
                CreatedAt = item.CreatedAt
            };
        }
    }
}
