using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface ISchedulingPlannerService
    {
        /// <summary>Step 1 — Orders that have at least one job card with material issued and not yet machine-scheduled</summary>
        Task<ApiResponse<IEnumerable<SchedulableOrderV2Response>>> GetSchedulableOrdersAsync();

        /// <summary>Step 2 — All job cards for selected orders/order-items, grouped by child part, with material status flag</summary>
        Task<ApiResponse<IEnumerable<ChildPartJobGroupResponse>>> GetJobCardsForOrdersAsync(IEnumerable<int> orderIds, IEnumerable<int>? orderItemIds = null);

        /// <summary>Step 4 — Per-process-category machine suggestions for the selected job cards</summary>
        Task<ApiResponse<IEnumerable<CategoryMachineSuggestionResponse>>> GetCategoryMachineSuggestionsAsync(
            IEnumerable<int> jobCardIds, DateTime targetDate);

        /// <summary>Step 5 — Batch-create schedules for all assigned job cards</summary>
        Task<ApiResponse<IEnumerable<BatchScheduleV2Result>>> BatchCreateSchedulesAsync(
            IEnumerable<CreateScheduleV2Request> requests);

        /// <summary>Batch reschedule — move a set of schedules to a new date/shift</summary>
        Task<ApiResponse<bool>> BatchRescheduleAsync(
            IEnumerable<int> scheduleIds, int shiftId, DateTime newDate, string reason, string? rescheduledBy);

        /// <summary>Create a rework job card from an existing one</summary>
        Task<ApiResponse<int>> CreateReworkJobCardAsync(int parentJobCardId, int reworkQty, string? notes, string? createdBy);
    }
}
