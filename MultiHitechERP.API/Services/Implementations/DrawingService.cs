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

        public async Task<ApiResponse<DrawingResponse>> GetByIdAsync(int id)
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

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetAllAsync()
        {
            try
            {
                var drawings = await _drawingRepository.GetAllAsync();
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(drawings.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving drawings: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateDrawingAsync(CreateDrawingRequest request)
        {
            try
            {
                var drawingNumber = await _drawingRepository.GetNextDrawingNumberAsync();

                var drawing = new Drawing
                {
                    DrawingNumber = drawingNumber,
                    DrawingName = request.DrawingName.Trim(),
                    DrawingType = request.DrawingType.Trim(),
                    RevisionNumber = request.RevisionNumber?.Trim(),
                    Status = request.Status?.Trim() ?? "Draft",
                    Description = request.Description?.Trim(),
                    Notes = request.Notes?.Trim(),
                    IsActive = true,
                    CreatedBy = "System"
                };

                var drawingId = await _drawingRepository.InsertAsync(drawing);
                return ApiResponse<int>.SuccessResponse(drawingId, $"Drawing '{drawing.DrawingNumber}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating drawing: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrawingAsync(UpdateDrawingRequest request)
        {
            try
            {
                var existingDrawing = await _drawingRepository.GetByIdAsync(request.Id);
                if (existingDrawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                existingDrawing.DrawingName = request.DrawingName.Trim();
                existingDrawing.DrawingType = request.DrawingType.Trim();
                existingDrawing.RevisionNumber = request.RevisionNumber?.Trim();
                existingDrawing.Status = request.Status.Trim();
                existingDrawing.Description = request.Description?.Trim();
                existingDrawing.Notes = request.Notes?.Trim();
                existingDrawing.IsActive = request.IsActive;
                existingDrawing.UpdatedBy = "System";

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

        public async Task<ApiResponse<bool>> DeleteDrawingAsync(int id)
        {
            try
            {
                var existingDrawing = await _drawingRepository.GetByIdAsync(id);
                if (existingDrawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                var success = await _drawingRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete drawing");

                return ApiResponse<bool>.SuccessResponse(true, $"Drawing '{existingDrawing.DrawingNumber}' deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting drawing: {ex.Message}");
            }
        }

        private DrawingResponse MapToResponse(Drawing drawing)
        {
            return new DrawingResponse
            {
                Id = drawing.Id,
                DrawingNumber = drawing.DrawingNumber,
                DrawingName = drawing.DrawingName,
                DrawingType = drawing.DrawingType,
                RevisionNumber = drawing.RevisionNumber,
                Status = drawing.Status,
                Description = drawing.Description,
                Notes = drawing.Notes,
                IsActive = drawing.IsActive,
                CreatedAt = drawing.CreatedAt,
                CreatedBy = drawing.CreatedBy,
                UpdatedAt = drawing.UpdatedAt,
                UpdatedBy = drawing.UpdatedBy
            };
        }
    }
}
