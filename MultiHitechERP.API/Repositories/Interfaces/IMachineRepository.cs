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
        Task<Machine?> GetByIdAsync(int id);
        Task<Machine?> GetByMachineCodeAsync(string machineCode);
        Task<IEnumerable<Machine>> GetAllAsync();
        Task<IEnumerable<Machine>> GetActiveMachinesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Machine machine);
        Task<bool> UpdateAsync(Machine machine);
        Task<bool> DeleteAsync(int id);

        // Status & Availability Operations
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable, DateTime? availableFrom);
        Task<bool> AssignToJobCardAsync(int id, string jobCardNo);
        Task<bool> ReleaseFromJobCardAsync(int id);

        // Queries
        Task<IEnumerable<Machine>> GetAvailableMachinesAsync();
        Task<IEnumerable<Machine>> GetByMachineTypeAsync(string machineType);
        Task<IEnumerable<Machine>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Machine>> GetDueForMaintenanceAsync();
        Task<bool> ExistsAsync(string machineCode);
    }
}
