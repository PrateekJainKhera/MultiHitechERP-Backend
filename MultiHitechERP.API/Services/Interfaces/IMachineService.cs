using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Machine business logic
    /// </summary>
    public interface IMachineService
    {
        // CRUD Operations
        Task<ApiResponse<MachineResponse>> GetByIdAsync(int id);
        Task<ApiResponse<MachineResponse>> GetByMachineCodeAsync(string machineCode);
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetActiveMachinesAsync();

        Task<ApiResponse<int>> CreateMachineAsync(CreateMachineRequest request);
        Task<ApiResponse<bool>> UpdateMachineAsync(UpdateMachineRequest request);
        Task<ApiResponse<bool>> DeleteMachineAsync(int id);

        // Status Operations
        Task<ApiResponse<bool>> ActivateMachineAsync(int id);
        Task<ApiResponse<bool>> DeactivateMachineAsync(int id);

        // Availability Operations
        Task<ApiResponse<bool>> AssignToJobCardAsync(int id, string jobCardNo);
        Task<ApiResponse<bool>> ReleaseFromJobCardAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetAvailableMachinesAsync();
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetByTypeAsync(string machineType);
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetByDepartmentAsync(string department);
        Task<ApiResponse<IEnumerable<MachineResponse>>> GetDueForMaintenanceAsync();
    }
}
