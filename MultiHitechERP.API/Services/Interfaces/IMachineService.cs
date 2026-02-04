using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMachineService
    {
        Task<ApiResponse<MachineResponse>> GetByIdAsync(int id);
        Task<ApiResponse<MachineResponse>> GetByMachineCodeAsync(string machineCode);
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetActiveMachinesAsync();
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetByTypeAsync(string machineType);
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetByDepartmentAsync(string department);

        Task<ApiResponse<int>> CreateMachineAsync(CreateMachineRequest request);
        Task<ApiResponse<bool>> UpdateMachineAsync(UpdateMachineRequest request);
        Task<ApiResponse<bool>> DeleteMachineAsync(int id);
    }
}
