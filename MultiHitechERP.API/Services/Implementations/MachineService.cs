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

                var response = await LoadAndMapToResponseAsync(machine);
                return ApiResponse<MachineResponse>.SuccessResponse(response);
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

                var response = await LoadAndMapToResponseAsync(machine);
                return ApiResponse<MachineResponse>.SuccessResponse(response);
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
                var responses = new List<MachineResponse>();

                foreach (var machine in machines)
                {
                    responses.Add(await LoadAndMapToResponseAsync(machine));
                }

                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
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
                var responses = new List<MachineResponse>();

                foreach (var machine in machines)
                {
                    responses.Add(await LoadAndMapToResponseAsync(machine));
                }

                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
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
                var responses = new List<MachineResponse>();

                foreach (var machine in machines)
                {
                    responses.Add(await LoadAndMapToResponseAsync(machine));
                }

                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
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
                var responses = new List<MachineResponse>();

                foreach (var machine in machines)
                {
                    responses.Add(await LoadAndMapToResponseAsync(machine));
                }

                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
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
                    DailyCapacityHours = request.DailyCapacityHours,
                    IsActive = true,
                    CreatedBy = "System"
                };

                var machineId = await _machineRepository.InsertAsync(machine);

                // Save process category mappings
                if (request.ProcessCategoryIds != null && request.ProcessCategoryIds.Count > 0)
                {
                    await _machineRepository.SaveProcessCategoriesAsync(machineId, request.ProcessCategoryIds);
                }

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
                existingMachine.DailyCapacityHours = request.DailyCapacityHours;
                existingMachine.IsActive = request.IsActive;
                existingMachine.UpdatedBy = "System";

                var success = await _machineRepository.UpdateAsync(existingMachine);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update machine");

                // Save process category mappings
                await _machineRepository.SaveProcessCategoriesAsync(request.Id, request.ProcessCategoryIds ?? new List<int>());

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

        /// <summary>
        /// Loads process categories for a machine and maps it to response
        /// </summary>
        private async Task<MachineResponse> LoadAndMapToResponseAsync(Machine machine)
        {
            await _machineRepository.LoadProcessCategoriesAsync(machine);
            return MapToResponse(machine);
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
                DailyCapacityHours = machine.DailyCapacityHours,
                ProcessCategoryIds = machine.ProcessCategoryIds,
                ProcessCategoryNames = machine.ProcessCategoryNames,
                IsActive = machine.IsActive,
                CreatedAt = machine.CreatedAt,
                CreatedBy = machine.CreatedBy,
                UpdatedAt = machine.UpdatedAt,
                UpdatedBy = machine.UpdatedBy
            };
        }
    }
}
