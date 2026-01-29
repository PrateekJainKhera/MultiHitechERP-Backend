using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMaterialCategoryRepository
    {
        Task<IEnumerable<MaterialCategory>> GetAllAsync();
        Task<MaterialCategory?> GetByIdAsync(int id);
        Task<MaterialCategory?> GetByCategoryCodeAsync(string categoryCode);
        Task<IEnumerable<MaterialCategory>> GetByMaterialTypeAsync(string materialType);
        Task<IEnumerable<MaterialCategory>> GetActiveAsync();
        Task<IEnumerable<MaterialCategory>> SearchByNameAsync(string searchTerm);
        Task<int> CreateAsync(MaterialCategory category);
        Task<bool> UpdateAsync(int id, MaterialCategory category);
        Task<bool> DeleteAsync(int id);
        Task<bool> CategoryCodeExistsAsync(string categoryCode, int? excludeId = null);

        // Code Generation
        Task<int> GetNextSequenceNumberAsync(string materialType);
    }
}
