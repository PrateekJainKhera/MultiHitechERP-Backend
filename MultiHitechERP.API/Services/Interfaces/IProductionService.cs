using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Production execution business logic
    /// Manages job card execution with machine and operator availability validation
    /// </summary>
    public interface IProductionService
    {
        // Basic CRUD Operations
        Task<ApiResponse<JobCardExecution>> GetByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByJobCardIdAsync(Guid jobCardId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByMachineIdAsync(Guid machineId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByOperatorIdAsync(Guid operatorId);

        // Create, Update, Delete
        Task<ApiResponse<Guid>> CreateExecutionAsync(JobCardExecution execution);
        Task<ApiResponse<bool>> UpdateExecutionAsync(JobCardExecution execution);
        Task<ApiResponse<bool>> DeleteExecutionAsync(Guid id);

        // Execution Operations
        Task<ApiResponse<Guid>> StartProductionAsync(Guid jobCardId, Guid machineId, Guid operatorId, int quantityStarted);
        Task<ApiResponse<bool>> PauseProductionAsync(Guid executionId);
        Task<ApiResponse<bool>> ResumeProductionAsync(Guid executionId);
        Task<ApiResponse<bool>> CompleteProductionAsync(Guid executionId, int quantityCompleted, int? quantityRejected);
        Task<ApiResponse<bool>> UpdateQuantitiesAsync(Guid executionId, int? completed, int? rejected, int? inProgress);

        // Queries
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetActiveExecutionsAsync();
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<JobCardExecution>> GetCurrentExecutionForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetExecutionHistoryForJobCardAsync(Guid jobCardId);

        // Statistics
        Task<ApiResponse<int>> GetTotalExecutionTimeForJobCardAsync(Guid jobCardId);
        Task<ApiResponse<int>> GetTotalCompletedQuantityForJobCardAsync(Guid jobCardId);
    }
}
