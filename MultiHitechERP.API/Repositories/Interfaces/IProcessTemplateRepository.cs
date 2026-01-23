using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Process Template operations
    /// </summary>
    public interface IProcessTemplateRepository
    {
        // Template CRUD Operations
        Task<ProcessTemplate?> GetByIdAsync(int id);
        Task<ProcessTemplate?> GetByNameAsync(string templateName);
        Task<IEnumerable<ProcessTemplate>> GetAllAsync();
        Task<IEnumerable<ProcessTemplate>> GetActiveTemplatesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(ProcessTemplate template);
        Task<bool> UpdateAsync(ProcessTemplate template);
        Task<bool> DeleteAsync(int id);

        // Template Steps Operations
        Task<IEnumerable<ProcessTemplateStep>> GetStepsByTemplateIdAsync(int templateId);
        Task<ProcessTemplateStep?> GetStepByIdAsync(int stepId);
        Task<int> InsertStepAsync(ProcessTemplateStep step);
        Task<bool> UpdateStepAsync(ProcessTemplateStep step);
        Task<bool> DeleteStepAsync(int stepId);
        Task<bool> DeleteAllStepsAsync(int templateId);

        // Queries
        Task<IEnumerable<ProcessTemplate>> GetByProductIdAsync(int productId);
        Task<IEnumerable<ProcessTemplate>> GetByChildPartIdAsync(int childPartId);
        Task<IEnumerable<ProcessTemplate>> GetByTemplateTypeAsync(string templateType);
        Task<IEnumerable<ProcessTemplate>> GetDefaultTemplatesAsync();
        Task<bool> ExistsAsync(string templateName);

        // Approval
        Task<bool> ApproveTemplateAsync(int id, string approvedBy);
    }
}
