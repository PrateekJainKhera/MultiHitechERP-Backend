using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Product Template business logic
    /// </summary>
    public interface IProductTemplateService
    {
        // CRUD Operations
        Task<ApiResponse<ProductTemplateResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ProductTemplateResponse>> GetByCodeAsync(string templateCode);
        Task<ApiResponse<ProductTemplateResponse>> GetByNameAsync(string templateName);
        Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetActiveTemplatesAsync();

        Task<ApiResponse<int>> CreateTemplateAsync(CreateProductTemplateRequest request);
        Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateProductTemplateRequest request);
        Task<ApiResponse<bool>> DeleteTemplateAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByRollerTypeAsync(string rollerType);
        Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByProcessTemplateIdAsync(int processTemplateId);
    }
}
