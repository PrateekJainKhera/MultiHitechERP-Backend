using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Customer business logic
    /// </summary>
    public interface ICustomerService
    {
        // CRUD Operations
        Task<ApiResponse<CustomerResponse>> GetByIdAsync(int id);
        Task<ApiResponse<CustomerResponse>> GetByCustomerCodeAsync(string customerCode);
        Task<ApiResponse<IEnumerable<CustomerResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<CustomerResponse>>> GetActiveCustomersAsync();

        Task<ApiResponse<int>> CreateCustomerAsync(CreateCustomerRequest request);
        Task<ApiResponse<bool>> UpdateCustomerAsync(UpdateCustomerRequest request);
        Task<ApiResponse<bool>> DeleteCustomerAsync(int id);

        // Status Operations
        Task<ApiResponse<bool>> ActivateCustomerAsync(int id);
        Task<ApiResponse<bool>> DeactivateCustomerAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<CustomerResponse>>> SearchByNameAsync(string searchTerm);
        Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByCustomerTypeAsync(string customerType);
        Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByCityAsync(string city);
        Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByStateAsync(string state);
    }
}
