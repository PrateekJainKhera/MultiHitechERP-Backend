using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Product master operations
    /// </summary>
    public interface IProductRepository
    {
        // Basic CRUD Operations
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByPartCodeAsync(string partCode);
        Task<IEnumerable<Product>> GetAllAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);

        // Queries
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> GetByRollerTypeAsync(string rollerType);
        Task<bool> ExistsAsync(string partCode);

        // Code Generation
        Task<int> GetNextSequenceNumberAsync(string rollerType);
    }
}
