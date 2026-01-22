using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Customer master operations
    /// </summary>
    public interface ICustomerRepository
    {
        // Basic CRUD Operations
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByCustomerCodeAsync(string customerCode);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);

        // Queries
        Task<IEnumerable<Customer>> SearchByNameAsync(string name);
        Task<IEnumerable<Customer>> GetByCustomerTypeAsync(string customerType);
        Task<IEnumerable<Customer>> GetByCityAsync(string city);
        Task<IEnumerable<Customer>> GetByStateAsync(string state);
        Task<bool> ExistsAsync(string customerCode);
    }
}
