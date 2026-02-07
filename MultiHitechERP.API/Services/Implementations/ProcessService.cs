using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ProcessService : IProcessService
    {
        private readonly IProcessRepository _processRepository;
        private readonly IProcessMachineCapabilityRepository _capabilityRepository;

        public ProcessService(
            IProcessRepository processRepository,
            IProcessMachineCapabilityRepository capabilityRepository)
        {
            _processRepository = processRepository;
            _capabilityRepository = capabilityRepository;
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
                if (string.IsNullOrWhiteSpace(request.ProcessName))
                    return ApiResponse<int>.ErrorResponse("Process name is required");

                if (string.IsNullOrWhiteSpace(request.Category))
                    return ApiResponse<int>.ErrorResponse("Category is required");

                // Auto-generate ProcessCode: Use first 3 letters of ProcessName + sequence
                string processNamePrefix = request.ProcessName.Length >= 3
                    ? request.ProcessName.Substring(0, 3).ToUpper()
                    : request.ProcessName.ToUpper();

                // Get next sequence number for this category
                int nextSequence = await _processRepository.GetNextSequenceNumberAsync(request.Category);
                string generatedCode = $"{processNamePrefix}-{nextSequence:D3}";

                var process = new Models.Masters.Process
                {
                    ProcessCode = generatedCode,
                    ProcessName = request.ProcessName.Trim(),
                    Category = request.Category.Trim(),
                    DefaultMachine = request.DefaultMachine?.Trim(),
                    DefaultMachineId = request.DefaultMachineId,
                    DefaultSetupTimeHours = request.DefaultSetupTimeHours ?? 0.5m,
                    DefaultCycleTimePerPieceHours = request.DefaultCycleTimePerPieceHours ?? 0.1m,
                    StandardSetupTimeMin = request.StandardSetupTimeMin,
                    RestTimeHours = request.RestTimeHours,
                    Description = request.Description?.Trim(),
                    IsOutsourced = request.IsOutsourced,
                    IsActive = true,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var processId = await _processRepository.InsertAsync(process);

                // Automatically create ProcessMachineCapability if DefaultMachineId is set
                if (request.DefaultMachineId.HasValue)
                {
                    await CreateProcessMachineCapabilityAsync(
                        processId,
                        request.DefaultMachineId.Value,
                        request.DefaultSetupTimeHours ?? 0.5m,
                        request.DefaultCycleTimePerPieceHours ?? 0.1m,
                        request.CreatedBy
                    );
                }

                return ApiResponse<int>.SuccessResponse(processId, $"Process '{generatedCode}' created successfully");
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

                // Track if DefaultMachineId is changing
                var defaultMachineChanged = existingProcess.DefaultMachineId != request.DefaultMachineId;
                var oldMachineId = existingProcess.DefaultMachineId;

                existingProcess.ProcessCode = request.ProcessCode.Trim().ToUpper();
                existingProcess.ProcessName = request.ProcessName.Trim();
                existingProcess.Category = request.Category?.Trim();
                existingProcess.DefaultMachine = request.DefaultMachine?.Trim();
                existingProcess.DefaultMachineId = request.DefaultMachineId;
                existingProcess.DefaultSetupTimeHours = request.DefaultSetupTimeHours ?? 0.5m;
                existingProcess.DefaultCycleTimePerPieceHours = request.DefaultCycleTimePerPieceHours ?? 0.1m;
                existingProcess.StandardSetupTimeMin = request.StandardSetupTimeMin;
                existingProcess.RestTimeHours = request.RestTimeHours;
                existingProcess.Description = request.Description?.Trim();
                existingProcess.IsOutsourced = request.IsOutsourced;
                existingProcess.IsActive = request.IsActive;
                existingProcess.Status = request.Status;
                existingProcess.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingProcess.UpdatedAt = DateTime.UtcNow;

                var success = await _processRepository.UpdateAsync(existingProcess);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update process");

                // Handle ProcessMachineCapability changes if DefaultMachineId changed
                if (defaultMachineChanged)
                {
                    // Unmark old preferred machine if it exists
                    if (oldMachineId.HasValue)
                    {
                        var oldCapability = await _capabilityRepository.GetByProcessAndMachineAsync(request.Id, oldMachineId.Value);
                        if (oldCapability != null)
                        {
                            oldCapability.IsPreferredMachine = false;
                            await _capabilityRepository.UpdateAsync(oldCapability);
                        }
                    }

                    // Create or update capability for new default machine
                    if (request.DefaultMachineId.HasValue)
                    {
                        var existingCapability = await _capabilityRepository.GetByProcessAndMachineAsync(
                            request.Id,
                            request.DefaultMachineId.Value
                        );

                        if (existingCapability != null)
                        {
                            // Update existing capability to mark as preferred
                            existingCapability.IsPreferredMachine = true;
                            existingCapability.SetupTimeHours = request.DefaultSetupTimeHours ?? 0.5m;
                            existingCapability.CycleTimePerPieceHours = request.DefaultCycleTimePerPieceHours ?? 0.1m;
                            existingCapability.PreferenceLevel = 1;
                            existingCapability.UpdatedAt = DateTime.UtcNow;
                            existingCapability.UpdatedBy = request.UpdatedBy ?? "System";
                            await _capabilityRepository.UpdateAsync(existingCapability);
                        }
                        else
                        {
                            // Create new capability
                            await CreateProcessMachineCapabilityAsync(
                                request.Id,
                                request.DefaultMachineId.Value,
                                request.DefaultSetupTimeHours ?? 0.5m,
                                request.DefaultCycleTimePerPieceHours ?? 0.1m,
                                request.UpdatedBy
                            );
                        }
                    }
                }

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

        private async Task CreateProcessMachineCapabilityAsync(
            int processId,
            int machineId,
            decimal setupTimeHours,
            decimal cycleTimePerPieceHours,
            string? createdBy)
        {
            var capability = new ProcessMachineCapability
            {
                ProcessId = processId,
                MachineId = machineId,
                SetupTimeHours = setupTimeHours,
                CycleTimePerPieceHours = cycleTimePerPieceHours,
                PreferenceLevel = 1, // Best choice
                EfficiencyRating = 95.00m, // Default efficiency
                IsPreferredMachine = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy ?? "System"
            };

            await _capabilityRepository.InsertAsync(capability);
        }

        private static ProcessResponse MapToResponse(Models.Masters.Process process)
        {
            return new ProcessResponse
            {
                Id = process.Id,
                ProcessCode = process.ProcessCode,
                ProcessName = process.ProcessName,
                Category = process.Category,
                DefaultMachine = process.DefaultMachine,
                DefaultMachineId = process.DefaultMachineId,
                DefaultMachineName = process.DefaultMachineName,
                DefaultMachineCode = process.DefaultMachineCode,
                DefaultSetupTimeHours = process.DefaultSetupTimeHours,
                DefaultCycleTimePerPieceHours = process.DefaultCycleTimePerPieceHours,
                StandardSetupTimeMin = process.StandardSetupTimeMin,
                RestTimeHours = process.RestTimeHours,
                Description = process.Description,
                IsOutsourced = process.IsOutsourced,
                IsActive = process.IsActive,
                Status = process.Status,
                CreatedAt = process.CreatedAt,
                CreatedBy = process.CreatedBy,
                UpdatedAt = process.UpdatedAt,
                UpdatedBy = process.UpdatedBy
            };
        }
    }
}
