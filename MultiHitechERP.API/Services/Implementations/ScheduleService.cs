using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IProcessMachineCapabilityRepository _capabilityRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IProcessRepository _processRepository;

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IJobCardRepository jobCardRepository,
            IProcessMachineCapabilityRepository capabilityRepository,
            IMachineRepository machineRepository,
            IProcessRepository processRepository)
        {
            _scheduleRepository = scheduleRepository;
            _jobCardRepository = jobCardRepository;
            _capabilityRepository = capabilityRepository;
            _machineRepository = machineRepository;
            _processRepository = processRepository;
        }

        public async Task<ApiResponse<ScheduleResponse>> GetByIdAsync(int id)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                    return ApiResponse<ScheduleResponse>.ErrorResponse($"Schedule with ID {id} not found");

                return ApiResponse<ScheduleResponse>.SuccessResponse(MapToResponse(schedule));
            }
            catch (Exception ex)
            {
                return ApiResponse<ScheduleResponse>.ErrorResponse($"Error retrieving schedule: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetAllAsync()
        {
            try
            {
                var schedules = await _scheduleRepository.GetAllAsync();
                var responses = schedules.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ScheduleResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ScheduleResponse>>.ErrorResponse($"Error retrieving schedules: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByMachineIdAsync(int machineId)
        {
            try
            {
                var schedules = await _scheduleRepository.GetByMachineIdAsync(machineId);
                var responses = schedules.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ScheduleResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ScheduleResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByJobCardIdAsync(int jobCardId)
        {
            try
            {
                var schedules = await _scheduleRepository.GetByJobCardIdAsync(jobCardId);
                var responses = schedules.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ScheduleResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ScheduleResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var schedules = await _scheduleRepository.GetByDateRangeAsync(startDate, endDate);
                var responses = schedules.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ScheduleResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ScheduleResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get the full scheduling tree for an order: Order → ChildPart groups → Process steps
        /// Shows which steps have machines assigned and which are still pending
        /// </summary>
        public async Task<ApiResponse<OrderSchedulingTreeResponse>> GetOrderSchedulingTreeAsync(int orderId)
        {
            try
            {
                // Get all job cards for this order
                var jobCards = await _jobCardRepository.GetByOrderIdAsync(orderId);
                var jobCardList = jobCards.ToList();

                if (!jobCardList.Any())
                    return ApiResponse<OrderSchedulingTreeResponse>.ErrorResponse($"No job cards found for order {orderId}");

                var orderNo = jobCardList[0].OrderNo ?? $"Order-{orderId}";
                var priority = jobCardList[0].Priority ?? "MEDIUM";

                // Fetch IsOutsourced flag for each unique process used by these job cards
                var uniqueProcessIds = jobCardList.Select(jc => jc.ProcessId).Distinct().ToList();
                var ospProcessIds = new HashSet<int>();
                foreach (var pid in uniqueProcessIds)
                {
                    var process = await _processRepository.GetByIdAsync(pid);
                    if (process != null && process.IsOutsourced)
                        ospProcessIds.Add(pid);
                }

                // For each job card, get its existing machine schedule (first active one)
                var stepItems = new List<ProcessStepSchedulingItem>();
                foreach (var jc in jobCardList)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    var activeSchedule = schedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();

                    stepItems.Add(new ProcessStepSchedulingItem
                    {
                        JobCardId = jc.Id,
                        JobCardNo = jc.JobCardNo,
                        ProcessId = jc.ProcessId,
                        ProcessName = jc.ProcessName,
                        ProcessCode = jc.ProcessCode,
                        StepNo = jc.StepNo,
                        IsOsp = ospProcessIds.Contains(jc.ProcessId),
                        Quantity = jc.Quantity,
                        Priority = jc.Priority,
                        JobCardStatus = jc.Status,
                        // Machine assignment from existing schedule
                        ScheduleId = activeSchedule?.Id,
                        AssignedMachineId = activeSchedule?.MachineId,
                        AssignedMachineCode = activeSchedule?.MachineCode,
                        AssignedMachineName = activeSchedule?.MachineName,
                        ScheduledStartTime = activeSchedule?.ScheduledStartTime,
                        ScheduledEndTime = activeSchedule?.ScheduledEndTime,
                        ScheduleStatus = activeSchedule?.Status,
                        EstimatedDurationMinutes = activeSchedule?.EstimatedDurationMinutes
                    });
                }

                // Group by child part name; assembly (CreationType = "Assembly") goes last
                var groups = new List<ChildPartGroupResponse>();

                // Child part groups
                var childPartGroups = jobCardList
                    .Where(jc => jc.CreationType != "Assembly")
                    .GroupBy(jc => jc.ChildPartName ?? "Unknown Part")
                    .OrderBy(g => g.Key);

                foreach (var group in childPartGroups)
                {
                    var groupSteps = stepItems
                        .Where(s => group.Any(jc => jc.Id == s.JobCardId))
                        .OrderBy(s => s.StepNo ?? 999)
                        .ToList();

                    groups.Add(new ChildPartGroupResponse
                    {
                        GroupName = group.Key,
                        CreationType = "ChildPart",
                        TotalSteps = groupSteps.Count,
                        ScheduledSteps = groupSteps.Count(s => s.ScheduleId.HasValue),
                        Steps = groupSteps
                    });
                }

                // Assembly groups last
                var assemblyGroups = jobCardList
                    .Where(jc => jc.CreationType == "Assembly")
                    .GroupBy(jc => jc.ChildPartName ?? "Assembly")
                    .OrderBy(g => g.Key);

                foreach (var group in assemblyGroups)
                {
                    var groupSteps = stepItems
                        .Where(s => group.Any(jc => jc.Id == s.JobCardId))
                        .OrderBy(s => s.StepNo ?? 999)
                        .ToList();

                    groups.Add(new ChildPartGroupResponse
                    {
                        GroupName = group.Key,
                        CreationType = "Assembly",
                        TotalSteps = groupSteps.Count,
                        ScheduledSteps = groupSteps.Count(s => s.ScheduleId.HasValue),
                        Steps = groupSteps
                    });
                }

                var totalSteps = stepItems.Count;
                var scheduledSteps = stepItems.Count(s => s.ScheduleId.HasValue);

                var tree = new OrderSchedulingTreeResponse
                {
                    OrderId = orderId,
                    OrderNo = orderNo,
                    Priority = priority,
                    TotalSteps = totalSteps,
                    ScheduledSteps = scheduledSteps,
                    PendingSteps = totalSteps - scheduledSteps,
                    Groups = groups
                };

                return ApiResponse<OrderSchedulingTreeResponse>.SuccessResponse(tree);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderSchedulingTreeResponse>.ErrorResponse($"Error building scheduling tree: {ex.Message}");
            }
        }

        /// <summary>
        /// SEMI-AUTOMATIC SCHEDULING: Get intelligent machine suggestions for a job card
        /// </summary>
        public async Task<ApiResponse<IEnumerable<MachineSuggestionResponse>>> GetMachineSuggestionsAsync(int jobCardId)
        {
            try
            {
                // Get job card details
                var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
                if (jobCard == null)
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse("Job card not found");

                // Get all capable machines for this process
                var capabilities = await _capabilityRepository.GetByProcessIdAsync(jobCard.ProcessId);
                if (capabilities == null || !capabilities.Any())
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse("No machines capable of performing this process");

                var suggestions = new List<MachineSuggestionResponse>();

                foreach (var capability in capabilities.Where(c => c.IsActive))
                {
                    // Get machine details
                    var machine = await _machineRepository.GetByIdAsync(capability.MachineId);
                    if (machine == null || !machine.IsActive) continue;

                    // Calculate estimated times
                    var setupMinutes = (int)(capability.SetupTimeHours * 60);
                    var cycleMinutes = (int)(capability.CycleTimePerPieceHours * jobCard.Quantity * 60);
                    var totalMinutes = setupMinutes + cycleMinutes;

                    // Get existing schedules for this machine
                    var existingSchedules = await _scheduleRepository.GetByMachineIdAsync(capability.MachineId);
                    var activeSchedules = existingSchedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderBy(s => s.ScheduledStartTime)
                        .ToList();

                    // Find next available time slot
                    var now = DateTime.UtcNow;
                    var nextAvailableStart = now;

                    if (activeSchedules.Any())
                    {
                        // Find the latest end time
                        var lastSchedule = activeSchedules.OrderByDescending(s => s.ScheduledEndTime).First();
                        nextAvailableStart = lastSchedule.ScheduledEndTime > now
                            ? lastSchedule.ScheduledEndTime
                            : now;
                    }

                    var suggestedEnd = nextAvailableStart.AddMinutes(totalMinutes);

                    // Create suggestion
                    var suggestion = new MachineSuggestionResponse
                    {
                        MachineId = capability.MachineId,
                        MachineCode = machine.MachineCode,
                        MachineName = machine.MachineName,
                        SetupTimeHours = capability.SetupTimeHours,
                        CycleTimePerPieceHours = capability.CycleTimePerPieceHours,
                        PreferenceLevel = capability.PreferenceLevel,
                        EfficiencyRating = capability.EfficiencyRating,
                        IsPreferredMachine = capability.IsPreferredMachine,
                        EstimatedSetupMinutes = setupMinutes,
                        EstimatedCycleMinutes = cycleMinutes,
                        TotalEstimatedMinutes = totalMinutes,
                        NextAvailableStart = nextAvailableStart,
                        SuggestedStart = nextAvailableStart,
                        SuggestedEnd = suggestedEnd,
                        IsCurrentlyAvailable = nextAvailableStart <= now.AddMinutes(5), // Available within 5 minutes
                        ScheduledJobsCount = activeSchedules.Count,
                        CurrentStatus = machine.Status,
                        UpcomingSchedules = activeSchedules.Take(5).Select(s => new ScheduleSlot
                        {
                            ScheduleId = s.Id,
                            JobCardNo = s.JobCardNo ?? "N/A",
                            StartTime = s.ScheduledStartTime,
                            EndTime = s.ScheduledEndTime,
                            Status = s.Status
                        }).ToList()
                    };

                    suggestions.Add(suggestion);
                }

                // Sort suggestions: Preferred machines first, then by preference level, then by next available time
                var sortedSuggestions = suggestions
                    .OrderBy(s => s.IsPreferredMachine ? 0 : 1)
                    .ThenBy(s => s.PreferenceLevel)
                    .ThenBy(s => s.NextAvailableStart)
                    .ToList();

                return ApiResponse<IEnumerable<MachineSuggestionResponse>>.SuccessResponse(sortedSuggestions);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse($"Error generating suggestions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateScheduleAsync(CreateScheduleRequest request)
        {
            try
            {
                // Get job card
                var jobCard = await _jobCardRepository.GetByIdAsync(request.JobCardId);
                if (jobCard == null)
                    return ApiResponse<int>.ErrorResponse("Job card not found");

                string machineCode = "OSP";
                string machineName = "Outside Service Process";

                if (!request.IsOsp)
                {
                    // Get machine
                    var machine = await _machineRepository.GetByIdAsync(request.MachineId);
                    if (machine == null)
                        return ApiResponse<int>.ErrorResponse("Machine not found");

                    machineCode = machine.MachineCode;
                    machineName = machine.MachineName;

                    // Check for scheduling conflicts (not needed for OSP)
                    var hasConflict = await _scheduleRepository.HasConflictAsync(
                        request.MachineId,
                        request.ScheduledStartTime,
                        request.ScheduledEndTime
                    );

                    if (hasConflict)
                        return ApiResponse<int>.ErrorResponse("Schedule conflict: Machine already has overlapping schedule for this time period");
                }

                // Create schedule
                var schedule = new MachineSchedule
                {
                    JobCardId = request.JobCardId,
                    JobCardNo = jobCard.JobCardNo,
                    MachineId = request.IsOsp ? (int?)null : request.MachineId,
                    MachineCode = machineCode,
                    MachineName = machineName,
                    ScheduledStartTime = request.ScheduledStartTime,
                    ScheduledEndTime = request.ScheduledEndTime,
                    EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                    Status = "Scheduled",
                    SchedulingMethod = request.IsOsp ? "OSP" : (request.SchedulingMethod ?? "Semi-Automatic"),
                    SuggestedBySystem = request.SuggestedBySystem,
                    ConfirmedBy = request.CreatedBy,
                    ConfirmedAt = DateTime.UtcNow,
                    ProcessId = jobCard.ProcessId,
                    ProcessName = jobCard.ProcessName,
                    ProcessCode = jobCard.ProcessCode,
                    Notes = request.Notes,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                var scheduleId = await _scheduleRepository.InsertAsync(schedule);

                // Update job card status (you'll need to implement this in JobCardRepository)
                // await _jobCardRepository.UpdateScheduleStatusAsync(request.JobCardId, "SCHEDULED", request.MachineId);

                return ApiResponse<int>.SuccessResponse(scheduleId, "Schedule created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating schedule: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateScheduleAsync(int id, CreateScheduleRequest request)
        {
            try
            {
                var existingSchedule = await _scheduleRepository.GetByIdAsync(id);
                if (existingSchedule == null)
                    return ApiResponse<bool>.ErrorResponse("Schedule not found");

                // Check for conflicts (excluding current schedule)
                var hasConflict = await _scheduleRepository.HasConflictAsync(
                    request.MachineId,
                    request.ScheduledStartTime,
                    request.ScheduledEndTime,
                    id
                );

                if (hasConflict)
                    return ApiResponse<bool>.ErrorResponse("Schedule conflict detected");

                existingSchedule.ScheduledStartTime = request.ScheduledStartTime;
                existingSchedule.ScheduledEndTime = request.ScheduledEndTime;
                existingSchedule.EstimatedDurationMinutes = request.EstimatedDurationMinutes;
                existingSchedule.Notes = request.Notes;
                existingSchedule.UpdatedBy = request.CreatedBy ?? "System";

                var success = await _scheduleRepository.UpdateAsync(existingSchedule);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Schedule updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update schedule");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating schedule: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteScheduleAsync(int id)
        {
            try
            {
                var schedule = await _scheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                    return ApiResponse<bool>.ErrorResponse("Schedule not found");

                var success = await _scheduleRepository.DeleteAsync(id);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Schedule deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete schedule");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting schedule: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status, string? updatedBy = null)
        {
            try
            {
                var success = await _scheduleRepository.UpdateStatusAsync(id, status, updatedBy);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Status updated to {status}")
                    : ApiResponse<bool>.ErrorResponse("Failed to update status");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RescheduleAsync(int scheduleId, DateTime newStartTime, DateTime newEndTime, string reason, string? rescheduledBy = null)
        {
            try
            {
                var existingSchedule = await _scheduleRepository.GetByIdAsync(scheduleId);
                if (existingSchedule == null)
                    return ApiResponse<bool>.ErrorResponse("Schedule not found");

                // Check for conflicts (skip for OSP schedules that have no machine)
                if (existingSchedule.MachineId.HasValue)
                {
                    var hasConflict = await _scheduleRepository.HasConflictAsync(
                        existingSchedule.MachineId.Value,
                        newStartTime,
                        newEndTime,
                        scheduleId
                    );

                    if (hasConflict)
                        return ApiResponse<bool>.ErrorResponse("Cannot reschedule: Conflict detected");
                }

                existingSchedule.ScheduledStartTime = newStartTime;
                existingSchedule.ScheduledEndTime = newEndTime;
                existingSchedule.IsRescheduled = true;
                existingSchedule.RescheduledReason = reason;
                existingSchedule.RescheduledAt = DateTime.UtcNow;
                existingSchedule.RescheduledBy = rescheduledBy ?? "System";

                var success = await _scheduleRepository.UpdateAsync(existingSchedule);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Rescheduled successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to reschedule");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error rescheduling: {ex.Message}");
            }
        }

        private static ScheduleResponse MapToResponse(MachineSchedule schedule)
        {
            return new ScheduleResponse
            {
                Id = schedule.Id,
                JobCardId = schedule.JobCardId,
                JobCardNo = schedule.JobCardNo,
                MachineId = schedule.MachineId,
                MachineCode = schedule.MachineCode,
                MachineName = schedule.MachineName,
                ScheduledStartTime = schedule.ScheduledStartTime,
                ScheduledEndTime = schedule.ScheduledEndTime,
                EstimatedDurationMinutes = schedule.EstimatedDurationMinutes,
                ActualStartTime = schedule.ActualStartTime,
                ActualEndTime = schedule.ActualEndTime,
                ActualDurationMinutes = schedule.ActualDurationMinutes,
                Status = schedule.Status,
                SchedulingMethod = schedule.SchedulingMethod,
                SuggestedBySystem = schedule.SuggestedBySystem,
                ConfirmedBy = schedule.ConfirmedBy,
                ConfirmedAt = schedule.ConfirmedAt,
                ProcessId = schedule.ProcessId,
                ProcessName = schedule.ProcessName,
                ProcessCode = schedule.ProcessCode,
                IsRescheduled = schedule.IsRescheduled,
                RescheduledFromId = schedule.RescheduledFromId,
                RescheduledReason = schedule.RescheduledReason,
                Notes = schedule.Notes,
                CreatedAt = schedule.CreatedAt,
                CreatedBy = schedule.CreatedBy,
                UpdatedAt = schedule.UpdatedAt,
                UpdatedBy = schedule.UpdatedBy
            };
        }
    }
}
