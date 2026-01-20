using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class DrawingService : IDrawingService
    {
        private readonly IDrawingRepository _drawingRepository;

        public DrawingService(IDrawingRepository drawingRepository)
        {
            _drawingRepository = drawingRepository;
        }

        public async Task<ApiResponse<DrawingResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var drawing = await _drawingRepository.GetByIdAsync(id);
                if (drawing == null)
                    return ApiResponse<DrawingResponse>.ErrorResponse($"Drawing with ID {id} not found");

                return ApiResponse<DrawingResponse>.SuccessResponse(MapToResponse(drawing));
            }
            catch (Exception ex)
            {
                return ApiResponse<DrawingResponse>.ErrorResponse($"Error retrieving drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DrawingResponse>> GetByDrawingNumberAsync(string drawingNumber)
        {
            try
            {
                var drawing = await _drawingRepository.GetByDrawingNumberAsync(drawingNumber);
                if (drawing == null)
                    return ApiResponse<DrawingResponse>.ErrorResponse($"Drawing {drawingNumber} not found");

                return ApiResponse<DrawingResponse>.SuccessResponse(MapToResponse(drawing));
            }
            catch (Exception ex)
            {
                return ApiResponse<DrawingResponse>.ErrorResponse($"Error retrieving drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetAllAsync()
        {
            try
            {
                var drawings = await _drawingRepository.GetAllAsync();
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving drawings: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetActiveDrawingsAsync()
        {
            try
            {
                var drawings = await _drawingRepository.GetActiveDrawingsAsync();
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving active drawings: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetRevisionHistoryAsync(string drawingNumber)
        {
            try
            {
                var drawings = await _drawingRepository.GetRevisionHistoryAsync(drawingNumber);
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving revision history: {ex.Message}");
            }
        }

        public async Task<ApiResponse<DrawingResponse>> GetLatestRevisionAsync(string drawingNumber)
        {
            try
            {
                var drawing = await _drawingRepository.GetLatestRevisionAsync(drawingNumber);
                if (drawing == null)
                    return ApiResponse<DrawingResponse>.ErrorResponse($"No revisions found for drawing {drawingNumber}");

                return ApiResponse<DrawingResponse>.SuccessResponse(MapToResponse(drawing));
            }
            catch (Exception ex)
            {
                return ApiResponse<DrawingResponse>.ErrorResponse($"Error retrieving latest revision: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetByProductIdAsync(Guid productId)
        {
            try
            {
                var drawings = await _drawingRepository.GetByProductIdAsync(productId);
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving drawings: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetByDrawingTypeAsync(string drawingType)
        {
            try
            {
                var drawings = await _drawingRepository.GetByDrawingTypeAsync(drawingType);
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving drawings: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetPendingApprovalAsync()
        {
            try
            {
                var drawings = await _drawingRepository.GetPendingApprovalAsync();
                var responses = drawings.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving pending approvals: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Guid>> CreateDrawingAsync(CreateDrawingRequest request)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(request.DrawingNumber))
                    return ApiResponse<Guid>.ErrorResponse("Drawing number is required");

                if (string.IsNullOrWhiteSpace(request.DrawingTitle))
                    return ApiResponse<Guid>.ErrorResponse("Drawing title is required");

                var revisionNumber = request.RevisionNumber ?? "A";
                var exists = await _drawingRepository.ExistsAsync(request.DrawingNumber, revisionNumber);
                if (exists)
                    return ApiResponse<Guid>.ErrorResponse($"Drawing {request.DrawingNumber} revision {revisionNumber} already exists");

                // Map to entity
                var drawing = new Drawing
                {
                    DrawingNumber = request.DrawingNumber.Trim().ToUpper(),
                    DrawingTitle = request.DrawingTitle.Trim(),
                    ProductId = request.ProductId,
                    ProductCode = request.ProductCode?.Trim(),
                    ProductName = request.ProductName?.Trim(),
                    RevisionNumber = revisionNumber,
                    RevisionDate = request.RevisionDate ?? DateTime.UtcNow,
                    RevisionDescription = request.RevisionDescription?.Trim(),
                    DrawingType = request.DrawingType?.Trim(),
                    Category = request.Category?.Trim(),
                    FilePath = request.FilePath?.Trim(),
                    FileName = request.FileName?.Trim(),
                    FileFormat = request.FileFormat?.Trim(),
                    FileSize = request.FileSize,
                    PreparedBy = request.PreparedBy?.Trim(),
                    CheckedBy = request.CheckedBy?.Trim(),
                    ApprovedBy = request.ApprovedBy?.Trim(),
                    ApprovalDate = request.ApprovalDate,
                    MaterialSpecification = request.MaterialSpecification?.Trim(),
                    Finish = request.Finish?.Trim(),
                    ToleranceGrade = request.ToleranceGrade?.Trim(),
                    TreatmentRequired = request.TreatmentRequired?.Trim(),
                    OverallLength = request.OverallLength,
                    OverallWidth = request.OverallWidth,
                    OverallHeight = request.OverallHeight,
                    Weight = request.Weight,
                    IsActive = true,
                    Status = "Active",
                    IsLatestRevision = true,
                    PreviousRevisionId = request.PreviousRevisionId,
                    VersionNumber = request.VersionNumber,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var drawingId = await _drawingRepository.InsertAsync(drawing);
                return ApiResponse<Guid>.SuccessResponse(drawingId, $"Drawing '{request.DrawingNumber}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrawingAsync(UpdateDrawingRequest request)
        {
            try
            {
                var existingDrawing = await _drawingRepository.GetByIdAsync(request.Id);
                if (existingDrawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                // Update fields
                existingDrawing.DrawingNumber = request.DrawingNumber.Trim().ToUpper();
                existingDrawing.DrawingTitle = request.DrawingTitle.Trim();
                existingDrawing.ProductId = request.ProductId;
                existingDrawing.ProductCode = request.ProductCode?.Trim();
                existingDrawing.ProductName = request.ProductName?.Trim();
                existingDrawing.RevisionNumber = request.RevisionNumber?.Trim();
                existingDrawing.RevisionDate = request.RevisionDate;
                existingDrawing.RevisionDescription = request.RevisionDescription?.Trim();
                existingDrawing.DrawingType = request.DrawingType?.Trim();
                existingDrawing.Category = request.Category?.Trim();
                existingDrawing.FilePath = request.FilePath?.Trim();
                existingDrawing.FileName = request.FileName?.Trim();
                existingDrawing.FileFormat = request.FileFormat?.Trim();
                existingDrawing.FileSize = request.FileSize;
                existingDrawing.PreparedBy = request.PreparedBy?.Trim();
                existingDrawing.CheckedBy = request.CheckedBy?.Trim();
                existingDrawing.ApprovedBy = request.ApprovedBy?.Trim();
                existingDrawing.ApprovalDate = request.ApprovalDate;
                existingDrawing.MaterialSpecification = request.MaterialSpecification?.Trim();
                existingDrawing.Finish = request.Finish?.Trim();
                existingDrawing.ToleranceGrade = request.ToleranceGrade?.Trim();
                existingDrawing.TreatmentRequired = request.TreatmentRequired?.Trim();
                existingDrawing.OverallLength = request.OverallLength;
                existingDrawing.OverallWidth = request.OverallWidth;
                existingDrawing.OverallHeight = request.OverallHeight;
                existingDrawing.Weight = request.Weight;
                existingDrawing.IsActive = request.IsActive;
                existingDrawing.Status = request.Status;
                existingDrawing.IsLatestRevision = request.IsLatestRevision;
                existingDrawing.PreviousRevisionId = request.PreviousRevisionId;
                existingDrawing.VersionNumber = request.VersionNumber;
                existingDrawing.Remarks = request.Remarks?.Trim();
                existingDrawing.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingDrawing.UpdatedAt = DateTime.UtcNow;

                var success = await _drawingRepository.UpdateAsync(existingDrawing);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update drawing");

                return ApiResponse<bool>.SuccessResponse(true, "Drawing updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteDrawingAsync(Guid id)
        {
            try
            {
                var drawing = await _drawingRepository.GetByIdAsync(id);
                if (drawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                var success = await _drawingRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete drawing");

                return ApiResponse<bool>.SuccessResponse(true, "Drawing deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> MarkAsLatestRevisionAsync(Guid id)
        {
            try
            {
                var drawing = await _drawingRepository.GetByIdAsync(id);
                if (drawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                var success = await _drawingRepository.MarkAsLatestRevisionAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to mark drawing as latest revision");

                return ApiResponse<bool>.SuccessResponse(true, $"Drawing {drawing.DrawingNumber} marked as latest revision");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error marking as latest revision: {ex.Message}");
            }
        }

        private static DrawingResponse MapToResponse(Drawing drawing)
        {
            return new DrawingResponse
            {
                Id = drawing.Id,
                DrawingNumber = drawing.DrawingNumber,
                DrawingTitle = drawing.DrawingTitle,
                ProductId = drawing.ProductId,
                ProductCode = drawing.ProductCode,
                ProductName = drawing.ProductName,
                RevisionNumber = drawing.RevisionNumber,
                RevisionDate = drawing.RevisionDate,
                RevisionDescription = drawing.RevisionDescription,
                DrawingType = drawing.DrawingType,
                Category = drawing.Category,
                FilePath = drawing.FilePath,
                FileName = drawing.FileName,
                FileFormat = drawing.FileFormat,
                FileSize = drawing.FileSize,
                PreparedBy = drawing.PreparedBy,
                CheckedBy = drawing.CheckedBy,
                ApprovedBy = drawing.ApprovedBy,
                ApprovalDate = drawing.ApprovalDate,
                MaterialSpecification = drawing.MaterialSpecification,
                Finish = drawing.Finish,
                ToleranceGrade = drawing.ToleranceGrade,
                TreatmentRequired = drawing.TreatmentRequired,
                OverallLength = drawing.OverallLength,
                OverallWidth = drawing.OverallWidth,
                OverallHeight = drawing.OverallHeight,
                Weight = drawing.Weight,
                IsActive = drawing.IsActive,
                Status = drawing.Status,
                IsLatestRevision = drawing.IsLatestRevision,
                PreviousRevisionId = drawing.PreviousRevisionId,
                VersionNumber = drawing.VersionNumber,
                Remarks = drawing.Remarks,
                CreatedAt = drawing.CreatedAt,
                CreatedBy = drawing.CreatedBy,
                UpdatedAt = drawing.UpdatedAt,
                UpdatedBy = drawing.UpdatedBy
            };
        }
    }
}
