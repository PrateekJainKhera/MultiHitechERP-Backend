using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class SchedulingPlannerService : ISchedulingPlannerService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IShiftRepository _shiftRepository;

        public SchedulingPlannerService(
            IDbConnectionFactory connectionFactory,
            IScheduleRepository scheduleRepository,
            IMachineRepository machineRepository,
            IShiftRepository shiftRepository)
        {
            _connectionFactory = connectionFactory;
            _scheduleRepository = scheduleRepository;
            _machineRepository = machineRepository;
            _shiftRepository = shiftRepository;
        }

        // ── STEP 1: Schedulable Orders ──────────────────────────────────────────

        public async Task<ApiResponse<IEnumerable<SchedulableOrderV2Response>>> GetSchedulableOrdersAsync()
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();

                // Use CTE to pre-compute per-JC flags first, then aggregate per order-item
                const string sql = @"
                    WITH JcFlags AS (
                        SELECT
                            jc.Id AS JobCardId,
                            jc.OrderId,
                            jc.OrderItemId,
                            jc.ItemSequence,
                            jc.OrderNo,
                            jc.Priority,
                            CASE WHEN (
                                EXISTS(SELECT 1 FROM Stores_MaterialRequisitions mr
                                       WHERE mr.OrderItemId = jc.OrderItemId AND mr.Status = 'Issued'
                                       AND jc.OrderItemId IS NOT NULL)
                                OR EXISTS(SELECT 1 FROM Stores_MaterialRequisitions mr
                                          WHERE mr.OrderId = jc.OrderId AND mr.Status = 'Issued'
                                          AND jc.OrderItemId IS NULL)
                                OR EXISTS(SELECT 1 FROM Planning_JobCardMaterialRequirements req
                                          WHERE req.JobCardId = jc.Id AND req.IssuedViaCuttingList = 1)
                            ) THEN 1 ELSE 0 END AS MaterialIssued,
                            CASE WHEN EXISTS(
                                SELECT 1 FROM Scheduling_MachineSchedules ms
                                WHERE ms.JobCardId = jc.Id AND ms.Status IN ('Scheduled', 'InProgress')
                            ) THEN 1 ELSE 0 END AS AlreadyScheduled
                        FROM Planning_JobCards jc
                        WHERE jc.Status NOT IN ('Completed', 'Cancelled')
                    )
                    SELECT
                        f.OrderId,
                        f.OrderItemId,
                        MAX(f.ItemSequence) AS ItemSequence,
                        MAX(f.OrderNo) AS OrderNo,
                        MAX(ISNULL(c.CustomerName, '')) AS CustomerName,
                        MAX(o.DueDate) AS DueDate,
                        MAX(ISNULL(f.Priority, 'Medium')) AS Priority,
                        COUNT(f.JobCardId) AS TotalJobCards,
                        SUM(f.MaterialIssued) AS MaterialIssuedCount,
                        SUM(f.AlreadyScheduled) AS AlreadyScheduledCount
                    FROM JcFlags f
                    LEFT JOIN Orders o ON o.Id = f.OrderId
                    LEFT JOIN Masters_Customers c ON c.Id = o.CustomerId
                    GROUP BY f.OrderId, f.OrderItemId
                    HAVING SUM(CASE WHEN f.MaterialIssued = 1 AND f.AlreadyScheduled = 0 THEN 1 ELSE 0 END) > 0
                    ORDER BY MAX(o.DueDate), MAX(f.OrderNo)";

                var rows = await conn.QueryAsync(sql);

                var result = rows.Select(r =>
                {
                    var baseOrderNo = (string)r.OrderNo ?? "";
                    var itemSeq = (string?)r.ItemSequence;
                    return new SchedulableOrderV2Response
                    {
                    OrderId = (int)r.OrderId,
                    OrderItemId = (int?)r.OrderItemId,
                    ItemSequence = itemSeq,
                    OrderNo = !string.IsNullOrEmpty(itemSeq) ? $"{baseOrderNo}-{itemSeq}" : baseOrderNo,
                    CustomerName = (string?)r.CustomerName,
                    DueDate = (DateTime?)r.DueDate,
                    Priority = (string)r.Priority ?? "Medium",
                    TotalJobCards = (int)r.TotalJobCards,
                    MaterialIssuedCount = (int)r.MaterialIssuedCount,
                    AlreadyScheduledCount = (int)r.AlreadyScheduledCount,
                    ReadyToScheduleCount = Math.Max(0, (int)r.MaterialIssuedCount - (int)r.AlreadyScheduledCount)
                    };
                }).ToList();

                return ApiResponse<IEnumerable<SchedulableOrderV2Response>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<SchedulableOrderV2Response>>.ErrorResponse($"Error loading orders: {ex.Message}");
            }
        }

        // ── STEP 2: Job Cards For Orders ────────────────────────────────────────

        public async Task<ApiResponse<IEnumerable<ChildPartJobGroupResponse>>> GetJobCardsForOrdersAsync(
            IEnumerable<int> orderIds, IEnumerable<int>? orderItemIds = null)
        {
            try
            {
                var orderIdList = orderIds.ToList();
                var orderItemIdList = orderItemIds?.ToList() ?? new List<int>();

                if (!orderIdList.Any() && !orderItemIdList.Any())
                    return ApiResponse<IEnumerable<ChildPartJobGroupResponse>>.ErrorResponse("No orders specified");

                using var conn = _connectionFactory.CreateConnection();

                // Build WHERE clause: filter by orderItemId when available, else by orderId
                string whereClause;
                object parameters;
                if (orderItemIdList.Any() && orderIdList.Any())
                {
                    whereClause = "(jc.OrderItemId IN @OrderItemIds OR (jc.OrderItemId IS NULL AND jc.OrderId IN @OrderIds))";
                    parameters = new { OrderItemIds = orderItemIdList, OrderIds = orderIdList };
                }
                else if (orderItemIdList.Any())
                {
                    whereClause = "jc.OrderItemId IN @OrderItemIds";
                    parameters = new { OrderItemIds = orderItemIdList };
                }
                else
                {
                    whereClause = "jc.OrderId IN @OrderIds";
                    parameters = new { OrderIds = orderIdList };
                }

                var sql = $@"
                    SELECT
                        jc.Id AS JobCardId,
                        jc.JobCardNo,
                        jc.OrderId,
                        jc.OrderNo,
                        jc.ItemSequence,
                        ISNULL(c.CustomerName, '') AS CustomerName,
                        o.DueDate,
                        ISNULL(jc.ChildPartName, 'Unknown Part') AS ChildPartName,
                        ISNULL(jc.CreationType, 'ChildPart') AS CreationType,
                        jc.ProcessId,
                        ISNULL(jc.ProcessName, '') AS ProcessName,
                        jc.ProcessCode,
                        jc.StepNo,
                        p.ProcessCategoryId,
                        ISNULL(pc.CategoryName, '') AS ProcessCategoryName,
                        ISNULL(p.IsOutsourced, 0) AS IsOsp,
                        ISNULL(p.IsManual, 0) AS IsManual,
                        jc.Quantity,
                        ISNULL(jc.Priority, 'Medium') AS Priority,
                        ISNULL(p.StandardSetupTimeMin, 30) AS SetupTimeMinutes,
                        ISNULL(p.CycleTimePerPieceHours, 0.25) AS CycleTimePerPieceHours,
                        ISNULL(jc.IsRework, 0) AS IsRework,
                        ISNULL(jc.JobCardType, 'Normal') AS JobCardType,
                        CASE WHEN (
                            EXISTS(SELECT 1 FROM Stores_MaterialRequisitions mr
                                   WHERE mr.OrderItemId = jc.OrderItemId AND mr.Status = 'Issued'
                                   AND jc.OrderItemId IS NOT NULL)
                            OR EXISTS(SELECT 1 FROM Stores_MaterialRequisitions mr
                                      WHERE mr.OrderId = jc.OrderId AND mr.Status = 'Issued'
                                      AND jc.OrderItemId IS NULL)
                            OR EXISTS(SELECT 1 FROM Planning_JobCardMaterialRequirements req
                                      WHERE req.JobCardId = jc.Id AND req.IssuedViaCuttingList = 1)
                        ) THEN 1 ELSE 0 END AS MaterialIssued,
                        CASE WHEN EXISTS(
                            SELECT 1 FROM Scheduling_MachineSchedules ms
                            WHERE ms.JobCardId = jc.Id AND ms.Status IN ('Scheduled', 'InProgress')
                        ) THEN 1 ELSE 0 END AS IsAlreadyScheduled
                    FROM Planning_JobCards jc
                    LEFT JOIN Orders o ON o.Id = jc.OrderId
                    LEFT JOIN Masters_Customers c ON c.Id = o.CustomerId
                    LEFT JOIN Masters_Processes p ON p.Id = jc.ProcessId
                    LEFT JOIN Masters_ProcessCategories pc ON pc.Id = p.ProcessCategoryId
                    WHERE {whereClause}
                    AND jc.Status NOT IN ('Completed', 'Cancelled')
                    ORDER BY
                        CASE WHEN jc.CreationType = 'Assembly' THEN 1 ELSE 0 END,
                        ISNULL(jc.ChildPartName, 'Unknown Part'),
                        jc.StepNo";

                var rows = await conn.QueryAsync(sql, parameters);

                // Map rows to JobCardForSchedulingResponse
                var jobCards = rows.Select(r =>
                {
                    var setupMin = (int)(r.SetupTimeMinutes ?? 30);
                    var cycleHours = (decimal)(r.CycleTimePerPieceHours ?? 0.25m);
                    var cycleMin = (decimal)(cycleHours * 60m);
                    var qty = (int)(r.Quantity ?? 1);
                    var estMin = (int)Math.Max(15, setupMin + (int)(qty * cycleMin));

                    var jcBaseNo = (string)r.OrderNo ?? "";
                    var jcSeq = (string?)r.ItemSequence;
                    return new JobCardForSchedulingResponse
                    {
                        JobCardId = (int)r.JobCardId,
                        JobCardNo = (string)r.JobCardNo ?? "",
                        OrderId = (int)r.OrderId,
                        OrderNo = !string.IsNullOrEmpty(jcSeq) ? $"{jcBaseNo}-{jcSeq}" : jcBaseNo,
                        CustomerName = (string?)r.CustomerName,
                        DueDate = (DateTime?)r.DueDate,
                        ChildPartName = (string?)r.ChildPartName,
                        CreationType = (string?)r.CreationType,
                        ProcessId = (int)r.ProcessId,
                        ProcessName = (string)r.ProcessName ?? "",
                        ProcessCode = (string?)r.ProcessCode,
                        StepNo = (int?)r.StepNo,
                        ProcessCategoryId = (int?)r.ProcessCategoryId,
                        ProcessCategoryName = (string?)r.ProcessCategoryName,
                        IsOsp = Convert.ToBoolean(r.IsOsp),
                        IsManual = Convert.ToBoolean(r.IsManual),
                        Quantity = qty,
                        Priority = (string)r.Priority ?? "Medium",
                        MaterialIssued = ((int)r.MaterialIssued) == 1,
                        IsAlreadyScheduled = ((int)r.IsAlreadyScheduled) == 1,
                        IsRework = Convert.ToBoolean(r.IsRework),
                        JobCardType = (string)r.JobCardType ?? "Normal",
                        SetupTimeMinutes = setupMin,
                        CycleTimeMinutesPerPiece = cycleMin,
                        EstimatedDurationMinutes = estMin
                    };
                }).ToList();

                // Group by ChildPartName
                var groups = jobCards
                    .GroupBy(jc => new { Name = jc.ChildPartName ?? "Unknown Part", Type = jc.CreationType ?? "ChildPart" })
                    .OrderBy(g => g.Key.Type == "Assembly" ? 1 : 0)
                    .ThenBy(g => g.Key.Name)
                    .Select(g => new ChildPartJobGroupResponse
                    {
                        ChildPartName = g.Key.Name,
                        CreationType = g.Key.Type,
                        JobCards = g.OrderBy(jc => jc.StepNo ?? 999).ToList()
                    })
                    .ToList();

                return ApiResponse<IEnumerable<ChildPartJobGroupResponse>>.SuccessResponse(groups);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartJobGroupResponse>>.ErrorResponse($"Error loading job cards: {ex.Message}");
            }
        }

        // ── STEP 4: Category Machine Suggestions ────────────────────────────────

        public async Task<ApiResponse<IEnumerable<CategoryMachineSuggestionResponse>>> GetCategoryMachineSuggestionsAsync(
            IEnumerable<int> jobCardIds, DateTime targetDate)
        {
            try
            {
                var jcIdList = jobCardIds.ToList();
                if (!jcIdList.Any())
                    return ApiResponse<IEnumerable<CategoryMachineSuggestionResponse>>.ErrorResponse("No job cards specified");

                using var conn = _connectionFactory.CreateConnection();

                // Get job card details (category, duration, etc.)
                const string jcSql = @"
                    SELECT
                        jc.Id AS JobCardId,
                        jc.ProcessId,
                        p.ProcessCategoryId,
                        ISNULL(pc.CategoryName, '') AS ProcessCategoryName,
                        ISNULL(p.IsOutsourced, 0) AS IsOsp,
                        ISNULL(p.IsManual, 0) AS IsManual,
                        jc.Quantity,
                        ISNULL(p.StandardSetupTimeMin, 30) AS SetupTimeMinutes,
                        ISNULL(p.CycleTimePerPieceHours, 0.25) AS CycleTimePerPieceHours
                    FROM Planning_JobCards jc
                    LEFT JOIN Masters_Processes p ON p.Id = jc.ProcessId
                    LEFT JOIN Masters_ProcessCategories pc ON pc.Id = p.ProcessCategoryId
                    WHERE jc.Id IN @JobCardIds";

                var jcRows = await conn.QueryAsync(jcSql, new { JobCardIds = jcIdList });

                // Group by (isOsp, isManual, categoryId)
                var categories = jcRows
                    .GroupBy(r => (
                        IsOsp: Convert.ToBoolean(r.IsOsp),
                        IsManual: Convert.ToBoolean(r.IsManual),
                        CatId: (int?)r.ProcessCategoryId,
                        CatName: (string?)r.ProcessCategoryName
                    ))
                    .OrderBy(g => g.Key.IsOsp ? 2 : g.Key.IsManual ? 1 : 0)
                    .ThenBy(g => g.Key.CatId ?? int.MaxValue);

                var result = new List<CategoryMachineSuggestionResponse>();

                foreach (var catGroup in categories)
                {
                    var isOsp = catGroup.Key.IsOsp;
                    var isManual = catGroup.Key.IsManual;
                    var catId = catGroup.Key.CatId;
                    var catName = catGroup.Key.CatName;
                    var displayName = catName?.Length > 0
                        ? catName
                        : (isOsp ? "Outside Service Process" : isManual ? "Manual Process" : "Uncategorized");

                    var key = isOsp ? "OSP" : isManual ? "MANUAL" : catId?.ToString() ?? "NONE";

                    var catJobCardIds = catGroup.Select(r => (int)r.JobCardId).ToList();

                    // Calculate total duration for this category
                    var totalEstMin = catGroup.Sum(r =>
                    {
                        var setup = (int)(r.SetupTimeMinutes ?? 30);
                        var cycleHrs = (decimal)(r.CycleTimePerPieceHours ?? 0.25m);
                        var qty = (int)(r.Quantity ?? 1);
                        return (int)Math.Max(15, setup + (int)(qty * cycleHrs * 60));
                    });

                    var suggestion = new CategoryMachineSuggestionResponse
                    {
                        CategoryKey = key,
                        ProcessCategoryId = catId,
                        ProcessCategoryName = displayName,
                        IsOsp = isOsp,
                        IsManual = isManual,
                        JobCardIds = catJobCardIds,
                        TotalJobCards = catJobCardIds.Count,
                        TotalEstimatedMinutes = totalEstMin,
                        TotalEstimatedHours = Math.Round(totalEstMin / 60m, 2),
                        SuggestedMachines = new List<MachineSuggestionResponse>()
                    };

                    // For machine processes (not OSP/Manual), find capable machines
                    if (!isOsp && !isManual && catId.HasValue)
                    {
                        var machines = await _machineRepository.GetByProcessCategoryIdAsync(catId.Value);
                        foreach (var machine in machines.Where(m => m.IsActive))
                        {
                            var allSchedules = await _scheduleRepository.GetByMachineIdAsync(machine.Id);
                            var todaySchedules = allSchedules
                                .Where(s => s.ScheduledStartTime.Date == targetDate.Date &&
                                            (s.Status == "Scheduled" || s.Status == "InProgress" || s.Status == "Completed"))
                                .ToList();

                            decimal scheduledHours = 0;
                            foreach (var s in todaySchedules)
                                scheduledHours += (decimal)(s.ScheduledEndTime - s.ScheduledStartTime).TotalHours;

                            var dailyCap = machine.DailyCapacityHours;
                            var available = Math.Max(0, dailyCap - scheduledHours);
                            var util = dailyCap > 0 ? Math.Round((scheduledHours / dailyCap) * 100, 1) : 0m;

                            string capStatus = util >= 100 ? "Overloaded" : util >= 90 ? "Busy" : util >= 70 ? "Moderate" : "Available";

                            suggestion.SuggestedMachines.Add(new MachineSuggestionResponse
                            {
                                MachineId = machine.Id,
                                MachineCode = machine.MachineCode,
                                MachineName = machine.MachineName,
                                MachineType = machine.MachineType,
                                Location = machine.Location,
                                Department = machine.Department,
                                ProcessCategoryId = catId.Value,
                                ProcessCategoryName = displayName,
                                DailyCapacityHours = dailyCap,
                                ScheduledHours = Math.Round(scheduledHours, 2),
                                AvailableHours = Math.Round(available, 2),
                                UtilizationPercent = util,
                                CapacityStatus = capStatus,
                                IsBusy = util >= 90,
                                TotalJobCards = todaySchedules.Select(s => s.JobCardNo).Distinct().Count(),
                                ScheduledJobCardNumbers = todaySchedules.Select(s => s.JobCardNo ?? "").Where(n => n.Length > 0).Distinct().ToList()
                            });
                        }

                        // Sort by utilization (best first)
                        suggestion.SuggestedMachines = suggestion.SuggestedMachines
                            .OrderBy(m => m.UtilizationPercent)
                            .ThenByDescending(m => m.AvailableHours)
                            .ToList();
                    }

                    result.Add(suggestion);
                }

                return ApiResponse<IEnumerable<CategoryMachineSuggestionResponse>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoryMachineSuggestionResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        // ── STEP 5: Batch Create Schedules ──────────────────────────────────────

        public async Task<ApiResponse<IEnumerable<BatchScheduleV2Result>>> BatchCreateSchedulesAsync(
            IEnumerable<CreateScheduleV2Request> requests)
        {
            var results = new List<BatchScheduleV2Result>();

            using var conn = _connectionFactory.CreateConnection();

            foreach (var req in requests)
            {
                try
                {
                    // Get job card info
                    var jcRow = await conn.QueryFirstOrDefaultAsync(
                        "SELECT Id, JobCardNo, OrderId, OrderNo, ProcessId, ProcessName, ProcessCode FROM Planning_JobCards WHERE Id = @Id",
                        new { Id = req.JobCardId });

                    if (jcRow == null)
                    {
                        results.Add(new BatchScheduleV2Result { JobCardId = req.JobCardId, Success = false, Error = "Job card not found" });
                        continue;
                    }

                    string machineCode = "MANUAL";
                    string machineName = "Manual Process";
                    if (req.IsOsp) { machineCode = "OSP"; machineName = "Outside Service Process"; }
                    else if (!req.IsManual && req.MachineId.HasValue)
                    {
                        var machine = await _machineRepository.GetByIdAsync(req.MachineId.Value);
                        if (machine == null)
                        {
                            results.Add(new BatchScheduleV2Result { JobCardId = req.JobCardId, JobCardNo = (string)jcRow.JobCardNo, Success = false, Error = "Machine not found" });
                            continue;
                        }
                        machineCode = machine.MachineCode;
                        machineName = machine.MachineName;
                    }

                    // Check for conflicts on real machines
                    if (!req.IsOsp && !req.IsManual && req.MachineId.HasValue)
                    {
                        var hasConflict = await _scheduleRepository.HasConflictAsync(
                            req.MachineId.Value, req.ScheduledStartTime, req.ScheduledEndTime);
                        if (hasConflict)
                        {
                            results.Add(new BatchScheduleV2Result { JobCardId = req.JobCardId, JobCardNo = (string)jcRow.JobCardNo, Success = false, Error = "Machine time conflict" });
                            continue;
                        }
                    }

                    // Insert schedule
                    var scheduleId = await conn.QuerySingleAsync<int>(@"
                        INSERT INTO Scheduling_MachineSchedules
                            (JobCardId, JobCardNo, OrderId, OrderNo, MachineId, MachineCode, MachineName,
                             ScheduledStartTime, ScheduledEndTime, EstimatedDurationMinutes,
                             Status, SchedulingMethod, SuggestedBySystem, ConfirmedBy, ConfirmedAt,
                             ProcessId, ProcessName, ProcessCode, ShiftId, ShiftName,
                             Notes, CreatedAt, CreatedBy)
                        VALUES
                            (@JobCardId, @JobCardNo, @OrderId, @OrderNo, @MachineId, @MachineCode, @MachineName,
                             @StartTime, @EndTime, @Duration,
                             'Scheduled', 'Batch', 1, @CreatedBy, GETUTCDATE(),
                             @ProcessId, @ProcessName, @ProcessCode, @ShiftId, @ShiftName,
                             @Notes, GETUTCDATE(), @CreatedBy);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        new
                        {
                            JobCardId = req.JobCardId,
                            JobCardNo = (string)jcRow.JobCardNo,
                            OrderId = (int)jcRow.OrderId,
                            OrderNo = (string)jcRow.OrderNo,
                            MachineId = (!req.IsOsp && !req.IsManual) ? req.MachineId : (int?)null,
                            MachineCode = machineCode,
                            MachineName = machineName,
                            StartTime = req.ScheduledStartTime,
                            EndTime = req.ScheduledEndTime,
                            Duration = req.EstimatedDurationMinutes,
                            CreatedBy = req.CreatedBy ?? "System",
                            ProcessId = (int)jcRow.ProcessId,
                            ProcessName = (string)jcRow.ProcessName,
                            ProcessCode = (string?)jcRow.ProcessCode,
                            ShiftId = req.ShiftId,
                            ShiftName = req.ShiftName,
                            Notes = req.Notes
                        });

                    // Update job card status
                    await conn.ExecuteAsync(
                        "UPDATE Planning_JobCards SET Status = 'Scheduled', UpdatedAt = GETUTCDATE() WHERE Id = @Id",
                        new { Id = req.JobCardId });

                    results.Add(new BatchScheduleV2Result
                    {
                        JobCardId = req.JobCardId,
                        JobCardNo = (string)jcRow.JobCardNo,
                        OrderNo = (string)jcRow.OrderNo,
                        Success = true,
                        ScheduleId = scheduleId,
                        MachineName = machineName,
                        ScheduledStart = req.ScheduledStartTime,
                        ScheduledEnd = req.ScheduledEndTime
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new BatchScheduleV2Result
                    {
                        JobCardId = req.JobCardId,
                        Success = false,
                        Error = ex.Message
                    });
                }
            }

            var allOk = results.All(r => r.Success);
            var msg = allOk
                ? $"{results.Count} schedule(s) created successfully"
                : $"{results.Count(r => r.Success)} succeeded, {results.Count(r => !r.Success)} failed";

            return ApiResponse<IEnumerable<BatchScheduleV2Result>>.SuccessResponse(results, msg);
        }

        // ── Batch Reschedule ────────────────────────────────────────────────────

        public async Task<ApiResponse<bool>> BatchRescheduleAsync(
            IEnumerable<int> scheduleIds, int shiftId, DateTime newDate, string reason, string? rescheduledBy)
        {
            try
            {
                var idList = scheduleIds.ToList();
                if (!idList.Any())
                    return ApiResponse<bool>.ErrorResponse("No schedules specified");

                var shift = await _shiftRepository.GetByIdAsync(shiftId);
                if (shift == null)
                    return ApiResponse<bool>.ErrorResponse("Shift not found");

                using var conn = _connectionFactory.CreateConnection();

                // Get all schedules to reschedule
                var schedules = await conn.QueryAsync(
                    "SELECT * FROM Scheduling_MachineSchedules WHERE Id IN @Ids ORDER BY ScheduledStartTime",
                    new { Ids = idList });

                var shiftStart = new DateTime(newDate.Year, newDate.Month, newDate.Day,
                    shift.StartTime.Hours, shift.StartTime.Minutes, 0);

                // Group by machine and sequence them
                var machineGroups = schedules.GroupBy(s => (int?)(s.MachineId as int?));
                var updatedCount = 0;

                foreach (var group in machineGroups)
                {
                    var cursor = shiftStart;
                    foreach (var s in group.OrderBy(s => s.ScheduledStartTime))
                    {
                        var duration = TimeSpan.FromMinutes((int)s.EstimatedDurationMinutes);
                        var newEnd = cursor + duration;

                        await conn.ExecuteAsync(@"
                            UPDATE Scheduling_MachineSchedules SET
                                ScheduledStartTime = @NewStart,
                                ScheduledEndTime = @NewEnd,
                                ShiftId = @ShiftId,
                                ShiftName = @ShiftName,
                                IsRescheduled = 1,
                                RescheduledReason = @Reason,
                                RescheduledAt = GETUTCDATE(),
                                RescheduledBy = @RescheduledBy,
                                UpdatedAt = GETUTCDATE()
                            WHERE Id = @Id",
                            new
                            {
                                Id = (int)s.Id,
                                NewStart = cursor,
                                NewEnd = newEnd,
                                ShiftId = shiftId,
                                ShiftName = shift.ShiftName,
                                Reason = reason,
                                RescheduledBy = rescheduledBy ?? "System"
                            });

                        cursor = newEnd;
                        updatedCount++;
                    }
                }

                return ApiResponse<bool>.SuccessResponse(true, $"{updatedCount} schedule(s) rescheduled to {newDate:dd MMM yyyy} {shift.ShiftName}");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error rescheduling: {ex.Message}");
            }
        }

        // ── Create Rework Job Card ───────────────────────────────────────────────

        public async Task<ApiResponse<int>> CreateReworkJobCardAsync(
            int parentJobCardId, int reworkQty, string? notes, string? createdBy)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();

                var parent = await conn.QueryFirstOrDefaultAsync(
                    "SELECT * FROM Planning_JobCards WHERE Id = @Id", new { Id = parentJobCardId });

                if (parent == null)
                    return ApiResponse<int>.ErrorResponse("Parent job card not found");

                if (reworkQty <= 0)
                    return ApiResponse<int>.ErrorResponse("Rework quantity must be > 0");

                // Generate rework job card number
                var baseNo = (string)parent.JobCardNo;
                var rwNo = baseNo + "-RW";

                // Check if rework already exists; if so, append a counter
                var existingCount = await conn.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Planning_JobCards WHERE JobCardNo LIKE @Pattern",
                    new { Pattern = baseNo + "-RW%" });
                if (existingCount > 0)
                    rwNo = baseNo + "-RW" + (existingCount + 1);

                var newId = await conn.QuerySingleAsync<int>(@"
                    INSERT INTO Planning_JobCards
                        (JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                         ChildPartId, ChildPartName, ProcessId, ProcessName, ProcessCode, StepNo,
                         Quantity, Status, Priority, IsRework, ParentJobCardId, JobCardType,
                         Notes, CreatedAt, CreatedBy)
                    VALUES
                        (@JobCardNo, @CreationType, @OrderId, @OrderNo, @OrderItemId, @ItemSequence,
                         @ChildPartId, @ChildPartName, @ProcessId, @ProcessName, @ProcessCode, @StepNo,
                         @Quantity, 'Pending', @Priority, 1, @ParentJobCardId, 'Rework',
                         @Notes, GETUTCDATE(), @CreatedBy);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new
                    {
                        JobCardNo = rwNo,
                        CreationType = (string)parent.CreationType,
                        OrderId = (int)parent.OrderId,
                        OrderNo = (string?)parent.OrderNo,
                        OrderItemId = (int?)parent.OrderItemId,
                        ItemSequence = (string?)parent.ItemSequence,
                        ChildPartId = (int?)parent.ChildPartId,
                        ChildPartName = (string?)parent.ChildPartName,
                        ProcessId = (int)parent.ProcessId,
                        ProcessName = (string?)parent.ProcessName,
                        ProcessCode = (string?)parent.ProcessCode,
                        StepNo = (int?)parent.StepNo,
                        Quantity = reworkQty,
                        Priority = (string?)parent.Priority ?? "Medium",
                        ParentJobCardId = parentJobCardId,
                        Notes = notes,
                        CreatedBy = createdBy ?? "System"
                    });

                return ApiResponse<int>.SuccessResponse(newId, $"Rework job card {rwNo} created for {reworkQty} pieces");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating rework: {ex.Message}");
            }
        }
    }
}
