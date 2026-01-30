using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Process Template business logic
    /// </summary>
    public interface IProcessTemplateService
    {
        // Template CRUD Operations
        Task<ApiResponse<ProcessTemplateResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ProcessTemplateResponse>> GetByNameAsync(string templateName);
        Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetActiveTemplatesAsync();

        Task<ApiResponse<int>> CreateTemplateAsync(CreateProcessTemplateRequest request);
        Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateProcessTemplateRequest request);
        Task<ApiResponse<bool>> DeleteTemplateAsync(int id);

        // Template with Steps (composite operations)
        Task<ApiResponse<ProcessTemplateWithStepsResponse>> GetTemplateWithStepsAsync(int templateId);
        Task<ApiResponse<int>> CreateTemplateWithStepsAsync(CreateProcessTemplateWithStepsRequest request);

        // Template Steps Operations
        Task<ApiResponse<IEnumerable<ProcessTemplateStepResponse>>> GetStepsByTemplateIdAsync(int templateId);
        Task<ApiResponse<int>> AddStepToTemplateAsync(CreateProcessTemplateStepRequest request);
        Task<ApiResponse<bool>> UpdateStepAsync(UpdateProcessTemplateStepRequest request);
        Task<ApiResponse<bool>> DeleteStepAsync(int stepId);

        // Business Queries
        Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetByApplicableTypeAsync(string applicableType);
    }
}
