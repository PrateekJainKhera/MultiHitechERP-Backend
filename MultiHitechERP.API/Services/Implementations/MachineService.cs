using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MachineService : IMachineService
    {
        private readonly IMachineRepository _machineRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IJobCardRepository _jobCardRepository;

        public MachineService(IMachineRepository machineRepository, IScheduleRepository scheduleRepository, IJobCardRepository jobCardRepository)
        {
            _machineRepository = machineRepository;
            _scheduleRepository = scheduleRepository;
            _jobCardRepository = jobCardRepository;
        }

        private async Task<MachineScheduleJobResponse> MapScheduleToJobResponseAsync(MachineSchedule s)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(s.JobCardId);
            return new MachineScheduleJobResponse
            {
                ScheduleId = s.Id,
                JobCardId = s.JobCardId,
                JobCardNo = s.JobCardNo ?? string.Empty,
                OrderNo = s.OrderNo,
                ItemSequence = jobCard?.ItemSequence,
                ProcessName = s.ProcessName,
                ChildPartName = jobCard?.ChildPartName,
                MachineModelName = jobCard?.MachineModelName,
                Quantity = jobCard?.Quantity ?? 0,
                ScheduledStartTime = s.ScheduledStartTime,
                ScheduledEndTime = s.ScheduledEndTime,
                ActualStartTime = s.ActualStartTime,
                ActualEndTime = s.ActualEndTime,
                Status = s.Status,
                FinishedEarly = s.ActualEndTime.HasValue && s.ActualEndTime.Value < s.ScheduledEndTime
            };
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
                var machine = new Machine
                {
                    MachineName = request.MachineName.Trim(),
                    MachineType = request.MachineType?.Trim(),
                    Location = request.Location?.Trim(),
                    Department = request.Department?.Trim(),
                    Status = request.Status?.Trim() ?? "Idle",
                    Notes = request.Notes?.Trim(),
                    DailyCapacityHours = request.DailyCapacityHours,
                    MaxLengthMM = request.MaxLengthMM,
                    IsActive = true,
                    CreatedBy = "System"
                };

                // MachineCode is backed by a UNIQUE constraint — a race between two
                // concurrent creates throws on insert instead of silently duplicating;
                // retry with the next number.
                int machineId = 0;
                for (int attempt = 1; ; attempt++)
                {
                    var machineCode = await _machineRepository.GetNextMachineCodeAsync();
                    if (attempt > 1)
                    {
                        var prefix = machineCode.Substring(0, machineCode.LastIndexOf('-') + 1);
                        var baseNum = int.Parse(machineCode.Substring(machineCode.LastIndexOf('-') + 1));
                        machineCode = $"{prefix}{baseNum + attempt - 1:D3}";
                    }
                    machine.MachineCode = machineCode;
                    try
                    {
                        machineId = await _machineRepository.InsertAsync(machine);
                        break;
                    }
                    catch (Exception ex) when (attempt < 10 &&
                        (ex.Message.Contains("UNIQUE KEY", StringComparison.OrdinalIgnoreCase)
                         || ex.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)))
                    {
                        // code was taken between read and insert — try the next number
                    }
                }

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
                existingMachine.MaxLengthMM = request.MaxLengthMM;
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

        public async Task<ApiResponse<IEnumerable<MachineUtilizationResponse>>> GetUtilizationAsync()
        {
            try
            {
                var machines = (await _machineRepository.GetAllAsync()).Where(m => m.IsActive).ToList();
                var now = DateTime.UtcNow;
                var todayStart = now.Date;
                var todayEnd = todayStart.AddDays(1);

                var result = new List<MachineUtilizationResponse>();
                foreach (var machine in machines)
                {
                    var todaySchedules = (await _scheduleRepository.GetByMachineAndDateRangeAsync(machine.Id, todayStart, todayEnd))
                        .Where(s => s.Status != "Cancelled")
                        .ToList();

                    // "Currently busy" is derived from real progress, not the original plan:
                    // a schedule with ActualEndTime already set is done — even if that's before
                    // its ScheduledEndTime — so it no longer counts as occupying the machine.
                    var current = todaySchedules.FirstOrDefault(s =>
                        s.ActualEndTime == null &&
                        now >= (s.ActualStartTime ?? s.ScheduledStartTime) &&
                        now <= s.ScheduledEndTime);

                    decimal usedMinutes = 0;
                    int completedCount = 0;
                    foreach (var s in todaySchedules)
                    {
                        if (s.Status == "Completed") completedCount++;

                        if (s.ActualEndTime.HasValue)
                        {
                            var start = s.ActualStartTime ?? s.ScheduledStartTime;
                            usedMinutes += (decimal)Math.Max(0, (s.ActualEndTime.Value - start).TotalMinutes);
                        }
                        else
                        {
                            var start = s.ActualStartTime ?? s.ScheduledStartTime;
                            if (now > start)
                            {
                                var end = now < s.ScheduledEndTime ? now : s.ScheduledEndTime;
                                usedMinutes += (decimal)Math.Max(0, (end - start).TotalMinutes);
                            }
                        }
                    }

                    var capacityMinutes = machine.DailyCapacityHours * 60m;
                    var utilizationPercent = capacityMinutes > 0
                        ? Math.Min(100m, Math.Round(usedMinutes / capacityMinutes * 100m, 1))
                        : 0m;

                    result.Add(new MachineUtilizationResponse
                    {
                        MachineId = machine.Id,
                        MachineCode = machine.MachineCode,
                        MachineName = machine.MachineName,
                        MachineType = machine.MachineType,
                        MachineStatus = machine.Status ?? "Idle",
                        IsCurrentlyBusy = current != null,
                        CurrentJobCardNo = current?.JobCardNo,
                        CurrentProcessName = current?.ProcessName,
                        CurrentJobExpectedFreeAt = current?.ScheduledEndTime,
                        UtilizationPercentToday = utilizationPercent,
                        ScheduledJobsToday = todaySchedules.Count,
                        CompletedJobsToday = completedCount
                    });
                }

                return ApiResponse<IEnumerable<MachineUtilizationResponse>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineUtilizationResponse>>.ErrorResponse($"Error computing machine utilization: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineScheduleJobResponse>>> GetMachineJobsAsync(int machineId, DateTime? date)
        {
            try
            {
                var day = (date ?? DateTime.UtcNow).Date;
                var scheduleRows = (await _scheduleRepository.GetByMachineAndDateRangeAsync(machineId, day, day.AddDays(1)))
                    .Where(s => s.Status != "Cancelled")
                    .OrderBy(s => s.ScheduledStartTime)
                    .ToList();

                var schedules = new List<MachineScheduleJobResponse>();
                foreach (var s in scheduleRows)
                    schedules.Add(await MapScheduleToJobResponseAsync(s));

                return ApiResponse<IEnumerable<MachineScheduleJobResponse>>.SuccessResponse(schedules);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineScheduleJobResponse>>.ErrorResponse($"Error retrieving machine jobs: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MachineDailyScheduleResponse>>> GetDailyScheduleAsync(DateTime? date)
        {
            try
            {
                var day = (date ?? DateTime.UtcNow).Date;
                var machines = (await _machineRepository.GetAllAsync()).Where(m => m.IsActive).ToList();

                var allSchedules = (await _scheduleRepository.GetByDateRangeAsync(day, day.AddDays(1)))
                    .Where(s => s.Status != "Cancelled" && s.MachineId.HasValue)
                    .ToList();

                var byMachine = allSchedules.ToLookup(s => s.MachineId!.Value);

                var result = new List<MachineDailyScheduleResponse>();
                foreach (var machine in machines)
                {
                    var jobs = new List<MachineScheduleJobResponse>();
                    foreach (var s in byMachine[machine.Id].OrderBy(s => s.ScheduledStartTime))
                        jobs.Add(await MapScheduleToJobResponseAsync(s));

                    result.Add(new MachineDailyScheduleResponse
                    {
                        MachineId = machine.Id,
                        MachineCode = machine.MachineCode,
                        MachineName = machine.MachineName,
                        MachineType = machine.MachineType,
                        Jobs = jobs
                    });
                }

                return ApiResponse<IEnumerable<MachineDailyScheduleResponse>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineDailyScheduleResponse>>.ErrorResponse($"Error loading daily schedule: {ex.Message}");
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
                MaxLengthMM = machine.MaxLengthMM,
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
