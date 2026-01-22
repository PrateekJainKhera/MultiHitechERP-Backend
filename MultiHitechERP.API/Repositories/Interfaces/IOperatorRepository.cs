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
        Task<Operator?> GetByIdAsync(int id);
        Task<Operator?> GetByOperatorCodeAsync(string operatorCode);
        Task<IEnumerable<Operator>> GetAllAsync();
        Task<IEnumerable<Operator>> GetActiveOperatorsAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Operator operatorEntity);
        Task<bool> UpdateAsync(Operator operatorEntity);
        Task<bool> DeleteAsync(int id);

        // Status & Availability Operations
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable);
        Task<bool> AssignToJobCardAsync(int id, int jobCardId, string jobCardNo, int? machineId);
        Task<bool> ReleaseFromJobCardAsync(int id);

        // Queries
        Task<IEnumerable<Operator>> GetAvailableOperatorsAsync();
        Task<IEnumerable<Operator>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Operator>> GetByShiftAsync(string shift);
        Task<IEnumerable<Operator>> GetBySkillLevelAsync(string skillLevel);
        Task<IEnumerable<Operator>> GetByMachineExpertiseAsync(int machineId);
        Task<bool> ExistsAsync(string operatorCode);
    }
}
