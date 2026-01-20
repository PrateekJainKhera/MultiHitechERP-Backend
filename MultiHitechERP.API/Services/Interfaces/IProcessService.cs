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
        Task<ApiResponse<ProcessResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<ProcessResponse>> GetByProcessCodeAsync(string processCode);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetActiveProcessesAsync();

        Task<ApiResponse<Guid>> CreateProcessAsync(CreateProcessRequest request);
        Task<ApiResponse<bool>> UpdateProcessAsync(UpdateProcessRequest request);
        Task<ApiResponse<bool>> DeleteProcessAsync(Guid id);

        // Status Operations
        Task<ApiResponse<bool>> ActivateProcessAsync(Guid id);
        Task<ApiResponse<bool>> DeactivateProcessAsync(Guid id);

        // Business Queries
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByProcessTypeAsync(string processType);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByDepartmentAsync(string department);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByMachineTypeAsync(string machineType);
        Task<ApiResponse<IEnumerable<ProcessResponse>>> GetOutsourcedProcessesAsync();
    }
}
