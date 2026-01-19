using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Operator master operations
    /// </summary>
    public interface IOperatorRepository
    {
        // Basic CRUD Operations
        Task<Operator?> GetByIdAsync(Guid id);
        Task<Operator?> GetByOperatorCodeAsync(string operatorCode);
        Task<IEnumerable<Operator>> GetAllAsync();
        Task<IEnumerable<Operator>> GetActiveOperatorsAsync();

        // Create, Update, Delete
        Task<Guid> InsertAsync(Operator operatorEntity);
        Task<bool> UpdateAsync(Operator operatorEntity);
        Task<bool> DeleteAsync(Guid id);

        // Status & Availability Operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable);
        Task<bool> AssignToJobCardAsync(Guid id, Guid jobCardId, string jobCardNo, Guid? machineId);
        Task<bool> ReleaseFromJobCardAsync(Guid id);

        // Queries
        Task<IEnumerable<Operator>> GetAvailableOperatorsAsync();
        Task<IEnumerable<Operator>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Operator>> GetByShiftAsync(string shift);
        Task<IEnumerable<Operator>> GetBySkillLevelAsync(string skillLevel);
        Task<IEnumerable<Operator>> GetByMachineExpertiseAsync(Guid machineId);
        Task<bool> ExistsAsync(string operatorCode);
    }
}
