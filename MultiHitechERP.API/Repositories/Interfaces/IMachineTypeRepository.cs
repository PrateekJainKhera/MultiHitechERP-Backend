using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMachineTypeRepository
    {
        Task<IEnumerable<MachineType>> GetAllAsync();
        Task<MachineType?> GetByIdAsync(int id);
        Task<int> CreateAsync(MachineType machineType);
        Task<bool> DeleteAsync(int id);
    }
}
