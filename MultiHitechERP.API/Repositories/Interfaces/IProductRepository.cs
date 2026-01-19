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
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetActiveProductsAsync();

        // Create, Update, Delete
        Task<Guid> InsertAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);

        // Status Operations
        Task<bool> ActivateAsync(Guid id);
        Task<bool> DeactivateAsync(Guid id);

        // Queries
        Task<IEnumerable<Product>> SearchByNameAsync(string name);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetByProductTypeAsync(string productType);
        Task<IEnumerable<Product>> GetByDrawingIdAsync(Guid drawingId);
        Task<bool> ExistsAsync(string productCode);
    }
}
