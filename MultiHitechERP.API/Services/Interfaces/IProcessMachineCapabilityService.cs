using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IProcessMachineCapabilityService
    {
        Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetAllAsync();
        Task<ApiResponse<ProcessMachineCapabilityResponse>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetByProcessIdAsync(int processId);
        Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetByMachineIdAsync(int machineId);
        Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetCapableMachinesForProcessAsync(int processId);
        Task<ApiResponse<int>> CreateAsync(CreateProcessMachineCapabilityRequest request);
        Task<ApiResponse<bool>> UpdateAsync(int id, UpdateProcessMachineCapabilityRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
