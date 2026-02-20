using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<ApiResponse<IEnumerable<ShiftResponse>>> GetAllAsync()
        {
            try
            {
                var shifts = await _shiftRepository.GetAllAsync();
                return ApiResponse<IEnumerable<ShiftResponse>>.SuccessResponse(shifts.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ShiftResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ShiftResponse>>> GetActiveAsync()
        {
            try
            {
                var shifts = await _shiftRepository.GetActiveAsync();
                return ApiResponse<IEnumerable<ShiftResponse>>.SuccessResponse(shifts.Select(MapToResponse));
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ShiftResponse>>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ShiftResponse>> GetByIdAsync(int id)
        {
            try
            {
                var shift = await _shiftRepository.GetByIdAsync(id);
                if (shift == null)
                    return ApiResponse<ShiftResponse>.ErrorResponse("Shift not found");
                return ApiResponse<ShiftResponse>.SuccessResponse(MapToResponse(shift));
            }
            catch (Exception ex)
            {
                return ApiResponse<ShiftResponse>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateShiftRequest request)
        {
            try
            {
                if (!TimeSpan.TryParse(request.StartTime, out var start))
                    return ApiResponse<int>.ErrorResponse("Invalid start time format. Use HH:mm");
                if (!TimeSpan.TryParse(request.EndTime, out var end))
                    return ApiResponse<int>.ErrorResponse("Invalid end time format. Use HH:mm");

                var shift = new ShiftMaster
                {
                    ShiftName = request.ShiftName,
                    StartTime = start,
                    EndTime = end,
                    RegularHours = request.RegularHours,
                    MaxOvertimeHours = request.MaxOvertimeHours,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow
                };
                var id = await _shiftRepository.InsertAsync(shift);
                return ApiResponse<int>.SuccessResponse(id, "Shift created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, CreateShiftRequest request)
        {
            try
            {
                var existing = await _shiftRepository.GetByIdAsync(id);
                if (existing == null)
                    return ApiResponse<bool>.ErrorResponse("Shift not found");

                if (!TimeSpan.TryParse(request.StartTime, out var start))
                    return ApiResponse<bool>.ErrorResponse("Invalid start time format. Use HH:mm");
                if (!TimeSpan.TryParse(request.EndTime, out var end))
                    return ApiResponse<bool>.ErrorResponse("Invalid end time format. Use HH:mm");

                existing.ShiftName = request.ShiftName;
                existing.StartTime = start;
                existing.EndTime = end;
                existing.RegularHours = request.RegularHours;
                existing.MaxOvertimeHours = request.MaxOvertimeHours;
                existing.IsActive = request.IsActive;
                existing.UpdatedAt = DateTime.UtcNow;

                var ok = await _shiftRepository.UpdateAsync(existing);
                return ok
                    ? ApiResponse<bool>.SuccessResponse(true, "Shift updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update shift");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var ok = await _shiftRepository.DeleteAsync(id);
                return ok
                    ? ApiResponse<bool>.SuccessResponse(true, "Shift deleted")
                    : ApiResponse<bool>.ErrorResponse("Shift not found");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private static ShiftResponse MapToResponse(ShiftMaster s) => new()
        {
            Id = s.Id,
            ShiftName = s.ShiftName,
            StartTime = s.StartTime.ToString(@"hh\:mm"),
            EndTime = s.EndTime.ToString(@"hh\:mm"),
            RegularHours = s.RegularHours,
            MaxOvertimeHours = s.MaxOvertimeHours,
            IsActive = s.IsActive
        };
    }
}
