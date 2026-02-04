using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMachineRepository
    {
        Task<Machine?> GetByIdAsync(int id);
        Task<Machine?> GetByMachineCodeAsync(string machineCode);
        Task<IEnumerable<Machine>> GetAllAsync();
        Task<IEnumerable<Machine>> GetActiveMachinesAsync();
        Task<IEnumerable<Machine>> GetByMachineTypeAsync(string machineType);
        Task<IEnumerable<Machine>> GetByDepartmentAsync(string department);

        Task<int> InsertAsync(Machine machine);
        Task<bool> UpdateAsync(Machine machine);
        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(string machineCode);
        Task<string> GetNextMachineCodeAsync();
    }
}
