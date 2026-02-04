using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _environment;

        public DrawingsController(IDrawingService drawingService, IWebHostEnvironment environment)
        {
            _drawingService = drawingService;
            _environment = environment;
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

                // Generate file path
                var year = DateTime.Now.Year.ToString();
                var month = DateTime.Now.Month.ToString("D2");
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "drawings", year, month);

                // Create directory if it doesn't exist
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var drawingNumber = !string.IsNullOrWhiteSpace(request.DrawingNumber)
                    ? request.DrawingNumber.Trim()
                    : $"DWG-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                var fileName = $"{drawingNumber}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update request with file information
                request.DrawingNumber = drawingNumber;
                request.FileName = Path.GetFileName(file.FileName);
                request.FileType = extension.TrimStart('.');
                request.FileUrl = $"/uploads/drawings/{year}/{month}/{fileName}";
                request.FileSize = (decimal)(file.Length / 1024.0); // Convert to KB

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
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "drawings", year, month);

                Directory.CreateDirectory(uploadsFolder);

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

                    // Generate unique filename
                    var drawingNumber = $"DWG-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                    var fileName = $"{drawingNumber}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Create drawing record with minimal information
                    var request = new CreateDrawingRequest
                    {
                        DrawingNumber = drawingNumber,
                        DrawingName = Path.GetFileNameWithoutExtension(file.FileName),
                        DrawingType = "other", // Default, can be updated later
                        Status = "draft",
                        FileName = Path.GetFileName(file.FileName),
                        FileType = extension.TrimStart('.'),
                        FileUrl = $"/uploads/drawings/{year}/{month}/{fileName}",
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
    }
}
