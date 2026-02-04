using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IDrawingService
    {
        Task<ApiResponse<DrawingResponse>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<DrawingResponse>>> GetAllAsync();
        Task<ApiResponse<int>> CreateDrawingAsync(CreateDrawingRequest request);
        Task<ApiResponse<bool>> UpdateDrawingAsync(UpdateDrawingRequest request);
        Task<ApiResponse<bool>> DeleteDrawingAsync(int id);
    }
}
