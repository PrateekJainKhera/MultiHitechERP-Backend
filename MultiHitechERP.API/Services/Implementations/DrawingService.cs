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

        public async Task<ApiResponse<IEnumerable<DrawingResponse>>> GetDrawingsByOrderIdAsync(int orderId)
        {
            try
            {
                var drawings = await _drawingRepository.GetByOrderIdAsync(orderId);
                return ApiResponse<IEnumerable<DrawingResponse>>.SuccessResponse(drawings.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<DrawingResponse>>.ErrorResponse($"Error retrieving drawings for order: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateDrawingAsync(CreateDrawingRequest request)
        {
            try
            {
                // Use provided drawing number or auto-generate if not provided
                var drawingNumber = !string.IsNullOrWhiteSpace(request.DrawingNumber)
                    ? request.DrawingNumber.Trim()
                    : await _drawingRepository.GetNextDrawingNumberAsync();

                // Check if drawing number already exists
                if (await _drawingRepository.ExistsAsync(drawingNumber))
                    return ApiResponse<int>.ErrorResponse($"Drawing number '{drawingNumber}' already exists");

                var drawing = new Drawing
                {
                    DrawingNumber = drawingNumber,
                    DrawingName = request.DrawingName.Trim(),
                    DrawingType = request.DrawingType.Trim(),
                    Revision = request.Revision?.Trim(),
                    RevisionDate = request.RevisionDate,
                    Status = request.Status?.Trim() ?? "draft",
                    FileName = request.FileName?.Trim(),
                    FileType = request.FileType?.Trim(),
                    FileUrl = request.FileUrl?.Trim(),
                    FileSize = request.FileSize,
                    ManufacturingDimensionsJSON = request.ManufacturingDimensionsJSON?.Trim(),
                    LinkedPartId = request.LinkedPartId,
                    LinkedProductId = request.LinkedProductId,
                    LinkedCustomerId = request.LinkedCustomerId,
                    LinkedOrderId = request.LinkedOrderId,
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

                // Check if drawing number is being changed and if new number already exists
                if (existingDrawing.DrawingNumber != request.DrawingNumber.Trim())
                {
                    if (await _drawingRepository.ExistsAsync(request.DrawingNumber.Trim()))
                        return ApiResponse<bool>.ErrorResponse($"Drawing number '{request.DrawingNumber}' already exists");
                }

                existingDrawing.DrawingNumber = request.DrawingNumber.Trim();
                existingDrawing.DrawingName = request.DrawingName.Trim();
                existingDrawing.DrawingType = request.DrawingType.Trim();
                existingDrawing.Revision = request.Revision?.Trim();
                existingDrawing.RevisionDate = request.RevisionDate;
                existingDrawing.Status = request.Status.Trim();
                existingDrawing.FileName = request.FileName?.Trim();
                existingDrawing.FileType = request.FileType?.Trim();
                existingDrawing.FileUrl = request.FileUrl?.Trim();
                existingDrawing.FileSize = request.FileSize;
                existingDrawing.ManufacturingDimensionsJSON = request.ManufacturingDimensionsJSON?.Trim();
                existingDrawing.LinkedPartId = request.LinkedPartId;
                existingDrawing.LinkedProductId = request.LinkedProductId;
                existingDrawing.LinkedCustomerId = request.LinkedCustomerId;
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
                Revision = drawing.Revision,
                RevisionDate = drawing.RevisionDate,
                Status = drawing.Status,
                FileName = drawing.FileName,
                FileType = drawing.FileType,
                FileUrl = drawing.FileUrl,
                FileSize = drawing.FileSize,
                ManufacturingDimensionsJSON = drawing.ManufacturingDimensionsJSON,
                LinkedPartId = drawing.LinkedPartId,
                LinkedProductId = drawing.LinkedProductId,
                LinkedCustomerId = drawing.LinkedCustomerId,
                LinkedOrderId = drawing.LinkedOrderId,
                Description = drawing.Description,
                Notes = drawing.Notes,
                IsActive = drawing.IsActive,
                CreatedAt = drawing.CreatedAt,
                CreatedBy = drawing.CreatedBy,
                UpdatedAt = drawing.UpdatedAt,
                UpdatedBy = drawing.UpdatedBy,
                ApprovedBy = drawing.ApprovedBy,
                ApprovedAt = drawing.ApprovedAt
            };
        }

        public async Task<ApiResponse<bool>> LinkDrawingToOrderAsync(int drawingId, int orderId)
        {
            try
            {
                var drawing = await _drawingRepository.GetByIdAsync(drawingId);
                if (drawing == null)
                    return ApiResponse<bool>.ErrorResponse("Drawing not found");

                drawing.LinkedOrderId = orderId;
                await _drawingRepository.UpdateAsync(drawing);
                return ApiResponse<bool>.SuccessResponse(true, "Drawing linked to order");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error linking drawing to order: {ex.Message}");
            }
        }
    }
}
