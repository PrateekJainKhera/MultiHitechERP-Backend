using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Product Template operations
    /// </summary>
    public interface IProductTemplateRepository
    {
        // Basic CRUD Operations
        Task<ProductTemplate?> GetByIdAsync(int id);
        Task<ProductTemplate?> GetByNameAsync(string templateName);
        Task<IEnumerable<ProductTemplate>> GetAllAsync();
        Task<IEnumerable<ProductTemplate>> GetActiveTemplatesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(ProductTemplate template);
        Task<bool> UpdateAsync(ProductTemplate template);
        Task<bool> DeleteAsync(int id);

        // Queries
        Task<IEnumerable<ProductTemplate>> GetByProductTypeAsync(string productType);
        Task<IEnumerable<ProductTemplate>> GetByCategoryAsync(string category);
        Task<IEnumerable<ProductTemplate>> GetByProcessTemplateIdAsync(int processTemplateId);
        Task<IEnumerable<ProductTemplate>> GetDefaultTemplatesAsync();
        Task<bool> ExistsAsync(string templateName);

        // Approval
        Task<bool> ApproveTemplateAsync(int id, string approvedBy);
    }
}
