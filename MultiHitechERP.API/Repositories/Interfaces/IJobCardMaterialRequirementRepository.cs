using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Planning;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for JobCardMaterialRequirement operations
    /// </summary>
    public interface IJobCardMaterialRequirementRepository
    {
        // Basic CRUD Operations
        Task<JobCardMaterialRequirement?> GetByIdAsync(int id);
        Task<IEnumerable<JobCardMaterialRequirement>> GetAllAsync();
        Task<IEnumerable<JobCardMaterialRequirement>> GetByJobCardIdAsync(int jobCardId);

        // Create, Update, Delete
        Task<int> InsertAsync(JobCardMaterialRequirement requirement);
        Task<bool> UpdateAsync(JobCardMaterialRequirement requirement);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByJobCardIdAsync(int jobCardId);

        // Batch Operations
        Task<bool> InsertBatchAsync(IEnumerable<JobCardMaterialRequirement> requirements);
    }
}
