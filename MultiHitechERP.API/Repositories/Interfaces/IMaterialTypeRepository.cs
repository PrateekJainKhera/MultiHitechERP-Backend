using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMaterialTypeRepository
    {
        Task<IEnumerable<MaterialTypeModel>> GetAllAsync();
        Task<MaterialTypeModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(MaterialTypeModel materialType);
        Task<bool> DeleteAsync(int id);
    }
}
