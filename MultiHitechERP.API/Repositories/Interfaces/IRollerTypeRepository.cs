using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IRollerTypeRepository
    {
        Task<IEnumerable<RollerType>> GetAllAsync();
        Task<RollerType?> GetByIdAsync(int id);
        Task<int> CreateAsync(RollerType rollerType);
        Task<bool> DeleteAsync(int id);
    }
}
