using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMISRepository
    {
        Task<MISOverviewResponse> GetOverviewAsync();
        Task<MachineModelsResponse> GetMachineModelsAsync();
        Task<MachineModelDetailResponse> GetMachineModelDetailAsync(string modelName);
        Task<List<MachineModelCustomerRow>> GetMachineModelCustomersAsync(string modelName, string? rollerType, int? numberOfTeeth);
    }
}
