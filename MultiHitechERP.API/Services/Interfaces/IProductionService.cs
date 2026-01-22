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
        Task<ApiResponse<JobCardExecution>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByJobCardIdAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByMachineIdAsync(int machineId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByOperatorIdAsync(int operatorId);

        // Create, Update, Delete
        Task<ApiResponse<int>> CreateExecutionAsync(JobCardExecution execution);
        Task<ApiResponse<bool>> UpdateExecutionAsync(JobCardExecution execution);
        Task<ApiResponse<bool>> DeleteExecutionAsync(int id);

        // Execution Operations
        Task<ApiResponse<int>> StartProductionAsync(int jobCardId, int machineId, int operatorId, int quantityStarted);
        Task<ApiResponse<bool>> PauseProductionAsync(int executionId);
        Task<ApiResponse<bool>> ResumeProductionAsync(int executionId);
        Task<ApiResponse<bool>> CompleteProductionAsync(int executionId, int quantityCompleted, int? quantityRejected);
        Task<ApiResponse<bool>> UpdateQuantitiesAsync(int executionId, int? completed, int? rejected, int? inProgress);

        // Queries
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetActiveExecutionsAsync();
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<JobCardExecution>> GetCurrentExecutionForJobCardAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<JobCardExecution>>> GetExecutionHistoryForJobCardAsync(int jobCardId);

        // Statistics
        Task<ApiResponse<int>> GetTotalExecutionTimeForJobCardAsync(int jobCardId);
        Task<ApiResponse<int>> GetTotalCompletedQuantityForJobCardAsync(int jobCardId);
    }
}
