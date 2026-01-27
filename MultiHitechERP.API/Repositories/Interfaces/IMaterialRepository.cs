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
        Task<IEnumerable<Material>> GetAllAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Material material);
        Task<bool> UpdateAsync(Material material);
        Task<bool> DeleteAsync(int id);

        // Queries
        Task<IEnumerable<Material>> SearchByNameAsync(string name);
        Task<IEnumerable<Material>> GetByGradeAsync(string grade);
        Task<IEnumerable<Material>> GetByShapeAsync(string shape);
        Task<bool> ExistsByNameAsync(string materialName);
    }
}
