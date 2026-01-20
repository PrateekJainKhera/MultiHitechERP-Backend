using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<ProductResponse>> GetByProductCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetActiveProductsAsync();

        Task<ApiResponse<Guid>> CreateProductAsync(CreateProductRequest request);
        Task<ApiResponse<bool>> UpdateProductAsync(UpdateProductRequest request);
        Task<ApiResponse<bool>> DeleteProductAsync(Guid id);

        Task<ApiResponse<bool>> ActivateProductAsync(Guid id);
        Task<ApiResponse<bool>> DeactivateProductAsync(Guid id);

        Task<ApiResponse<IEnumerable<ProductResponse>>> SearchByNameAsync(string searchTerm);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetByProductTypeAsync(string productType);
    }
}
