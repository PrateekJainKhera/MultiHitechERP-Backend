using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Planning;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for JobCard operations (Planning module only)
    /// </summary>
    public interface IJobCardRepository
    {
        // Basic CRUD
        Task<JobCard?> GetByIdAsync(int id);
        Task<JobCard?> GetByJobCardNoAsync(string jobCardNo);
        Task<IEnumerable<JobCard>> GetAllAsync();
        Task<IEnumerable<JobCard>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<JobCard>> GetByProcessIdAsync(int processId);
        Task<IEnumerable<JobCard>> GetByStatusAsync(string status);

        Task<int> InsertAsync(JobCard jobCard);
        Task<bool> UpdateAsync(JobCard jobCard);
        Task<bool> DeleteAsync(int id);

        // Status
        Task<bool> UpdateStatusAsync(int id, string status);

        // Dependency Operations
        Task<IEnumerable<JobCard>> GetDependentJobCardsAsync(int jobCardId);
        Task<IEnumerable<JobCard>> GetPrerequisiteJobCardsAsync(int jobCardId);
        Task<bool> HasUnresolvedDependenciesAsync(int jobCardId);

        // Queries
        Task<IEnumerable<JobCard>> GetBlockedJobCardsAsync();

        // Optimistic Locking
        Task<bool> UpdateWithVersionCheckAsync(JobCard jobCard);
        Task<int> GetVersionAsync(int id);

        // Production Execution
        Task<bool> UpdateProductionStatusAsync(int id, string productionStatus, DateTime? actualStartTime, DateTime? actualEndTime, int completedQty, int rejectedQty);
        Task<bool> SetReadyForAssemblyAsync(int id, bool ready);
        Task<IEnumerable<JobCard>> GetByProductionStatusAsync(int orderId, string productionStatus);
    }
}
