using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMachineModelRepository
    {
        Task<IEnumerable<MachineModel>> GetAllAsync();
        Task<MachineModel?> GetByIdAsync(int id);
        Task<MachineModel?> GetByNameAsync(string modelName);
        Task<int> CreateAsync(MachineModel model);
        Task<bool> UpdateAsync(MachineModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByNameAsync(string modelName);
        Task<int> GetProductCountAsync(int modelId);
    }
}
