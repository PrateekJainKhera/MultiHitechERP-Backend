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
        Task<ApiResponse<JobCardResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<JobCardResponse>> GetByJobCardNoAsync(string jobCardNo);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOrderIdAsync(Guid orderId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByProcessIdAsync(Guid processId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<ApiResponse<Guid>> CreateJobCardAsync(CreateJobCardRequest request);
        Task<ApiResponse<bool>> UpdateJobCardAsync(UpdateJobCardRequest request);
        Task<ApiResponse<bool>> DeleteJobCardAsync(Guid id);

        // Status Operations
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);
        Task<ApiResponse<bool>> UpdateMaterialStatusAsync(Guid id, string materialStatus);
        Task<ApiResponse<bool>> UpdateScheduleStatusAsync(Guid id, string scheduleStatus, DateTime? startDate, DateTime? endDate);

        // Assignment Operations
        Task<ApiResponse<bool>> AssignMachineAsync(Guid id, Guid machineId, string machineName);
        Task<ApiResponse<bool>> AssignOperatorAsync(Guid id, Guid operatorId, string operatorName);

        // Execution Operations
        Task<ApiResponse<bool>> StartExecutionAsync(Guid id);
        Task<ApiResponse<bool>> CompleteExecutionAsync(Guid id);
        Task<ApiResponse<bool>> UpdateQuantitiesAsync(Guid id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty);

        // Dependency Operations
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetDependentJobCardsAsync(Guid jobCardId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetPrerequisiteJobCardsAsync(Guid jobCardId);
        Task<ApiResponse<bool>> AddDependencyAsync(Guid dependentJobCardId, Guid prerequisiteJobCardId);
        Task<ApiResponse<bool>> RemoveDependencyAsync(Guid dependencyId);

        // Queries
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetReadyForSchedulingAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetScheduledJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetInProgressJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetBlockedJobCardsAsync();
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByMachineIdAsync(Guid machineId);
        Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOperatorIdAsync(Guid operatorId);
    }
}
