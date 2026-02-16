using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IChildPartTypeRepository
    {
        Task<IEnumerable<ChildPartType>> GetAllAsync();
        Task<IEnumerable<ChildPartType>> GetActiveAsync();
        Task<ChildPartType?> GetByIdAsync(int id);
        Task<int> CreateAsync(ChildPartType childPartType);
        Task<bool> DeleteAsync(int id);
    }
}
