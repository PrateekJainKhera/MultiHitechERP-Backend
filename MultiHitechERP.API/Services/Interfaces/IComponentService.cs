using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IComponentService
    {
        Task<ApiResponse<IEnumerable<ComponentResponse>>> GetAllComponentsAsync();
        Task<ApiResponse<ComponentResponse>> GetComponentByIdAsync(int id);
        Task<ApiResponse<ComponentResponse>> GetComponentByPartNumberAsync(string partNumber);
        Task<ApiResponse<IEnumerable<ComponentResponse>>> GetComponentsByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ComponentResponse>>> SearchComponentsByNameAsync(string searchTerm);
        Task<ApiResponse<int>> CreateComponentAsync(CreateComponentRequest request);
        Task<ApiResponse<bool>> UpdateComponentAsync(int id, UpdateComponentRequest request);
        Task<ApiResponse<bool>> DeleteComponentAsync(int id);
        Task<ApiResponse<IEnumerable<ComponentLowStockResponse>>> GetLowStockComponentsAsync();
    }
}
