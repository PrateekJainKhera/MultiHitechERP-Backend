using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ProcessMachineCapabilityService : IProcessMachineCapabilityService
    {
        private readonly IProcessMachineCapabilityRepository _repository;

        public ProcessMachineCapabilityService(IProcessMachineCapabilityRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetAllAsync()
        {
            try
            {
                var capabilities = await _repository.GetAllAsync();
                var response = capabilities.Select(MapToResponse);
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.ErrorResponse(
                    $"Failed to retrieve process machine capabilities: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<ProcessMachineCapabilityResponse>> GetByIdAsync(int id)
        {
            try
            {
                var capability = await _repository.GetByIdAsync(id);
                if (capability == null)
                {
                    return ApiResponse<ProcessMachineCapabilityResponse>.ErrorResponse(
                        $"Process machine capability with ID {id} not found"
                    );
                }
                return ApiResponse<ProcessMachineCapabilityResponse>.SuccessResponse(MapToResponse(capability));
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessMachineCapabilityResponse>.ErrorResponse(
                    $"Failed to retrieve process machine capability: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetByProcessIdAsync(int processId)
        {
            try
            {
                var capabilities = await _repository.GetByProcessIdAsync(processId);
                var response = capabilities.Select(MapToResponse);
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.ErrorResponse(
                    $"Failed to retrieve capabilities for process {processId}: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetByMachineIdAsync(int machineId)
        {
            try
            {
                var capabilities = await _repository.GetByMachineIdAsync(machineId);
                var response = capabilities.Select(MapToResponse);
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.ErrorResponse(
                    $"Failed to retrieve capabilities for machine {machineId}: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>> GetCapableMachinesForProcessAsync(int processId)
        {
            try
            {
                var capabilities = await _repository.GetCapableMachinesForProcessAsync(processId);
                var response = capabilities.Select(MapToResponse);
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessMachineCapabilityResponse>>.ErrorResponse(
                    $"Failed to retrieve capable machines for process {processId}: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateProcessMachineCapabilityRequest request)
        {
            try
            {
                // Check if combination already exists
                var exists = await _repository.ExistsAsync(request.ProcessId, request.MachineId);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse(
                        $"A capability already exists for Process ID {request.ProcessId} and Machine ID {request.MachineId}"
                    );
                }

                var capability = new ProcessMachineCapability
                {
                    ProcessId = request.ProcessId,
                    MachineId = request.MachineId,
                    SetupTimeHours = request.SetupTimeHours,
                    CycleTimePerPieceHours = request.CycleTimePerPieceHours,
                    PreferenceLevel = request.PreferenceLevel,
                    EfficiencyRating = request.EfficiencyRating,
                    IsPreferredMachine = request.IsPreferredMachine,
                    MaxWorkpieceLength = request.MaxWorkpieceLength,
                    MaxWorkpieceDiameter = request.MaxWorkpieceDiameter,
                    MaxBatchSize = request.MaxBatchSize,
                    HourlyRate = request.HourlyRate,
                    EstimatedCostPerPiece = request.EstimatedCostPerPiece,
                    IsActive = request.IsActive,
                    AvailableFrom = request.AvailableFrom,
                    AvailableTo = request.AvailableTo,
                    Remarks = request.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                var id = await _repository.InsertAsync(capability);
                return ApiResponse<int>.SuccessResponse(id, "Process machine capability created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Failed to create process machine capability: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, UpdateProcessMachineCapabilityRequest request)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"ProcessMachineCapability with ID {id} not found");
                }

                // Check if changing to an existing combination
                if (existing.ProcessId != request.ProcessId || existing.MachineId != request.MachineId)
                {
                    var exists = await _repository.ExistsAsync(request.ProcessId, request.MachineId);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse(
                            $"A capability already exists for Process ID {request.ProcessId} and Machine ID {request.MachineId}"
                        );
                    }
                }

                existing.ProcessId = request.ProcessId;
                existing.MachineId = request.MachineId;
                existing.SetupTimeHours = request.SetupTimeHours;
                existing.CycleTimePerPieceHours = request.CycleTimePerPieceHours;
                existing.PreferenceLevel = request.PreferenceLevel;
                existing.EfficiencyRating = request.EfficiencyRating;
                existing.IsPreferredMachine = request.IsPreferredMachine;
                existing.MaxWorkpieceLength = request.MaxWorkpieceLength;
                existing.MaxWorkpieceDiameter = request.MaxWorkpieceDiameter;
                existing.MaxBatchSize = request.MaxBatchSize;
                existing.HourlyRate = request.HourlyRate;
                existing.EstimatedCostPerPiece = request.EstimatedCostPerPiece;
                existing.IsActive = request.IsActive;
                existing.AvailableFrom = request.AvailableFrom;
                existing.AvailableTo = request.AvailableTo;
                existing.Remarks = request.Remarks;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = request.UpdatedBy ?? "System";

                var result = await _repository.UpdateAsync(existing);
                return ApiResponse<bool>.SuccessResponse(result, "Process machine capability updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to update process machine capability: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"ProcessMachineCapability with ID {id} not found");
                }

                var result = await _repository.DeleteAsync(id);
                return ApiResponse<bool>.SuccessResponse(result, "Process machine capability deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to delete process machine capability: {ex.Message}");
            }
        }

        private static ProcessMachineCapabilityResponse MapToResponse(ProcessMachineCapability capability)
        {
            return new ProcessMachineCapabilityResponse
            {
                Id = capability.Id,
                ProcessId = capability.ProcessId,
                MachineId = capability.MachineId,
                ProcessName = capability.ProcessName,
                MachineName = capability.MachineName,
                MachineCode = capability.MachineCode,
                SetupTimeHours = capability.SetupTimeHours,
                CycleTimePerPieceHours = capability.CycleTimePerPieceHours,
                PreferenceLevel = capability.PreferenceLevel,
                EfficiencyRating = capability.EfficiencyRating,
                IsPreferredMachine = capability.IsPreferredMachine,
                MaxWorkpieceLength = capability.MaxWorkpieceLength,
                MaxWorkpieceDiameter = capability.MaxWorkpieceDiameter,
                MaxBatchSize = capability.MaxBatchSize,
                HourlyRate = capability.HourlyRate,
                EstimatedCostPerPiece = capability.EstimatedCostPerPiece,
                IsActive = capability.IsActive,
                AvailableFrom = capability.AvailableFrom,
                AvailableTo = capability.AvailableTo,
                Remarks = capability.Remarks,
                CreatedAt = capability.CreatedAt,
                CreatedBy = capability.CreatedBy,
                UpdatedAt = capability.UpdatedAt,
                UpdatedBy = capability.UpdatedBy
            };
        }
    }
}
