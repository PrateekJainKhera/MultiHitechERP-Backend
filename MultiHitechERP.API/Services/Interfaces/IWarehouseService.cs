using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IWarehouseService
    {
        Task<ApiResponse<IEnumerable<WarehouseResponse>>> GetAllAsync();
        Task<ApiResponse<WarehouseResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreateWarehouseRequest request);
        Task<ApiResponse<bool>> UpdateAsync(UpdateWarehouseRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<LowStockAlertResponse>>> GetLowStockStatusAsync();
    }
}
