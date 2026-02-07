using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IProcessMachineCapabilityRepository
    {
        Task<IEnumerable<ProcessMachineCapability>> GetAllAsync();
        Task<ProcessMachineCapability?> GetByIdAsync(int id);
        Task<IEnumerable<ProcessMachineCapability>> GetByProcessIdAsync(int processId);
        Task<IEnumerable<ProcessMachineCapability>> GetByMachineIdAsync(int machineId);
        Task<IEnumerable<ProcessMachineCapability>> GetCapableMachinesForProcessAsync(int processId);
        Task<ProcessMachineCapability?> GetByProcessAndMachineAsync(int processId, int machineId);
        Task<int> InsertAsync(ProcessMachineCapability capability);
        Task<bool> UpdateAsync(ProcessMachineCapability capability);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int processId, int machineId);
    }
}
