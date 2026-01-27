using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMaterialCategoryService
    {
        Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetAllCategoriesAsync();
        Task<ApiResponse<MaterialCategoryResponse>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<MaterialCategoryResponse>> GetCategoryByCodeAsync(string categoryCode);
        Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetCategoriesByMaterialTypeAsync(string materialType);
        Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetActiveCategoriesAsync();
        Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> SearchCategoriesByNameAsync(string searchTerm);
        Task<ApiResponse<int>> CreateCategoryAsync(CreateMaterialCategoryRequest request);
        Task<ApiResponse<bool>> UpdateCategoryAsync(int id, UpdateMaterialCategoryRequest request);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}
