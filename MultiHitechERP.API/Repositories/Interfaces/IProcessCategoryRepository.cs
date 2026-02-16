using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IProcessCategoryRepository
    {
        Task<IEnumerable<ProcessCategory>> GetAllAsync();
        Task<ProcessCategory?> GetByIdAsync(int id);
        Task<ProcessCategory?> GetByCategoryCodeAsync(string categoryCode);
        Task<int> CreateAsync(ProcessCategory processCategory);
        Task<bool> UpdateAsync(ProcessCategory processCategory);
        Task<bool> DeleteAsync(int id);
        Task<bool> CategoryCodeExistsAsync(string categoryCode, int? excludeId = null);
    }
}
