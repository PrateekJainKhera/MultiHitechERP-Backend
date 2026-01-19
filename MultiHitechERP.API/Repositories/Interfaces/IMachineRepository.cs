using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Machine master operations
    /// </summary>
    public interface IMachineRepository
    {
        // Basic CRUD Operations
        Task<Machine?> GetByIdAsync(Guid id);
        Task<Machine?> GetByMachineCodeAsync(string machineCode);
        Task<IEnumerable<Machine>> GetAllAsync();
        Task<IEnumerable<Machine>> GetActiveMachinesAsync();

        // Create, Update, Delete
        Task<Guid> InsertAsync(Machine machine);
        Task<bool> UpdateAsync(Machine machine);
        Task<bool> DeleteAsync(Guid id);

        // Status & Availability Operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom);
        Task<bool> AssignToJobCardAsync(Guid id, string jobCardNo);
        Task<bool> ReleaseFromJobCardAsync(Guid id);

        // Queries
        Task<IEnumerable<Machine>> GetAvailableMachinesAsync();
        Task<IEnumerable<Machine>> GetByMachineTypeAsync(string machineType);
        Task<IEnumerable<Machine>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Machine>> GetDueForMaintenanceAsync();
        Task<bool> ExistsAsync(string machineCode);
    }
}
