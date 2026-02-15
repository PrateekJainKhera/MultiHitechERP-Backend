using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ProductResponse>> GetByPartCodeAsync(string partCode);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync();

        Task<ApiResponse<int>> CreateProductAsync(CreateProductRequest request);
        Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductRequest request);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);

        Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByNameAsync(string searchTerm);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetByRollerTypeAsync(string rollerType);
        Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByCriteriaAsync(int modelId, string rollerType, int numberOfTeeth);

        Task<ApiResponse<bool>> UpdateDrawingReviewStatusAsync(int productId, string status, string? notes, string reviewedBy);
        Task<ApiResponse<bool>> RequestDrawingAsync(int productId, string requestedBy);
    }
}
