using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Process master operations
    /// </summary>
    public interface IProcessRepository
    {
        // Basic CRUD Operations
        Task<Process?> GetByIdAsync(int id);
        Task<Process?> GetByProcessCodeAsync(string processCode);
        Task<IEnumerable<Process>> GetAllAsync();
        Task<IEnumerable<Process>> GetActiveProcessesAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Process process);
        Task<bool> UpdateAsync(Process process);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);

        // Queries
        Task<IEnumerable<Process>> GetByProcessTypeAsync(string processType);
        Task<IEnumerable<Process>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Process>> GetByMachineTypeAsync(string machineType);
        Task<IEnumerable<Process>> GetOutsourcedProcessesAsync();
        Task<bool> ExistsAsync(string processCode);

        // Code Generation
        Task<int> GetNextSequenceNumberAsync(string category);
    }
}
