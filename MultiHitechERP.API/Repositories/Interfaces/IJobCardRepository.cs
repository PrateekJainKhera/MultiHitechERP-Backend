using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Planning;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for JobCard operations
    /// </summary>
    public interface IJobCardRepository
    {
        // Basic CRUD Operations
        Task<JobCard?> GetByIdAsync(int id);
        Task<JobCard?> GetByJobCardNoAsync(string jobCardNo);
        Task<IEnumerable<JobCard>> GetAllAsync();
        Task<IEnumerable<JobCard>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<JobCard>> GetByProcessIdAsync(int processId);
        Task<IEnumerable<JobCard>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<int> InsertAsync(JobCard jobCard);
        Task<bool> UpdateAsync(JobCard jobCard);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> UpdateMaterialStatusAsync(int id, string materialStatus);
        Task<bool> UpdateScheduleStatusAsync(int id, string scheduleStatus, DateTime? startDate, DateTime? endDate);

        // Assignment Operations
        Task<bool> AssignMachineAsync(int id, int machineId, string machineName);
        Task<bool> AssignOperatorAsync(int id, int operatorId, string operatorName);

        // Execution Operations
        Task<bool> StartExecutionAsync(int id, DateTime startTime);
        Task<bool> CompleteExecutionAsync(int id, DateTime endTime, int actualTimeMin);
        Task<bool> UpdateQuantitiesAsync(int id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty);

        // Dependency Operations
        Task<IEnumerable<JobCard>> GetDependentJobCardsAsync(int jobCardId);
        Task<IEnumerable<JobCard>> GetPrerequisiteJobCardsAsync(int jobCardId);
        Task<bool> HasUnresolvedDependenciesAsync(int jobCardId);

        // Queries
        Task<IEnumerable<JobCard>> GetReadyForSchedulingAsync();
        Task<IEnumerable<JobCard>> GetScheduledJobCardsAsync();
        Task<IEnumerable<JobCard>> GetInProgressJobCardsAsync();
        Task<IEnumerable<JobCard>> GetBlockedJobCardsAsync();
        Task<IEnumerable<JobCard>> GetByMachineIdAsync(int machineId);
        Task<IEnumerable<JobCard>> GetByOperatorIdAsync(int operatorId);

        // Optimistic Locking
        Task<bool> UpdateWithVersionCheckAsync(JobCard jobCard);
        Task<int> GetVersionAsync(int id);
    }
}
