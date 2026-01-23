using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IChildPartTemplateService
    {
        Task<ApiResponse<ChildPartTemplateResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ChildPartTemplateResponse>> GetByNameAsync(string templateName);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetActiveTemplatesAsync();
        Task<ApiResponse<int>> CreateTemplateAsync(CreateChildPartTemplateRequest request);
        Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateChildPartTemplateRequest request);
        Task<ApiResponse<bool>> DeleteTemplateAsync(int id);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByChildPartTypeAsync(string childPartType);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByProcessTemplateIdAsync(int processTemplateId);
        Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetDefaultTemplatesAsync();
        Task<ApiResponse<bool>> ApproveTemplateAsync(int id, string approvedBy);
    }
}
