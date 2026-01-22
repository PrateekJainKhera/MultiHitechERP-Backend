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
                var responses = machines.Select(MapToResponse).ToList();
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
                var responses = machines.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error retrieving active machines: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateMachineAsync(CreateMachineRequest request)
        {
            try
            {
                var exists = await _machineRepository.ExistsAsync(request.MachineCode);
                if (exists)
                    return ApiResponse<int>.ErrorResponse($"Machine code '{request.MachineCode}' already exists");

                if (string.IsNullOrWhiteSpace(request.MachineName))
                    return ApiResponse<int>.ErrorResponse("Machine name is required");

                var machine = new Machine
                {
                    MachineCode = request.MachineCode.Trim().ToUpper(),
                    MachineName = request.MachineName.Trim(),
                    MachineType = request.MachineType?.Trim(),
                    Category = request.Category?.Trim(),
                    Manufacturer = request.Manufacturer?.Trim(),
                    Model = request.Model?.Trim(),
                    SerialNumber = request.SerialNumber?.Trim(),
                    YearOfManufacture = request.YearOfManufacture,
                    Capacity = request.Capacity,
                    CapacityUnit = request.CapacityUnit?.Trim(),
                    Specifications = request.Specifications?.Trim(),
                    MaxWorkpieceLength = request.MaxWorkpieceLength,
                    MaxWorkpieceDiameter = request.MaxWorkpieceDiameter,
                    ChuckSize = request.ChuckSize,
                    Department = request.Department?.Trim(),
                    ShopFloor = request.ShopFloor?.Trim(),
                    Location = request.Location?.Trim(),
                    HourlyRate = request.HourlyRate ?? 0,
                    PowerConsumption = request.PowerConsumption,
                    OperatorsRequired = request.OperatorsRequired ?? 1,
                    PurchaseDate = request.PurchaseDate,
                    MaintenanceSchedule = request.MaintenanceSchedule?.Trim(),
                    IsActive = true,
                    Status = "Available",
                    IsAvailable = true,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var machineId = await _machineRepository.InsertAsync(machine);
                return ApiResponse<int>.SuccessResponse(machineId, $"Machine '{request.MachineCode}' created successfully");
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

                if (existingMachine.MachineCode != request.MachineCode)
                {
                    var exists = await _machineRepository.ExistsAsync(request.MachineCode);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Machine code '{request.MachineCode}' already exists");
                }

                existingMachine.MachineCode = request.MachineCode.Trim().ToUpper();
                existingMachine.MachineName = request.MachineName.Trim();
                existingMachine.MachineType = request.MachineType?.Trim();
                existingMachine.Category = request.Category?.Trim();
                existingMachine.Manufacturer = request.Manufacturer?.Trim();
                existingMachine.Model = request.Model?.Trim();
                existingMachine.SerialNumber = request.SerialNumber?.Trim();
                existingMachine.YearOfManufacture = request.YearOfManufacture;
                existingMachine.Capacity = request.Capacity;
                existingMachine.CapacityUnit = request.CapacityUnit?.Trim();
                existingMachine.Specifications = request.Specifications?.Trim();
                existingMachine.MaxWorkpieceLength = request.MaxWorkpieceLength;
                existingMachine.MaxWorkpieceDiameter = request.MaxWorkpieceDiameter;
                existingMachine.ChuckSize = request.ChuckSize;
                existingMachine.Department = request.Department?.Trim();
                existingMachine.ShopFloor = request.ShopFloor?.Trim();
                existingMachine.Location = request.Location?.Trim();
                existingMachine.HourlyRate = request.HourlyRate;
                existingMachine.PowerConsumption = request.PowerConsumption;
                existingMachine.OperatorsRequired = request.OperatorsRequired;
                existingMachine.PurchaseDate = request.PurchaseDate;
                existingMachine.LastMaintenanceDate = request.LastMaintenanceDate;
                existingMachine.NextMaintenanceDate = request.NextMaintenanceDate;
                existingMachine.MaintenanceSchedule = request.MaintenanceSchedule?.Trim();
                existingMachine.IsActive = request.IsActive;
                existingMachine.Status = request.Status;
                existingMachine.Remarks = request.Remarks?.Trim();
                existingMachine.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingMachine.UpdatedAt = DateTime.UtcNow;

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

        public async Task<ApiResponse<bool>> ActivateMachineAsync(int id)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");
                if (machine.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Machine is already active");

                var success = await _machineRepository.UpdateStatusAsync(id, "Active");
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to activate machine");

                return ApiResponse<bool>.SuccessResponse(true, "Machine activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateMachineAsync(int id)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");
                if (!machine.IsActive)
                    return ApiResponse<bool>.ErrorResponse("Machine is already inactive");

                var success = await _machineRepository.UpdateStatusAsync(id, "Inactive");
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to deactivate machine");

                return ApiResponse<bool>.SuccessResponse(true, "Machine deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignToJobCardAsync(int id, string jobCardNo)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");
                if (!machine.IsAvailable)
                    return ApiResponse<bool>.ErrorResponse($"Machine is already assigned to job card {machine.CurrentJobCardNo}");

                var success = await _machineRepository.AssignToJobCardAsync(id, jobCardNo);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to assign machine");

                return ApiResponse<bool>.SuccessResponse(true, $"Machine assigned to job card {jobCardNo}");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error assigning machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ReleaseFromJobCardAsync(int id)
        {
            try
            {
                var machine = await _machineRepository.GetByIdAsync(id);
                if (machine == null)
                    return ApiResponse<bool>.ErrorResponse("Machine not found");
                if (machine.IsAvailable)
                    return ApiResponse<bool>.ErrorResponse("Machine is not currently assigned");

                var success = await _machineRepository.ReleaseFromJobCardAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to release machine");

                return ApiResponse<bool>.SuccessResponse(true, "Machine released successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error releasing machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetAvailableMachinesAsync()
        {
            try
            {
                var machines = await _machineRepository.GetAvailableMachinesAsync();
                var responses = machines.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetByTypeAsync(string machineType)
        {
            try
            {
                var machines = await _machineRepository.GetByMachineTypeAsync(machineType);
                var responses = machines.Select(MapToResponse).ToList();
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
                var responses = machines.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineResponse>>> GetDueForMaintenanceAsync()
        {
            try
            {
                var machines = await _machineRepository.GetDueForMaintenanceAsync();
                var responses = machines.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MachineResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineResponse>>.ErrorResponse($"Error: {ex.Message}");
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
                Category = machine.Category,
                Manufacturer = machine.Manufacturer,
                Model = machine.Model,
                SerialNumber = machine.SerialNumber,
                YearOfManufacture = machine.YearOfManufacture,
                Capacity = machine.Capacity,
                CapacityUnit = machine.CapacityUnit,
                Specifications = machine.Specifications,
                MaxWorkpieceLength = machine.MaxWorkpieceLength,
                MaxWorkpieceDiameter = machine.MaxWorkpieceDiameter,
                ChuckSize = machine.ChuckSize,
                Department = machine.Department,
                ShopFloor = machine.ShopFloor,
                Location = machine.Location,
                HourlyRate = machine.HourlyRate,
                PowerConsumption = machine.PowerConsumption,
                OperatorsRequired = machine.OperatorsRequired,
                PurchaseDate = machine.PurchaseDate,
                LastMaintenanceDate = machine.LastMaintenanceDate,
                NextMaintenanceDate = machine.NextMaintenanceDate,
                MaintenanceSchedule = machine.MaintenanceSchedule,
                IsActive = machine.IsActive,
                Status = machine.Status,
                CurrentJobCardNo = machine.CurrentJobCardNo,
                IsAvailable = machine.IsAvailable,
                AvailableFrom = machine.AvailableFrom,
                Remarks = machine.Remarks,
                CreatedAt = machine.CreatedAt,
                CreatedBy = machine.CreatedBy,
                UpdatedAt = machine.UpdatedAt,
                UpdatedBy = machine.UpdatedBy
            };
        }
    }
}
