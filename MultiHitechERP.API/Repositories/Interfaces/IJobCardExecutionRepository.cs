using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Production;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Job Card Execution operations
    /// </summary>
    public interface IJobCardExecutionRepository
    {
        // Basic CRUD Operations
        Task<JobCardExecution?> GetByIdAsync(Guid id);
        Task<IEnumerable<JobCardExecution>> GetAllAsync();
        Task<IEnumerable<JobCardExecution>> GetByJobCardIdAsync(Guid jobCardId);
        Task<IEnumerable<JobCardExecution>> GetByMachineIdAsync(Guid machineId);
        Task<IEnumerable<JobCardExecution>> GetByOperatorIdAsync(Guid operatorId);

        // Create, Update, Delete
        Task<Guid> InsertAsync(JobCardExecution execution);
        Task<bool> UpdateAsync(JobCardExecution execution);
        Task<bool> DeleteAsync(Guid id);

        // Execution Operations
        Task<bool> StartExecutionAsync(Guid id, DateTime startTime);
        Task<bool> PauseExecutionAsync(Guid id, DateTime pausedTime);
        Task<bool> ResumeExecutionAsync(Guid id, DateTime resumedTime);
        Task<bool> CompleteExecutionAsync(Guid id, DateTime endTime, int totalTimeMin);
        Task<bool> UpdateQuantitiesAsync(Guid id, int? completed, int? rejected, int? inProgress);

        // Queries
        Task<IEnumerable<JobCardExecution>> GetActiveExecutionsAsync();
        Task<IEnumerable<JobCardExecution>> GetByStatusAsync(string status);
        Task<IEnumerable<JobCardExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<JobCardExecution?> GetCurrentExecutionForJobCardAsync(Guid jobCardId);
        Task<IEnumerable<JobCardExecution>> GetExecutionHistoryForJobCardAsync(Guid jobCardId);

        // Statistics
        Task<int> GetTotalExecutionTimeForJobCardAsync(Guid jobCardId);
        Task<int> GetTotalCompletedQuantityForJobCardAsync(Guid jobCardId);
    }
}
