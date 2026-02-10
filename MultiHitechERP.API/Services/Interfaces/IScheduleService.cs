using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<ApiResponse<ScheduleResponse>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByMachineIdAsync(int machineId);
        Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByJobCardIdAsync(int jobCardId);
        Task<ApiResponse<IEnumerable<ScheduleResponse>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Order scheduling tree - groups job cards by child part for the UI
        Task<ApiResponse<OrderSchedulingTreeResponse>> GetOrderSchedulingTreeAsync(int orderId);

        // Semi-automatic scheduling - the core feature!
        Task<ApiResponse<IEnumerable<MachineSuggestionResponse>>> GetMachineSuggestionsAsync(int jobCardId);
        Task<ApiResponse<int>> CreateScheduleAsync(CreateScheduleRequest request);

        Task<ApiResponse<bool>> UpdateScheduleAsync(int id, CreateScheduleRequest request);
        Task<ApiResponse<bool>> DeleteScheduleAsync(int id);
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status, string? updatedBy = null);
        Task<ApiResponse<bool>> RescheduleAsync(int scheduleId, DateTime newStartTime, DateTime newEndTime, string reason, string? rescheduledBy = null);
    }
}
