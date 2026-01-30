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
        Task<ChildPartTemplate?> GetByCodeAsync(string templateCode);
        Task<ChildPartTemplate?> GetByNameAsync(string templateName);
        Task<IEnumerable<ChildPartTemplate>> GetAllAsync();
        Task<IEnumerable<ChildPartTemplate>> GetActiveTemplatesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(ChildPartTemplate template);
        Task<bool> UpdateAsync(ChildPartTemplate template);
        Task<bool> DeleteAsync(int id);

        // Queries
        Task<IEnumerable<ChildPartTemplate>> GetByChildPartTypeAsync(string childPartType);
        Task<IEnumerable<ChildPartTemplate>> GetByRollerTypeAsync(string rollerType);
        Task<bool> ExistsAsync(string templateName);
        Task<int> GetNextSequenceNumberAsync();

        // Material Requirements
        Task<IEnumerable<ChildPartTemplateMaterialRequirement>> GetMaterialRequirementsByTemplateIdAsync(int templateId);
        Task<bool> DeleteMaterialRequirementsByTemplateIdAsync(int templateId);
        Task<bool> InsertMaterialRequirementsAsync(int templateId, IEnumerable<ChildPartTemplateMaterialRequirement> requirements);

        // Process Steps
        Task<IEnumerable<ChildPartTemplateProcessStep>> GetProcessStepsByTemplateIdAsync(int templateId);
        Task<bool> DeleteProcessStepsByTemplateIdAsync(int templateId);
        Task<bool> InsertProcessStepsAsync(int templateId, IEnumerable<ChildPartTemplateProcessStep> steps);
    }
}
