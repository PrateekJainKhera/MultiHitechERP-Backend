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
        Task<ApiResponse<ProductResponse>> GetByPartCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetActiveProductsAsync();

        Task<ApiResponse<int>> CreateProductAsync(CreateProductRequest request);
        Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductRequest request);
        Task<ApiResponse<bool>> DeleteProductAsync(int id);

        Task<ApiResponse<bool>> ActivateProductAsync(int id);
        Task<ApiResponse<bool>> DeactivateProductAsync(int id);

        Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByNameAsync(string searchTerm);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetByProductTypeAsync(string productType);
    }
}
