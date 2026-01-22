using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Material master operations
    /// </summary>
    public interface IMaterialRepository
    {
        // Basic CRUD Operations
        Task<Material?> GetByIdAsync(int id);
        Task<Material?> GetByMaterialCodeAsync(string materialCode);
        Task<IEnumerable<Material>> GetAllAsync();
        Task<IEnumerable<Material>> GetActiveMaterialsAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Material material);
        Task<bool> UpdateAsync(Material material);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);

        // Queries
        Task<IEnumerable<Material>> SearchByNameAsync(string name);
        Task<IEnumerable<Material>> GetByCategoryAsync(string category);
        Task<IEnumerable<Material>> GetByGradeAsync(string grade);
        Task<IEnumerable<Material>> GetByMaterialTypeAsync(string materialType);
        Task<IEnumerable<Material>> GetLowStockMaterialsAsync();
        Task<bool> ExistsAsync(string materialCode);
    }
}
