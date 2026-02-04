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
    public class MachineService : IMachineService
    {
        private readonly IMachineRepository _machineRepository;

        public MachineService(IMachineRepository machineRepository)
        {
            _machineRepository = machineRepository;
        }

        public async Task<ApiResponse<MachineResponse>> GetByIdAsync(int id)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<MachineResponse>.ErrorResponse($"Machine with ID {id} not found");

                return ApiResponse<MachineResponse>.SuccessResponse(MapToResponse(machine));
            }
            catch (Exception ex)
            {
                return ApiResponse<MachineResponse>.ErrorResponse($"Error retrieving machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MachineResponse>> GetByMachineCodeAsync(string machineCode)
        {
            try
            {
                var machine = await _machineRepository.GetByMachineCodeAsync(machineCode);
                if (machine == null)
                    return ApiResponse<MachineResponse>.ErrorResponse($"Machine {machineCode} not found");

                return ApiResponse<MachineResponse>.SuccessResponse(MapToResponse(machine));
            }
            catch (Exception ex)
            {
                return ApiResponse<MachineResponse>.ErrorResponse($"Error retrieving machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetAllAsync()
        {
            try
            {
                var machines = await _machineRepository.GetAllAsync();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(machines.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error retrieving machines: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetActiveMachinesAsync()
        {
            try
            {
                var machines = await _machineRepository.GetActiveMachinesAsync();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(machines.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error retrieving active machines: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetByTypeAsync(string machineType)
        {
            try
            {
                var machines = await _machineRepository.GetByMachineTypeAsync(machineType);
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(machines.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetByDepartmentAsync(string department)
        {
            try
            {
                var machines = await _machineRepository.GetByDepartmentAsync(department);
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(machines.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateMachineAsync(CreateMachineRequest request)
        {
            try
            {
                var machineCode = await _machineRepository.GetNextMachineCodeAsync();

                var machine = new Machine
                {
                    MachineCode = machineCode,
                    MachineName = request.MachineName.Trim(),
                    MachineType = request.MachineType?.Trim(),
                    Location = request.Location?.Trim(),
                    Department = request.Department?.Trim(),
                    Status = request.Status?.Trim() ?? "Idle",
                    Notes = request.Notes?.Trim(),
                    IsActive = true,
                    CreatedBy = "System"
                };

                var machineId = await _machineRepository.InsertAsync(machine);
                return ApiResponse<int>.SuccessResponse(machineId, $"Machine '{machine.MachineCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateMachineAsync(UpdateMachineRequest request)
        {
            try
            {
                var existingMachine = await _machineRepository.GetByIdAsync(request.Id);
                if (existingMachine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");

                existingMachine.MachineName = request.MachineName.Trim();
                existingMachine.MachineType = request.MachineType?.Trim();
                existingMachine.Location = request.Location?.Trim();
                existingMachine.Department = request.Department?.Trim();
                existingMachine.Status = request.Status?.Trim();
                existingMachine.Notes = request.Notes?.Trim();
                existingMachine.IsActive = request.IsActive;
                existingMachine.UpdatedBy = "System";

                var success = await _machineRepository.UpdateAsync(existingMachine);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update machine");

                return ApiResponse<bool>.SuccessResponse(true, "Machine updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteMachineAsync(int id)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");

                var success = await _machineRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete machine");

                return ApiResponse<bool>.SuccessResponse(true, "Machine deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting machine: {ex.Message}");
            }
        }

        private static MachineResponse MapToResponse(Machine machine)
        {
            return new MachineResponse
            {
                Id = machine.Id,
                MachineCode = machine.MachineCode,
                MachineName = machine.MachineName,
                MachineType = machine.MachineType,
                Location = machine.Location,
                Department = machine.Department,
                Status = machine.Status,
                Notes = machine.Notes,
                IsActive = machine.IsActive,
                CreatedAt = machine.CreatedAt,
                CreatedBy = machine.CreatedBy,
                UpdatedAt = machine.UpdatedAt,
                UpdatedBy = machine.UpdatedBy
            };
        }
    }
}
