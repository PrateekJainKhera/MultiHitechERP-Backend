using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Planning;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for JobCardDependency operations
    /// </summary>
    public interface IJobCardDependencyRepository
    {
        // Basic CRUD Operations
        Task<JobCardDependency?> GetByIdAsync(int id);
        Task<IEnumerable<JobCardDependency>> GetAllAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(JobCardDependency dependency);
        Task<bool> DeleteAsync(int id);

        // Dependency Queries
        Task<IEnumerable<JobCardDependency>> GetDependenciesForJobCardAsync(int jobCardId);
        Task<IEnumerable<JobCardDependency>> GetPrerequisitesForJobCardAsync(int jobCardId);
        Task<IEnumerable<JobCardDependency>> GetUnresolvedDependenciesAsync(int jobCardId);
        Task<bool> HasUnresolvedDependenciesAsync(int jobCardId);

        // Resolution Operations
        Task<bool> MarkAsResolvedAsync(int dependencyId);
        Task<bool> MarkAllResolvedForPrerequisiteAsync(int prerequisiteJobCardId);

        // Circular Dependency Check
        Task<bool> WouldCreateCircularDependencyAsync(int dependentJobCardId, int prerequisiteJobCardId);
    }
}
