using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductChildPartDrawingRepository _childPartDrawingRepository;
        private readonly IDrawingRepository _drawingRepository;
        private readonly IChildPartTemplateRepository _childPartTemplateRepository;

        public ProductsController(
            IProductService productService,
            IProductChildPartDrawingRepository childPartDrawingRepository,
            IDrawingRepository drawingRepository,
            IChildPartTemplateRepository childPartTemplateRepository)
        {
            _productService = productService;
            _childPartDrawingRepository = childPartDrawingRepository;
            _drawingRepository = drawingRepository;
            _childPartTemplateRepository = childPartTemplateRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{partCode}")]
        public async Task<IActionResult> GetByPartCode(string partCode)
        {
            var result = await _productService.GetByPartCodeAsync(partCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var result = await _productService.SearchByNameAsync(searchTerm);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search products by machine model, roller type, and number of teeth
        /// Used for order creation to find matching products
        /// </summary>
        [HttpGet("search-by-criteria")]
        public async Task<IActionResult> SearchByCriteria(
            [FromQuery] int modelId,
            [FromQuery] string rollerType,
            [FromQuery] int numberOfTeeth)
        {
            var result = await _productService.SearchByCriteriaAsync(modelId, rollerType, numberOfTeeth);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-roller-type/{rollerType}")]
        public async Task<IActionResult> GetByRollerType(string rollerType)
        {
            var result = await _productService.GetByRollerTypeAsync(rollerType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateProductAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update drawing review status for a product
        /// Used by drawing review team to approve/reject/request revision
        /// </summary>
        [HttpPut("{id}/drawing-review-status")]
        public async Task<IActionResult> UpdateDrawingReviewStatus(int id, [FromBody] UpdateDrawingReviewStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateDrawingReviewStatusAsync(
                id,
                request.DrawingReviewStatus,
                request.DrawingReviewNotes,
                request.DrawingReviewedBy
            );

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Request drawing creation from drawing team
        /// Changes product status to UnderReview and notifies drawing team
        /// </summary>
        [HttpPost("{id}/request-drawing")]
        public async Task<IActionResult> RequestDrawing(int id, [FromBody] RequestDrawingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.RequestDrawingAsync(id, request.RequestedBy);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Link a child part drawing to a product
        /// Creates a record in Product_ChildPartDrawings table
        /// </summary>
        [HttpPost("{id}/child-part-drawings")]
        public async Task<IActionResult> LinkChildPartDrawing(int id, [FromBody] LinkChildPartDrawingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if product exists
                var productResult = await _productService.GetByIdAsync(id);
                if (!productResult.Success)
                    return NotFound(ApiResponse<int>.ErrorResponse($"Product with ID {id} not found"));

                // Check if drawing exists
                var drawing = await _drawingRepository.GetByIdAsync(request.DrawingId);
                if (drawing == null)
                    return NotFound(ApiResponse<int>.ErrorResponse($"Drawing with ID {request.DrawingId} not found"));

                // Check if this child part is already linked (update if exists)
                var existing = await _childPartDrawingRepository.GetByProductAndChildPartAsync(id, request.ChildPartTemplateId);
                if (existing != null)
                {
                    // Update existing record
                    existing.DrawingId = request.DrawingId;
                    existing.UpdatedBy = request.CreatedBy;
                    await _childPartDrawingRepository.UpdateAsync(existing);
                    return Ok(ApiResponse<int>.SuccessResponse(existing.Id, "Child part drawing updated successfully"));
                }

                // Create new record
                var record = new Models.Masters.ProductChildPartDrawing
                {
                    ProductId = id,
                    ChildPartTemplateId = request.ChildPartTemplateId,
                    DrawingId = request.DrawingId,
                    CreatedBy = request.CreatedBy
                };

                var recordId = await _childPartDrawingRepository.CreateAsync(record);
                return Ok(ApiResponse<int>.SuccessResponse(recordId, "Child part drawing linked successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse($"Error linking child part drawing: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all child part drawings for a product with drawing details
        /// Returns child part template info + drawing file info
        /// </summary>
        [HttpGet("{id}/child-part-drawings")]
        public async Task<IActionResult> GetChildPartDrawings(int id)
        {
            try
            {
                // Get all child part drawing records for this product
                var records = await _childPartDrawingRepository.GetByProductIdAsync(id);
                var responses = new System.Collections.Generic.List<ProductChildPartDrawingResponse>();

                foreach (var record in records)
                {
                    // Get drawing details
                    var drawing = await _drawingRepository.GetByIdAsync(record.DrawingId);

                    // Get child part template name
                    var childPartTemplate = await _childPartTemplateRepository.GetByIdAsync(record.ChildPartTemplateId);

                    responses.Add(new ProductChildPartDrawingResponse
                    {
                        Id = record.Id,
                        ProductId = record.ProductId,
                        ChildPartTemplateId = record.ChildPartTemplateId,
                        ChildPartTemplateName = childPartTemplate?.TemplateName ?? "Unknown",
                        DrawingId = record.DrawingId,
                        DrawingNumber = drawing?.DrawingNumber,
                        DrawingName = drawing?.DrawingName,
                        FileName = drawing?.FileName,
                        FileType = drawing?.FileType,
                        FileUrl = drawing?.FileUrl,
                        FileSize = drawing?.FileSize,
                        Status = drawing?.Status,
                        CreatedAt = record.CreatedAt,
                        CreatedBy = record.CreatedBy
                    });
                }

                return Ok(ApiResponse<System.Collections.Generic.List<ProductChildPartDrawingResponse>>.SuccessResponse(responses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<System.Collections.Generic.List<ProductChildPartDrawingResponse>>.ErrorResponse($"Error fetching child part drawings: {ex.Message}"));
            }
        }
    }
}
