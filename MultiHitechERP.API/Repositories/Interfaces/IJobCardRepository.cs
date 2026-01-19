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
        Task<JobCard?> GetByIdAsync(Guid id);
        Task<JobCard?> GetByJobCardNoAsync(string jobCardNo);
        Task<IEnumerable<JobCard>> GetAllAsync();
        Task<IEnumerable<JobCard>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<JobCard>> GetByProcessIdAsync(Guid processId);
        Task<IEnumerable<JobCard>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<Guid> InsertAsync(JobCard jobCard);
        Task<bool> UpdateAsync(JobCard jobCard);
        Task<bool> DeleteAsync(Guid id);

        // Status Operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> UpdateMaterialStatusAsync(Guid id, string materialStatus);
        Task<bool> UpdateScheduleStatusAsync(Guid id, string scheduleStatus, DateTime? startDate, DateTime? endDate);

        // Assignment Operations
        Task<bool> AssignMachineAsync(Guid id, Guid machineId, string machineName);
        Task<bool> AssignOperatorAsync(Guid id, Guid operatorId, string operatorName);

        // Execution Operations
        Task<bool> StartExecutionAsync(Guid id, DateTime startTime);
        Task<bool> CompleteExecutionAsync(Guid id, DateTime endTime, int actualTimeMin);
        Task<bool> UpdateQuantitiesAsync(Guid id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty);

        // Dependency Operations
        Task<IEnumerable<JobCard>> GetDependentJobCardsAsync(Guid jobCardId);
        Task<IEnumerable<JobCard>> GetPrerequisiteJobCardsAsync(Guid jobCardId);
        Task<bool> HasUnresolvedDependenciesAsync(Guid jobCardId);

        // Queries
        Task<IEnumerable<JobCard>> GetReadyForSchedulingAsync();
        Task<IEnumerable<JobCard>> GetScheduledJobCardsAsync();
        Task<IEnumerable<JobCard>> GetInProgressJobCardsAsync();
        Task<IEnumerable<JobCard>> GetBlockedJobCardsAsync();
        Task<IEnumerable<JobCard>> GetByMachineIdAsync(Guid machineId);
        Task<IEnumerable<JobCard>> GetByOperatorIdAsync(Guid operatorId);

        // Optimistic Locking
        Task<bool> UpdateWithVersionCheckAsync(JobCard jobCard);
        Task<int> GetVersionAsync(Guid id);
    }
}
