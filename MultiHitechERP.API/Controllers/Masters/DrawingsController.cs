using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/drawings")]
    public class DrawingsController : ControllerBase
    {
        private readonly IDrawingService _drawingService;
        private readonly IS3Service _s3Service;

        public DrawingsController(IDrawingService drawingService, IS3Service s3Service)
        {
            _drawingService = drawingService;
            _s3Service = s3Service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _drawingService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _drawingService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDrawingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _drawingService.CreateDrawingAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDrawingRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _drawingService.UpdateDrawingAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _drawingService.DeleteDrawingAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDrawing([FromForm] IFormFile file, [FromForm] CreateDrawingRequest request)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(ApiResponse<int>.ErrorResponse("No file uploaded"));

                // Validate file type
                var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".dwg" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedExtensions, ext => ext == extension))
                    return BadRequest(ApiResponse<int>.ErrorResponse("Invalid file type. Only PDF, PNG, JPG, and DWG files are allowed"));

                // Validate file size (max 10MB)
                if (file.Length > 10 * 1024 * 1024)
                    return BadRequest(ApiResponse<int>.ErrorResponse("File size exceeds 10MB limit"));

                // Generate S3 key
                var year = DateTime.Now.Year.ToString();
                var month = DateTime.Now.Month.ToString("D2");
                var drawingNumber = !string.IsNullOrWhiteSpace(request.DrawingNumber)
                    ? request.DrawingNumber.Trim()
                    : $"DWG-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                var fileName = $"{drawingNumber}_{Path.GetFileName(file.FileName)}";
                var s3Key = $"multihitech/drawings/{year}/{month}/{fileName}";

                // Upload to S3
                using var fileStream = file.OpenReadStream();
                var fileUrl = await _s3Service.UploadAsync(fileStream, s3Key, file.ContentType);

                // Update request with file information
                request.DrawingNumber = drawingNumber;
                request.FileName = Path.GetFileName(file.FileName);
                request.FileType = extension.TrimStart('.');
                request.FileUrl = fileUrl;
                request.FileSize = (decimal)(file.Length / 1024.0);

                // Create drawing record
                var result = await _drawingService.CreateDrawingAsync(request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.ErrorResponse($"Error uploading file: {ex.Message}"));
            }
        }

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUploadDrawings([FromForm] IFormFileCollection files, [FromForm] int? linkedProductId, [FromForm] int? linkedCustomerId)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(ApiResponse<object>.ErrorResponse("No files uploaded"));

                var results = new System.Collections.Generic.List<object>();
                var year = DateTime.Now.Year.ToString();
                var month = DateTime.Now.Month.ToString("D2");

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    // Validate file type
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".dwg" };
                    if (!Array.Exists(allowedExtensions, ext => ext == extension))
                    {
                        results.Add(new { FileName = file.FileName, Success = false, Message = "Invalid file type" });
                        continue;
                    }

                    // Validate file size
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        results.Add(new { FileName = file.FileName, Success = false, Message = "File size exceeds 10MB" });
                        continue;
                    }

                    // Generate S3 key and upload
                    var drawingNumber = $"DWG-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                    var fileName = $"{drawingNumber}_{Path.GetFileName(file.FileName)}";
                    var s3Key = $"multihitech/drawings/{year}/{month}/{fileName}";

                    using var fileStream = file.OpenReadStream();
                    var fileUrl = await _s3Service.UploadAsync(fileStream, s3Key, file.ContentType);

                    // Create drawing record with minimal information
                    var request = new CreateDrawingRequest
                    {
                        DrawingNumber = drawingNumber,
                        DrawingName = Path.GetFileNameWithoutExtension(file.FileName),
                        DrawingType = "other",
                        Status = "draft",
                        FileName = Path.GetFileName(file.FileName),
                        FileType = extension.TrimStart('.'),
                        FileUrl = fileUrl,
                        FileSize = (decimal)(file.Length / 1024.0),
                        LinkedProductId = linkedProductId,
                        LinkedCustomerId = linkedCustomerId,
                        Description = $"Bulk uploaded: {Path.GetFileName(file.FileName)}"
                    };

                    var result = await _drawingService.CreateDrawingAsync(request);
                    results.Add(new
                    {
                        FileName = file.FileName,
                        Success = result.Success,
                        DrawingNumber = drawingNumber,
                        DrawingId = result.Success ? result.Data : 0,
                        Message = result.Message
                    });
                }

                return Ok(ApiResponse<object>.SuccessResponse(results, $"Processed {files.Count} files"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse($"Error during bulk upload: {ex.Message}"));
            }
        }

        [HttpPatch("{id}/link-order")]
        public async Task<IActionResult> LinkDrawingToOrder(int id, [FromBody] dynamic body)
        {
            try
            {
                int orderId = (int)body.linkedOrderId;
                var result = await _drawingService.LinkDrawingToOrderAsync(id, orderId);
                return result.Success ? Ok(result) : NotFound(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse($"Error linking drawing: {ex.Message}"));
            }
        }
    }
}
