using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Child Part Template business logic
    /// </summary>
    public interface IChildPartTemplateService
    {
        // CRUD Operations
        Task<ApiResponse<ChildPartTemplateResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ChildPartTemplateResponse>> GetByCodeAsync(string templateCode);
        Task<ApiResponse<ChildPartTemplateResponse>> GetByNameAsync(string templateName);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetActiveTemplatesAsync();

        Task<ApiResponse<int>> CreateTemplateAsync(CreateChildPartTemplateRequest request);
        Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateChildPartTemplateRequest request);
        Task<ApiResponse<bool>> DeleteTemplateAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByChildPartTypeAsync(string childPartType);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByRollerTypeAsync(string rollerType);
    }
}
