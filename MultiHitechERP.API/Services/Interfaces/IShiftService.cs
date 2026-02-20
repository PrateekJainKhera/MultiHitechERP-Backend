using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IShiftService
    {
        Task<ApiResponse<IEnumerable<ShiftResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ShiftResponse>>> GetActiveAsync();
        Task<ApiResponse<ShiftResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreateShiftRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, CreateShiftRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
