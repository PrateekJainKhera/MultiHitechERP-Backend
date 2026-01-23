using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Child Part Template operations
    /// </summary>
    public interface IChildPartTemplateRepository
    {
        // Basic CRUD Operations
        Task<ChildPartTemplate?> GetByIdAsync(int id);
        Task<ChildPartTemplate?> GetByNameAsync(string templateName);
        Task<IEnumerable<ChildPartTemplate>> GetAllAsync();
        Task<IEnumerable<ChildPartTemplate>> GetActiveTemplatesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(ChildPartTemplate template);
        Task<bool> UpdateAsync(ChildPartTemplate template);
        Task<bool> DeleteAsync(int id);

        // Queries
        Task<IEnumerable<ChildPartTemplate>> GetByChildPartTypeAsync(string childPartType);
        Task<IEnumerable<ChildPartTemplate>> GetByCategoryAsync(string category);
        Task<IEnumerable<ChildPartTemplate>> GetByProcessTemplateIdAsync(int processTemplateId);
        Task<IEnumerable<ChildPartTemplate>> GetDefaultTemplatesAsync();
        Task<bool> ExistsAsync(string templateName);

        // Approval
        Task<bool> ApproveTemplateAsync(int id, string approvedBy);
    }
}
