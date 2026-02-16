using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProcessCategoryService
    {
        Task<ApiResponse<IEnumerable<ProcessCategoryResponse>>> GetAllAsync();
        Task<ApiResponse<ProcessCategoryResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreateProcessCategoryRequest request);
        Task<ApiResponse<bool>> UpdateAsync(UpdateProcessCategoryRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
