using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for JobCard operations
    /// </summary>
    public interface IJobCardService
    {
        // Basic CRUD Operations
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

        // Status Operations
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);
        Task<ApiResponse<bool>> UpdateMaterialStatusAsync(int id, string materialStatus);
        Task<ApiResponse<bool>> UpdateScheduleStatusAsync(int id, string scheduleStatus, DateTime? startDate, DateTime? endDate);

        // Assignment Operations
        Task<ApiResponse<bool>> AssignMachineAsync(int id, int machineId, string machineName);
        Task<ApiResponse<bool>> AssignOperatorAsync(int id, int operatorId, string operatorName);

        // Execution Operations
        Task<ApiResponse<bool>> StartExecutionAsync(int id);
        Task<ApiResponse<bool>> CompleteExecutionAsync(int id);
        Task<ApiResponse<bool>> UpdateQuantitiesAsync(int id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty);

        // Dependency Operations
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetDependentJobCardsAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetPrerequisiteJobCardsAsync(int jobCardId);
        Task<ApiResponse<bool>> AddDependencyAsync(int dependentJobCardId, int prerequisiteJobCardId);
        Task<ApiResponse<bool>> RemoveDependencyAsync(int dependencyId);

        // Queries
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetReadyForSchedulingAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetScheduledJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetInProgressJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetBlockedJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByMachineIdAsync(int machineId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOperatorIdAsync(int operatorId);
    }
}
