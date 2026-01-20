using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Drawing operations
    /// </summary>
    public interface IDrawingService
    {
        // Basic CRUD Operations
        Task<ApiResponse<DrawingResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<DrawingResponse>> GetByDrawingNumberAsync(string drawingNumber);
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetActiveDrawingsAsync();

        // Create, Update, Delete
        Task<ApiResponse<Guid>> CreateDrawingAsync(CreateDrawingRequest request);
        Task<ApiResponse<bool>> UpdateDrawingAsync(UpdateDrawingRequest request);
        Task<ApiResponse<bool>> DeleteDrawingAsync(Guid id);

        // Revision Operations
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetRevisionHistoryAsync(string drawingNumber);
        Task<ApiResponse<DrawingResponse>> GetLatestRevisionAsync(string drawingNumber);
        Task<ApiResponse<bool>> MarkAsLatestRevisionAsync(Guid id);

        // Queries
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetByProductIdAsync(Guid productId);
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetByDrawingTypeAsync(string drawingType);
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetPendingApprovalAsync();
    }
}
