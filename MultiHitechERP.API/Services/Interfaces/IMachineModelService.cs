using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMachineModelService
    {
        Task<ApiResponse<MachineModelResponse[]>> GetAllAsync();
        Task<ApiResponse<MachineModelResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(CreateMachineModelRequest request);
        Task<ApiResponse<bool>> UpdateAsync(UpdateMachineModelRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
