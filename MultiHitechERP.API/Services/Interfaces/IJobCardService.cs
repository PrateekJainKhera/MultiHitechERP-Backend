using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for JobCard operations (Planning module only)
    /// </summary>
    public interface IJobCardService
    {
        // Basic CRUD
        Task<ApiResponse<JobCardResponse>> GetByIdAsync(int id);
        Task<ApiResponse<JobCardResponse>> GetByJobCardNoAsync(string jobCardNo);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOrderIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByProcessIdAsync(int processId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<ApiResponse<int>> CreateJobCardAsync(CreateJobCardRequest request);
        Task<ApiResponse<bool>> UpdateJobCardAsync(UpdateJobCardRequest request);
        Task<ApiResponse<bool>> DeleteJobCardAsync(int id);

        // Status
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);

        // Dependency Operations
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetDependentJobCardsAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetPrerequisiteJobCardsAsync(int jobCardId);
        Task<ApiResponse<bool>> AddDependencyAsync(int dependentJobCardId, int prerequisiteJobCardId);
        Task<ApiResponse<bool>> RemoveDependencyAsync(int dependencyId);

        // Queries
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetBlockedJobCardsAsync();
    }
}
