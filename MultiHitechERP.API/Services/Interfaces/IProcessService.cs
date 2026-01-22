using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Process business logic
    /// </summary>
    public interface IProcessService
    {
        // CRUD Operations
        Task<ApiResponse<ProcessResponse>> GetByIdAsync(int id);
        Task<ApiResponse<ProcessResponse>> GetByProcessCodeAsync(string processCode);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetActiveProcessesAsync();

        Task<ApiResponse<int>> CreateProcessAsync(CreateProcessRequest request);
        Task<ApiResponse<bool>> UpdateProcessAsync(UpdateProcessRequest request);
        Task<ApiResponse<bool>> DeleteProcessAsync(int id);

        // Status Operations
        Task<ApiResponse<bool>> ActivateProcessAsync(int id);
        Task<ApiResponse<bool>> DeactivateProcessAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByProcessTypeAsync(string processType);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByDepartmentAsync(string department);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByMachineTypeAsync(string machineType);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetOutsourcedProcessesAsync();
    }
}
