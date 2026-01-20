using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Operator operations
    /// </summary>
    public interface IOperatorService
    {
        // Basic CRUD Operations
        Task<ApiResponse<OperatorResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<OperatorResponse>> GetByOperatorCodeAsync(string operatorCode);
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetActiveOperatorsAsync();

        // Create, Update, Delete
        Task<ApiResponse<Guid>> CreateOperatorAsync(CreateOperatorRequest request);
        Task<ApiResponse<bool>> UpdateOperatorAsync(UpdateOperatorRequest request);
        Task<ApiResponse<bool>> DeleteOperatorAsync(Guid id);

        // Status & Availability Operations
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);
        Task<ApiResponse<bool>> UpdateAvailabilityAsync(Guid id, bool isAvailable);
        Task<ApiResponse<bool>> AssignToJobCardAsync(Guid id, Guid jobCardId, string jobCardNo, Guid? machineId);
        Task<ApiResponse<bool>> ReleaseFromJobCardAsync(Guid id);

        // Queries
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetAvailableOperatorsAsync();
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByDepartmentAsync(string department);
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByShiftAsync(string shift);
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetBySkillLevelAsync(string skillLevel);
        Task<ApiResponse<IEnumerable<OperatorResponse>>> GetByMachineExpertiseAsync(Guid machineId);
    }
}
