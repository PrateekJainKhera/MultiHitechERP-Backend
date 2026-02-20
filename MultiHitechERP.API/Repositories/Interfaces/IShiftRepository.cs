using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Scheduling;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IShiftRepository
    {
        Task<IEnumerable<ShiftMaster>> GetAllAsync();
        Task<IEnumerable<ShiftMaster>> GetActiveAsync();
        Task<ShiftMaster?> GetByIdAsync(int id);
        Task<int> InsertAsync(ShiftMaster shift);
        Task<bool> UpdateAsync(ShiftMaster shift);
        Task<bool> DeleteAsync(int id);
    }
}
