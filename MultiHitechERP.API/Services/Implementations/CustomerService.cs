using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Customer business logic
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<ApiResponse<CustomerResponse>> GetByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return ApiResponse<CustomerResponse>.ErrorResponse($"Customer with ID {id} not found");
                }

                var response = MapToResponse(customer);
                return ApiResponse<CustomerResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerResponse>.ErrorResponse($"Error retrieving customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerResponse>> GetByCustomerCodeAsync(string customerCode)
        {
            try
            {
                var customer = await _customerRepository.GetByCustomerCodeAsync(customerCode);
                if (customer == null)
                {
                    return ApiResponse<CustomerResponse>.ErrorResponse($"Customer {customerCode} not found");
                }

                var response = MapToResponse(customer);
                return ApiResponse<CustomerResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<CustomerResponse>.ErrorResponse($"Error retrieving customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> GetAllAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error retrieving customers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> GetActiveCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetActiveCustomersAsync();
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error retrieving active customers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                // Business Rule 1: Validate Customer Code is unique
                var exists = await _customerRepository.ExistsAsync(request.CustomerCode);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Customer code '{request.CustomerCode}' already exists");
                }

                // Business Rule 2: Validate required fields
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return ApiResponse<int>.ErrorResponse("Customer name is required");
                }

                // Business Rule 3: Validate credit limit if credit days are specified
                if (request.CreditDays.HasValue && request.CreditDays.Value > 0 && !request.CreditLimit.HasValue)
                {
                    return ApiResponse<int>.ErrorResponse("Credit limit is required when credit days are specified");
                }

                // Business Rule 4: Validate GST format if provided
                if (!string.IsNullOrWhiteSpace(request.GSTNo) && request.GSTNo.Length != 15)
                {
                    return ApiResponse<int>.ErrorResponse("GST number must be 15 characters long");
                }

                // Create Customer
                var customer = new Customer
                {
                    CustomerCode = request.CustomerCode.Trim().ToUpper(),
                    CustomerName = request.CustomerName.Trim(),
                    CustomerType = request.CustomerType.Trim(),
                    ContactPerson = request.ContactPerson?.Trim(),
                    Email = request.Email?.Trim().ToLower(),
                    Phone = request.Phone?.Trim(),
                    Address = request.Address?.Trim(),
                    City = request.City?.Trim(),
                    State = request.State?.Trim(),
                    Country = request.Country?.Trim() ?? "India",
                    PinCode = request.PinCode?.Trim(),
                    GSTNo = request.GSTNo?.Trim().ToUpper(),
                    PANNo = request.PANNo?.Trim().ToUpper(),
                    CreditDays = request.CreditDays ?? 0,
                    CreditLimit = request.CreditLimit ?? 0,
                    PaymentTerms = request.PaymentTerms ?? "Net 30 Days",
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var customerId = await _customerRepository.InsertAsync(customer);

                return ApiResponse<int>.SuccessResponse(customerId, $"Customer '{request.CustomerCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateCustomerAsync(UpdateCustomerRequest request)
        {
            try
            {
                // Get existing customer
                var existingCustomer = await _customerRepository.GetByIdAsync(request.Id);
                if (existingCustomer == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer not found");
                }

                // Business Rule 1: Validate Customer Code uniqueness if changed
                if (existingCustomer.CustomerCode != request.CustomerCode)
                {
                    var exists = await _customerRepository.ExistsAsync(request.CustomerCode);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Customer code '{request.CustomerCode}' already exists");
                    }
                }

                // Business Rule 2: Validate GST format if provided
                if (!string.IsNullOrWhiteSpace(request.GSTNo) && request.GSTNo.Length != 15)
                {
                    return ApiResponse<bool>.ErrorResponse("GST number must be 15 characters long");
                }

                // Update customer
                existingCustomer.CustomerCode = request.CustomerCode.Trim().ToUpper();
                existingCustomer.CustomerName = request.CustomerName.Trim();
                existingCustomer.CustomerType = request.CustomerType.Trim();
                existingCustomer.ContactPerson = request.ContactPerson?.Trim();
                existingCustomer.Email = request.Email?.Trim().ToLower();
                existingCustomer.Phone = request.Phone?.Trim();
                existingCustomer.Address = request.Address?.Trim();
                existingCustomer.City = request.City?.Trim();
                existingCustomer.State = request.State?.Trim();
                existingCustomer.Country = request.Country?.Trim();
                existingCustomer.PinCode = request.PinCode?.Trim();
                existingCustomer.GSTNo = request.GSTNo?.Trim().ToUpper();
                existingCustomer.PANNo = request.PANNo?.Trim().ToUpper();
                existingCustomer.CreditDays = request.CreditDays ?? 0;
                existingCustomer.CreditLimit = request.CreditLimit ?? 0;
                existingCustomer.PaymentTerms = request.PaymentTerms?.Trim();
                existingCustomer.IsActive = request.IsActive;
                existingCustomer.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                var success = await _customerRepository.UpdateAsync(existingCustomer);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update customer. Please try again.");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Customer updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer not found");
                }

                // Business Rule: For now, allow deletion. In production, check if customer has orders
                // TODO: Add check for existing orders before deletion

                var success = await _customerRepository.DeleteAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete customer");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ActivateCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer not found");
                }

                if (customer.IsActive)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer is already active");
                }

                var success = await _customerRepository.ActivateAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to activate customer");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Customer activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer not found");
                }

                if (!customer.IsActive)
                {
                    return ApiResponse<bool>.ErrorResponse("Customer is already inactive");
                }

                var success = await _customerRepository.DeactivateAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to deactivate customer");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Customer deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating customer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse("Search term is required");
                }

                var customers = await _customerRepository.SearchByNameAsync(searchTerm);
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error searching customers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByCustomerTypeAsync(string customerType)
        {
            try
            {
                var customers = await _customerRepository.GetByCustomerTypeAsync(customerType);
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error retrieving customers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByCityAsync(string city)
        {
            try
            {
                var customers = await _customerRepository.GetByCityAsync(city);
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error retrieving customers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<CustomerResponse>>> GetByStateAsync(string state)
        {
            try
            {
                var customers = await _customerRepository.GetByStateAsync(state);
                var responses = customers.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<CustomerResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CustomerResponse>>.ErrorResponse($"Error retrieving customers: {ex.Message}");
            }
        }

        // Helper Methods

        private static CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                CustomerCode = customer.CustomerCode,
                CustomerName = customer.CustomerName,
                CustomerType = customer.CustomerType,
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                PinCode = customer.PinCode,
                GSTNo = customer.GSTNo,
                PANNo = customer.PANNo,
                CreditDays = customer.CreditDays,
                CreditLimit = customer.CreditLimit,
                PaymentTerms = customer.PaymentTerms,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                CreatedBy = customer.CreatedBy,
                UpdatedAt = customer.UpdatedAt,
                UpdatedBy = customer.UpdatedBy
            };
        }
    }
}
