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
        Task<JobCardDependency?> GetByIdAsync(Guid id);
        Task<IEnumerable<JobCardDependency>> GetAllAsync();

        // Create, Update, Delete
        Task<Guid> InsertAsync(JobCardDependency dependency);
        Task<bool> DeleteAsync(Guid id);

        // Dependency Queries
        Task<IEnumerable<JobCardDependency>> GetDependenciesForJobCardAsync(Guid jobCardId);
        Task<IEnumerable<JobCardDependency>> GetPrerequisitesForJobCardAsync(Guid jobCardId);
        Task<IEnumerable<JobCardDependency>> GetUnresolvedDependenciesAsync(Guid jobCardId);
        Task<bool> HasUnresolvedDependenciesAsync(Guid jobCardId);

        // Resolution Operations
        Task<bool> MarkAsResolvedAsync(Guid dependencyId);
        Task<bool> MarkAllResolvedForPrerequisiteAsync(Guid prerequisiteJobCardId);

        // Circular Dependency Check
        Task<bool> WouldCreateCircularDependencyAsync(Guid dependentJobCardId, Guid prerequisiteJobCardId);
    }
}
