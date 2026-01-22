using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ProcessService : IProcessService
    {
        private readonly IProcessRepository _processRepository;

        public ProcessService(IProcessRepository processRepository)
        {
            _processRepository = processRepository;
        }

        public async Task<ApiResponse<ProcessResponse>> GetByIdAsync(int id)
        {
            try
            {
                var process = await _processRepository.GetByIdAsync(id);
                if (process == null)
                    return ApiResponse<ProcessResponse>.ErrorResponse($"Process with ID {id} not found");

                return ApiResponse<ProcessResponse>.SuccessResponse(MapToResponse(process));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessResponse>.ErrorResponse($"Error retrieving process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProcessResponse>> GetByProcessCodeAsync(string processCode)
        {
            try
            {
                var process = await _processRepository.GetByProcessCodeAsync(processCode);
                if (process == null)
                    return ApiResponse<ProcessResponse>.ErrorResponse($"Process {processCode} not found");

                return ApiResponse<ProcessResponse>.SuccessResponse(MapToResponse(process));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessResponse>.ErrorResponse($"Error retrieving process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetAllAsync()
        {
            try
            {
                var processes = await _processRepository.GetAllAsync();
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error retrieving processes: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetActiveProcessesAsync()
        {
            try
            {
                var processes = await _processRepository.GetActiveProcessesAsync();
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error retrieving active processes: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateProcessAsync(CreateProcessRequest request)
        {
            try
            {
                var exists = await _processRepository.ExistsAsync(request.ProcessCode);
                if (exists)
                    return ApiResponse<int>.ErrorResponse($"Process code '{request.ProcessCode}' already exists");

                if (string.IsNullOrWhiteSpace(request.ProcessName))
                    return ApiResponse<int>.ErrorResponse("Process name is required");

                var process = new Models.Masters.Process
                {
                    ProcessCode = request.ProcessCode.Trim().ToUpper(),
                    ProcessName = request.ProcessName.Trim(),
                    ProcessType = request.ProcessType?.Trim(),
                    Category = request.Category?.Trim(),
                    Department = request.Department?.Trim(),
                    Description = request.Description?.Trim(),
                    ProcessDetails = request.ProcessDetails?.Trim(),
                    MachineType = request.MachineType?.Trim(),
                    DefaultMachineId = request.DefaultMachineId,
                    DefaultMachineName = request.DefaultMachineName?.Trim(),
                    StandardSetupTimeMin = request.StandardSetupTimeMin ?? 0,
                    StandardCycleTimeMin = request.StandardCycleTimeMin ?? 0,
                    StandardCycleTimePerPiece = request.StandardCycleTimePerPiece,
                    SkillLevel = request.SkillLevel ?? "Medium",
                    OperatorsRequired = request.OperatorsRequired ?? 1,
                    HourlyRate = request.HourlyRate ?? 0,
                    StandardCostPerPiece = request.StandardCostPerPiece,
                    RequiresQC = request.RequiresQC,
                    QCCheckpoints = request.QCCheckpoints?.Trim(),
                    IsOutsourced = request.IsOutsourced,
                    PreferredVendor = request.PreferredVendor?.Trim(),
                    IsActive = true,
                    Status = "Active",
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var processId = await _processRepository.InsertAsync(process);
                return ApiResponse<int>.SuccessResponse(processId, $"Process '{request.ProcessCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateProcessAsync(UpdateProcessRequest request)
        {
            try
            {
                var existingProcess = await _processRepository.GetByIdAsync(request.Id);
                if (existingProcess == null)
                    return ApiResponse<bool>.ErrorResponse("Process not found");

                if (existingProcess.ProcessCode != request.ProcessCode)
                {
                    var exists = await _processRepository.ExistsAsync(request.ProcessCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Process code '{request.ProcessCode}' already exists");
                }

                existingProcess.ProcessCode = request.ProcessCode.Trim().ToUpper();
                existingProcess.ProcessName = request.ProcessName.Trim();
                existingProcess.ProcessType = request.ProcessType?.Trim();
                existingProcess.Category = request.Category?.Trim();
                existingProcess.Department = request.Department?.Trim();
                existingProcess.Description = request.Description?.Trim();
                existingProcess.ProcessDetails = request.ProcessDetails?.Trim();
                existingProcess.MachineType = request.MachineType?.Trim();
                existingProcess.DefaultMachineId = request.DefaultMachineId;
                existingProcess.DefaultMachineName = request.DefaultMachineName?.Trim();
                existingProcess.StandardSetupTimeMin = request.StandardSetupTimeMin;
                existingProcess.StandardCycleTimeMin = request.StandardCycleTimeMin;
                existingProcess.StandardCycleTimePerPiece = request.StandardCycleTimePerPiece;
                existingProcess.SkillLevel = request.SkillLevel;
                existingProcess.OperatorsRequired = request.OperatorsRequired;
                existingProcess.HourlyRate = request.HourlyRate;
                existingProcess.StandardCostPerPiece = request.StandardCostPerPiece;
                existingProcess.RequiresQC = request.RequiresQC;
                existingProcess.QCCheckpoints = request.QCCheckpoints?.Trim();
                existingProcess.IsOutsourced = request.IsOutsourced;
                existingProcess.PreferredVendor = request.PreferredVendor?.Trim();
                existingProcess.IsActive = request.IsActive;
                existingProcess.Status = request.Status;
                existingProcess.Remarks = request.Remarks?.Trim();
                existingProcess.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingProcess.UpdatedAt = DateTime.UtcNow;

                var success = await _processRepository.UpdateAsync(existingProcess);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update process");

                return ApiResponse<bool>.SuccessResponse(true, "Process updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProcessAsync(int id)
        {
            try
            {
                var process = await _processRepository.GetByIdAsync(id);
                if (process == null)
                    return ApiResponse<bool>.ErrorResponse("Process not found");

                var success = await _processRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete process");

                return ApiResponse<bool>.SuccessResponse(true, "Process deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ActivateProcessAsync(int id)
        {
            try
            {
                var process = await _processRepository.GetByIdAsync(id);
                if (process == null)
                    return ApiResponse<bool>.ErrorResponse("Process not found");
                if (process.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Process is already active");

                var success = await _processRepository.ActivateAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to activate process");

                return ApiResponse<bool>.SuccessResponse(true, "Process activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateProcessAsync(int id)
        {
            try
            {
                var process = await _processRepository.GetByIdAsync(id);
                if (process == null)
                    return ApiResponse<bool>.ErrorResponse("Process not found");
                if (!process.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Process is already inactive");

                var success = await _processRepository.DeactivateAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to deactivate process");

                return ApiResponse<bool>.SuccessResponse(true, "Process deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating process: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByProcessTypeAsync(string processType)
        {
            try
            {
                var processes = await _processRepository.GetByProcessTypeAsync(processType);
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByDepartmentAsync(string department)
        {
            try
            {
                var processes = await _processRepository.GetByDepartmentAsync(department);
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetByMachineTypeAsync(string machineType)
        {
            try
            {
                var processes = await _processRepository.GetByMachineTypeAsync(machineType);
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessResponse>>> GetOutsourcedProcessesAsync()
        {
            try
            {
                var processes = await _processRepository.GetOutsourcedProcessesAsync();
                var responses = processes.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static ProcessResponse MapToResponse(Models.Masters.Process process)
        {
            return new ProcessResponse
            {
                Id = process.Id,
                ProcessCode = process.ProcessCode,
                ProcessName = process.ProcessName,
                ProcessType = process.ProcessType,
                Category = process.Category,
                Department = process.Department,
                Description = process.Description,
                ProcessDetails = process.ProcessDetails,
                MachineType = process.MachineType,
                DefaultMachineId = process.DefaultMachineId,
                DefaultMachineName = process.DefaultMachineName,
                StandardSetupTimeMin = process.StandardSetupTimeMin,
                StandardCycleTimeMin = process.StandardCycleTimeMin,
                StandardCycleTimePerPiece = process.StandardCycleTimePerPiece,
                SkillLevel = process.SkillLevel,
                OperatorsRequired = process.OperatorsRequired,
                HourlyRate = process.HourlyRate,
                StandardCostPerPiece = process.StandardCostPerPiece,
                RequiresQC = process.RequiresQC,
                QCCheckpoints = process.QCCheckpoints,
                IsOutsourced = process.IsOutsourced,
                PreferredVendor = process.PreferredVendor,
                IsActive = process.IsActive,
                Status = process.Status,
                Remarks = process.Remarks,
                CreatedAt = process.CreatedAt,
                CreatedBy = process.CreatedBy,
                UpdatedAt = process.UpdatedAt,
                UpdatedBy = process.UpdatedBy
            };
        }
    }
}
