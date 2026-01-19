using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        public Task<Product?> GetByIdAsync(Guid id) => Task.FromResult<Product?>(null);
        public Task<Product?> GetByProductCodeAsync(string productCode) => Task.FromResult<Product?>(null);
        public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<IEnumerable<Product>> GetActiveProductsAsync() => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<Guid> InsertAsync(Product product) => Task.FromResult(Guid.NewGuid());
        public Task<bool> UpdateAsync(Product product) => Task.FromResult(true);
        public Task<bool> DeleteAsync(Guid id) => Task.FromResult(true);
        public Task<bool> ActivateAsync(Guid id) => Task.FromResult(true);
        public Task<bool> DeactivateAsync(Guid id) => Task.FromResult(true);
        public Task<IEnumerable<Product>> SearchByNameAsync(string name) => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<IEnumerable<Product>> GetByCategoryAsync(string category) => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<IEnumerable<Product>> GetByProductTypeAsync(string productType) => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<IEnumerable<Product>> GetByDrawingIdAsync(Guid drawingId) => Task.FromResult<IEnumerable<Product>>(Array.Empty<Product>());
        public Task<bool> ExistsAsync(string productCode) => Task.FromResult(false);
    }
}
