using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Planning;
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

                // Fetch IsOutsourced + IsManual flags for each unique process
                var uniqueProcessIds = jobCardList.Select(jc => jc.ProcessId).Distinct().ToList();
                var ospProcessIds = new HashSet<int>();
                var manualProcessIds = new HashSet<int>();
                foreach (var pid in uniqueProcessIds)
                {
                    var process = await _processRepository.GetByIdAsync(pid);
                    if (process == null) continue;
                    if (process.IsOutsourced) ospProcessIds.Add(pid);
                    if (process.IsManual) manualProcessIds.Add(pid);
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
                        IsManual = manualProcessIds.Contains(jc.ProcessId),
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
        /// Get the full scheduling tree for an ORDER ITEM: OrderItem → ChildPart groups → Process steps
        /// For multi-product orders (schedules specific order item, e.g., ORD-007-A)
        /// </summary>
        public async Task<ApiResponse<OrderSchedulingTreeResponse>> GetOrderItemSchedulingTreeAsync(int orderItemId)
        {
            try
            {
                // Get all job cards for this order item
                var jobCards = await _jobCardRepository.GetByOrderItemIdAsync(orderItemId);
                var jobCardList = jobCards.ToList();

                if (!jobCardList.Any())
                    return ApiResponse<OrderSchedulingTreeResponse>.ErrorResponse($"No job cards found for order item {orderItemId}");

                var orderNo = jobCardList[0].OrderNo ?? "Unknown Order";
                var itemSequence = jobCardList[0].ItemSequence ?? "";
                var fullOrderRef = orderNo + (string.IsNullOrEmpty(itemSequence) ? "" : $"-{itemSequence}");
                var priority = jobCardList[0].Priority ?? "MEDIUM";

                // Fetch IsOutsourced + IsManual flags for each unique process
                var uniqueProcessIds = jobCardList.Select(jc => jc.ProcessId).Distinct().ToList();
                var ospProcessIds = new HashSet<int>();
                var manualProcessIds = new HashSet<int>();
                foreach (var pid in uniqueProcessIds)
                {
                    var process = await _processRepository.GetByIdAsync(pid);
                    if (process == null) continue;
                    if (process.IsOutsourced) ospProcessIds.Add(pid);
                    if (process.IsManual) manualProcessIds.Add(pid);
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
                        IsManual = manualProcessIds.Contains(jc.ProcessId),
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
                    OrderId = jobCardList[0].OrderId,
                    OrderNo = fullOrderRef, // Includes item sequence (e.g., "ORD-007-A")
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
                return ApiResponse<OrderSchedulingTreeResponse>.ErrorResponse($"Error building order item scheduling tree: {ex.Message}");
            }
        }

        /// <summary>
        /// CAPACITY-BASED SCHEDULING: Get machine suggestions based on daily capacity and utilization
        /// Uses new Process Category system to find capable machines
        /// </summary>
        public async Task<ApiResponse<IEnumerable<MachineSuggestionResponse>>> GetMachineSuggestionsAsync(int jobCardId)
        {
            try
            {
                // Get job card details
                var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
                if (jobCard == null)
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse("Job card not found");

                // Get process details to find the process category
                var process = await _processRepository.GetByIdAsync(jobCard.ProcessId);
                if (process == null)
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse("Process not found");

                if (!process.ProcessCategoryId.HasValue)
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse($"Process '{process.ProcessName}' has no Process Category assigned. Please assign a category first.");

                // Get all machines that can perform this process category
                var machines = await _machineRepository.GetByProcessCategoryIdAsync(process.ProcessCategoryId.Value);
                if (machines == null || !machines.Any())
                    return ApiResponse<IEnumerable<MachineSuggestionResponse>>.ErrorResponse($"No machines found with Process Category '{process.ProcessCategoryName}'");

                var suggestions = new List<MachineSuggestionResponse>();
                var targetDate = DateTime.Today; // Can be parameterized later

                foreach (var machine in machines.Where(m => m.IsActive))
                {
                    // Get all schedules for this machine on the target date
                    var allSchedules = await _scheduleRepository.GetByMachineIdAsync(machine.Id);
                    var todaySchedules = allSchedules
                        .Where(s => s.ScheduledStartTime.Date == targetDate &&
                                   (s.Status == "Scheduled" || s.Status == "InProgress" || s.Status == "Completed"))
                        .ToList();

                    // Calculate scheduled hours for today
                    decimal scheduledHours = 0;
                    var scheduledJobCardNumbers = new List<string>();

                    foreach (var schedule in todaySchedules)
                    {
                        var duration = (schedule.ScheduledEndTime - schedule.ScheduledStartTime).TotalHours;
                        scheduledHours += (decimal)duration;
                        if (!string.IsNullOrEmpty(schedule.JobCardNo) && !scheduledJobCardNumbers.Contains(schedule.JobCardNo))
                        {
                            scheduledJobCardNumbers.Add(schedule.JobCardNo);
                        }
                    }

                    // Calculate capacity metrics
                    var dailyCapacity = machine.DailyCapacityHours;
                    var availableHours = Math.Max(0, dailyCapacity - scheduledHours);
                    var utilizationPercent = dailyCapacity > 0 ? Math.Round((scheduledHours / dailyCapacity) * 100, 1) : 0;

                    // Determine capacity status
                    string capacityStatus;
                    bool isBusy;

                    if (utilizationPercent >= 100)
                    {
                        capacityStatus = "Overloaded";
                        isBusy = true;
                    }
                    else if (utilizationPercent >= 90)
                    {
                        capacityStatus = "Busy";
                        isBusy = true;
                    }
                    else if (utilizationPercent >= 70)
                    {
                        capacityStatus = "Moderate";
                        isBusy = false;
                    }
                    else
                    {
                        capacityStatus = "Available";
                        isBusy = false;
                    }

                    // Get process category name (already loaded by repository)
                    var processCategoryName = machine.ProcessCategoryNames.FirstOrDefault() ?? process.ProcessCategoryName ?? "Unknown";

                    var suggestion = new MachineSuggestionResponse
                    {
                        MachineId = machine.Id,
                        MachineCode = machine.MachineCode,
                        MachineName = machine.MachineName,
                        MachineType = machine.MachineType,
                        Location = machine.Location,
                        Department = machine.Department,
                        ProcessCategoryId = process.ProcessCategoryId.Value,
                        ProcessCategoryName = processCategoryName,
                        DailyCapacityHours = dailyCapacity,
                        ScheduledHours = Math.Round(scheduledHours, 2),
                        AvailableHours = Math.Round(availableHours, 2),
                        UtilizationPercent = utilizationPercent,
                        CapacityStatus = capacityStatus,
                        IsBusy = isBusy,
                        TotalJobCards = scheduledJobCardNumbers.Count,
                        ScheduledJobCardNumbers = scheduledJobCardNumbers
                    };

                    suggestions.Add(suggestion);
                }

                // Sort suggestions: Lowest utilization first (best available machines at top)
                var sortedSuggestions = suggestions
                    .OrderBy(s => s.UtilizationPercent)
                    .ThenByDescending(s => s.AvailableHours)
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

                bool noMachine = request.IsOsp || request.IsManual;

                string machineCode = request.IsOsp ? "OSP" : "MANUAL";
                string machineName = request.IsOsp ? "Outside Service Process" : "Manual Process";

                if (!noMachine)
                {
                    // Get machine
                    var machine = await _machineRepository.GetByIdAsync(request.MachineId);
                    if (machine == null)
                        return ApiResponse<int>.ErrorResponse("Machine not found");

                    machineCode = machine.MachineCode;
                    machineName = machine.MachineName;

                    // Check for scheduling conflicts (not needed for OSP/Manual)
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
                    OrderId = jobCard.OrderId,
                    OrderNo = jobCard.OrderNo,
                    OrderItemId = jobCard.OrderItemId,
                    ItemSequence = jobCard.ItemSequence,
                    MachineId = noMachine ? (int?)null : request.MachineId,
                    MachineCode = machineCode,
                    MachineName = machineName,
                    ScheduledStartTime = request.ScheduledStartTime,
                    ScheduledEndTime = request.ScheduledEndTime,
                    EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                    Status = "Scheduled",
                    SchedulingMethod = request.IsOsp ? "OSP" : request.IsManual ? "Manual" : (request.SchedulingMethod ?? "Semi-Automatic"),
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

                // Mark job card as Scheduled
                await _jobCardRepository.UpdateStatusAsync(request.JobCardId, "Scheduled");

                // Determine ProductionStatus for this job card:
                // - Step 1 of a child part → Ready (can start immediately)
                // - Step N>1 → Ready only if the previous step is already Completed
                // - Assembly → stays Pending (cascade will set Ready when all parts done)
                string productionStatus = "Pending";
                if (jobCard.CreationType != "Assembly")
                {
                    if (jobCard.StepNo == 1)
                    {
                        productionStatus = "Ready";
                    }
                    else
                    {
                        // Check if previous step in same child part is already Completed
                        var siblingCards = (await _jobCardRepository.GetByOrderIdAsync(jobCard.OrderId)).ToList();
                        var prevStep = siblingCards
                            .Where(jc =>
                                jc.CreationType != "Assembly" &&
                                (jc.ChildPartId.HasValue
                                    ? jc.ChildPartId == jobCard.ChildPartId
                                    : (jc.ChildPartName ?? "") == (jobCard.ChildPartName ?? "")) &&
                                jc.StepNo == jobCard.StepNo - 1)
                            .FirstOrDefault();

                        if (prevStep != null && prevStep.ProductionStatus == "Completed")
                            productionStatus = "Ready";
                    }
                }

                await _jobCardRepository.UpdateProductionStatusAsync(
                    request.JobCardId, productionStatus,
                    jobCard.ActualStartTime, jobCard.ActualEndTime,
                    jobCard.CompletedQty, jobCard.RejectedQty
                );

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

        // ── Child-Part-First Batch Scheduling ──────────────────────────────────

        /// <summary>
        /// Returns all orders that have at least one PLANNED job card (material issued, machine not yet assigned).
        /// These are the orders the user can select for batch scheduling.
        /// </summary>
        public async Task<ApiResponse<IEnumerable<SchedulableOrderResponse>>> GetSchedulableOrdersAsync()
        {
            try
            {
                var allJobCards = await _jobCardRepository.GetAllAsync();

                // Job cards in scheduling queue: status Scheduled or PLANNED
                // "Scheduled" = released to scheduling module; "PLANNED" = planned but not yet released
                // Machine assignment is tracked separately in Scheduling_MachineSchedules
                var schedulingCards = allJobCards
                    .Where(jc => jc.Status == "Scheduled" || jc.Status == "PLANNED" || jc.Status == "Planned")
                    .ToList();

                if (!schedulingCards.Any())
                    return ApiResponse<IEnumerable<SchedulableOrderResponse>>.SuccessResponse(
                        Enumerable.Empty<SchedulableOrderResponse>(), "No schedulable orders found");

                // Find which job cards already have an active machine schedule
                var jobCardIdsWithSchedule = new HashSet<int>();
                foreach (var jc in schedulingCards)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    if (schedules.Any(s => s.Status == "Scheduled" || s.Status == "InProgress"))
                        jobCardIdsWithSchedule.Add(jc.Id);
                }

                // Only include job cards that don't yet have a machine assigned
                var unassignedCards = schedulingCards
                    .Where(jc => !jobCardIdsWithSchedule.Contains(jc.Id))
                    .ToList();

                if (!unassignedCards.Any())
                    return ApiResponse<IEnumerable<SchedulableOrderResponse>>.SuccessResponse(
                        Enumerable.Empty<SchedulableOrderResponse>(), "No schedulable orders found");

                // Group by orderId
                var grouped = unassignedCards
                    .GroupBy(jc => jc.OrderId)
                    .Select(g =>
                    {
                        var first = g.First();
                        var totalForOrder = allJobCards.Count(jc => jc.OrderId == g.Key);
                        return new SchedulableOrderResponse
                        {
                            OrderId = g.Key,
                            OrderNo = first.OrderNo ?? $"ORD-{g.Key}",
                            CustomerName = null,
                            DueDate = null,
                            Priority = first.Priority ?? "MEDIUM",
                            PendingJobCardCount = g.Count(),
                            TotalJobCardCount = totalForOrder
                        };
                    })
                    .OrderBy(o => o.OrderNo)
                    .ToList();

                return ApiResponse<IEnumerable<SchedulableOrderResponse>>.SuccessResponse(grouped);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<SchedulableOrderResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cross-order child-part view: takes selected order IDs and returns PLANNED job cards
        /// grouped by ChildPartName → StepNo, showing all orders that need each step.
        /// </summary>
        public async Task<ApiResponse<CrossOrderGroupsResponse>> GetCrossOrderGroupsAsync(IEnumerable<int> orderIds)
        {
            try
            {
                var orderIdList = orderIds.ToList();
                if (!orderIdList.Any())
                    return ApiResponse<CrossOrderGroupsResponse>.ErrorResponse("No orders specified");

                // Collect job cards in scheduling queue for all selected orders
                var allCards = new List<MultiHitechERP.API.Models.Planning.JobCard>();
                foreach (var orderId in orderIdList)
                {
                    var cards = await _jobCardRepository.GetByOrderIdAsync(orderId);
                    allCards.AddRange(cards.Where(jc =>
                        jc.Status == "Scheduled" || jc.Status == "PLANNED" || jc.Status == "Planned"));
                }

                if (!allCards.Any())
                    return ApiResponse<CrossOrderGroupsResponse>.SuccessResponse(
                        new CrossOrderGroupsResponse(), "No PLANNED job cards in selected orders");

                // Fetch process flags (OSP/Manual) and category info
                var uniqueProcessIds = allCards.Select(jc => jc.ProcessId).Distinct().ToList();
                var processInfoCache = new Dictionary<int, (bool isOsp, bool isManual, int? catId, string? catName)>();
                foreach (var pid in uniqueProcessIds)
                {
                    var proc = await _processRepository.GetByIdAsync(pid);
                    processInfoCache[pid] = proc == null
                        ? (false, false, null, null)
                        : (proc.IsOutsourced, proc.IsManual, proc.ProcessCategoryId, proc.ProcessCategoryName);
                }

                // For each job card, check if it already has a machine schedule
                var scheduleCache = new Dictionary<int, ProcessStepSchedulingItem>();
                foreach (var jc in allCards)
                {
                    var schedules = await _scheduleRepository.GetByJobCardIdAsync(jc.Id);
                    var active = schedules
                        .Where(s => s.Status == "Scheduled" || s.Status == "InProgress")
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();

                    scheduleCache[jc.Id] = new ProcessStepSchedulingItem
                    {
                        JobCardId = jc.Id,
                        JobCardNo = jc.JobCardNo,
                        ProcessId = jc.ProcessId,
                        ProcessName = jc.ProcessName,
                        ProcessCode = jc.ProcessCode,
                        StepNo = jc.StepNo,
                        Quantity = jc.Quantity,
                        Priority = jc.Priority ?? "MEDIUM",
                        JobCardStatus = jc.Status,
                        ScheduleId = active?.Id,
                        AssignedMachineId = active?.MachineId,
                        AssignedMachineCode = active?.MachineCode,
                        AssignedMachineName = active?.MachineName,
                        ScheduledStartTime = active?.ScheduledStartTime,
                        ScheduledEndTime = active?.ScheduledEndTime
                    };
                }

                // Group: ChildPartName → StepNo
                var childPartGroups = allCards
                    .GroupBy(jc => new { PartName = jc.ChildPartName ?? "Unknown Part", jc.CreationType })
                    .OrderBy(g => g.Key.CreationType == "Assembly" ? 1 : 0)
                    .ThenBy(g => g.Key.PartName)
                    .Select(partGroup =>
                    {
                        var steps = partGroup
                            .GroupBy(jc => jc.StepNo ?? 0)
                            .OrderBy(sg => sg.Key)
                            .Select(stepGroup =>
                            {
                                var firstInStep = stepGroup.First();
                                var (isOsp, isManual, catId, catName) =
                                    processInfoCache.GetValueOrDefault(firstInStep.ProcessId,
                                        (false, false, null, null));

                                return new CrossOrderProcessStep
                                {
                                    StepNo = stepGroup.Key,
                                    ProcessId = firstInStep.ProcessId,
                                    ProcessName = firstInStep.ProcessName ?? "Unknown Process",
                                    ProcessCode = firstInStep.ProcessCode,
                                    IsOsp = isOsp,
                                    IsManual = isManual,
                                    ProcessCategoryId = catId,
                                    ProcessCategoryName = catName,
                                    JobCards = stepGroup.Select(jc =>
                                    {
                                        var si = scheduleCache[jc.Id];
                                        return new CrossOrderJobCardItem
                                        {
                                            JobCardId = jc.Id,
                                            JobCardNo = jc.JobCardNo,
                                            OrderId = jc.OrderId,
                                            OrderNo = jc.OrderNo ?? $"ORD-{jc.OrderId}",
                                            Quantity = jc.Quantity,
                                            Priority = jc.Priority ?? "MEDIUM",
                                            ScheduleId = si.ScheduleId,
                                            AssignedMachineName = si.AssignedMachineName,
                                            ScheduledStartTime = si.ScheduledStartTime,
                                            ScheduledEndTime = si.ScheduledEndTime
                                        };
                                    }).ToList()
                                };
                            }).ToList();

                        return new CrossOrderChildPartGroup
                        {
                            ChildPartName = partGroup.Key.PartName,
                            CreationType = partGroup.Key.CreationType ?? "ChildPart",
                            Steps = steps
                        };
                    }).ToList();

                return ApiResponse<CrossOrderGroupsResponse>.SuccessResponse(
                    new CrossOrderGroupsResponse { ChildParts = childPartGroups });
            }
            catch (Exception ex)
            {
                return ApiResponse<CrossOrderGroupsResponse>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Batch-create machine schedules: creates one schedule per request entry.
        /// Returns per-job-card success/failure results.
        /// </summary>
        public async Task<ApiResponse<IEnumerable<BatchScheduleResult>>> BatchCreateSchedulesAsync(
            IEnumerable<CreateScheduleRequest> requests)
        {
            var results = new List<BatchScheduleResult>();
            foreach (var req in requests)
            {
                try
                {
                    // Re-use existing CreateScheduleAsync for each job card
                    var result = await CreateScheduleAsync(req);
                    var jc = await _jobCardRepository.GetByIdAsync(req.JobCardId);
                    results.Add(new BatchScheduleResult
                    {
                        JobCardId = req.JobCardId,
                        JobCardNo = jc?.JobCardNo ?? req.JobCardId.ToString(),
                        Success = result.Success,
                        ScheduleId = result.Success ? result.Data : null,
                        Error = result.Success ? null : result.Message
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new BatchScheduleResult
                    {
                        JobCardId = req.JobCardId,
                        JobCardNo = req.JobCardId.ToString(),
                        Success = false,
                        Error = ex.Message
                    });
                }
            }

            var allOk = results.All(r => r.Success);
            var msg = allOk
                ? $"{results.Count} schedules created successfully"
                : $"{results.Count(r => r.Success)} succeeded, {results.Count(r => !r.Success)} failed";

            return ApiResponse<IEnumerable<BatchScheduleResult>>.SuccessResponse(results, msg);
        }

        private static ScheduleResponse MapToResponse(MachineSchedule schedule)
        {
            return new ScheduleResponse
            {
                Id = schedule.Id,
                JobCardId = schedule.JobCardId,
                JobCardNo = schedule.JobCardNo,
                OrderId = schedule.OrderId,
                OrderNo = schedule.OrderNo,
                OrderItemId = schedule.OrderItemId,
                ItemSequence = schedule.ItemSequence,
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
