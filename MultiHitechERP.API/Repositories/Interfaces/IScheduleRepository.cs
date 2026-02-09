using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Scheduling;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<MachineSchedule?> GetByIdAsync(int id);
        Task<IEnumerable<MachineSchedule>> GetAllAsync();
        Task<IEnumerable<MachineSchedule>> GetByMachineIdAsync(int machineId);
        Task<IEnumerable<MachineSchedule>> GetByJobCardIdAsync(int jobCardId);
        Task<IEnumerable<MachineSchedule>> GetByStatusAsync(string status);
        Task<IEnumerable<MachineSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MachineSchedule>> GetByMachineAndDateRangeAsync(int machineId, DateTime startDate, DateTime endDate);
        Task<bool> HasConflictAsync(int machineId, DateTime startTime, DateTime endTime, int? excludeScheduleId = null);
        Task<int> InsertAsync(MachineSchedule schedule);
        Task<bool> UpdateAsync(MachineSchedule schedule);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status, string? updatedBy = null);
    }
}
