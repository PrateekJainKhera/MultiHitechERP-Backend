using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Component>> GetAllAsync();
        Task<IEnumerable<ComponentLowStockResponse>> GetLowStockAsync();
        Task<Component?> GetByIdAsync(int id);
        Task<Component?> GetByPartNumberAsync(string partNumber);
        Task<IEnumerable<Component>> GetByCategoryAsync(string category);
        Task<IEnumerable<Component>> SearchByNameAsync(string searchTerm);
        Task<int> CreateAsync(Component component);
        Task<bool> UpdateAsync(int id, Component component);
        Task<bool> DeleteAsync(int id);
        Task<bool> PartNumberExistsAsync(string partNumber, int? excludeId = null);

        // Code Generation
        Task<int> GetNextSequenceNumberAsync(string category);
    }
}
