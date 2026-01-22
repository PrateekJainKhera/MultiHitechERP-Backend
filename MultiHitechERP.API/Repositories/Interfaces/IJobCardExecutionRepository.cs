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
        Task<JobCardExecution?> GetByIdAsync(int id);
        Task<IEnumerable<JobCardExecution>> GetAllAsync();
        Task<IEnumerable<JobCardExecution>> GetByJobCardIdAsync(int jobCardId);
        Task<IEnumerable<JobCardExecution>> GetByMachineIdAsync(int machineId);
        Task<IEnumerable<JobCardExecution>> GetByOperatorIdAsync(int operatorId);

        // Create, Update, Delete
        Task<int> InsertAsync(JobCardExecution execution);
        Task<bool> UpdateAsync(JobCardExecution execution);
        Task<bool> DeleteAsync(int id);

        // Execution Operations
        Task<bool> StartExecutionAsync(int id, DateTime startTime);
        Task<bool> PauseExecutionAsync(int id, DateTime pausedTime);
        Task<bool> ResumeExecutionAsync(int id, DateTime resumedTime);
        Task<bool> CompleteExecutionAsync(int id, DateTime endTime, int totalTimeMin);
        Task<bool> UpdateQuantitiesAsync(int id, int? completed, int? rejected, int? inProgress);

        // Queries
        Task<IEnumerable<JobCardExecution>> GetActiveExecutionsAsync();
        Task<IEnumerable<JobCardExecution>> GetByStatusAsync(string status);
        Task<IEnumerable<JobCardExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<JobCardExecution?> GetCurrentExecutionForJobCardAsync(int jobCardId);
        Task<IEnumerable<JobCardExecution>> GetExecutionHistoryForJobCardAsync(int jobCardId);

        // Statistics
        Task<int> GetTotalExecutionTimeForJobCardAsync(int jobCardId);
        Task<int> GetTotalCompletedQuantityForJobCardAsync(int jobCardId);
    }
}
