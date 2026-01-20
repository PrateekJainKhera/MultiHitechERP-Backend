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

        public async Task<ApiResponse<CustomerResponse>> GetByIdAsync(Guid id)
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

        public async Task<ApiResponse<Guid>> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                // Business Rule 1: Validate Customer Code is unique
                var exists = await _customerRepository.ExistsAsync(request.CustomerCode);
                if (exists)
                {
                    return ApiResponse<Guid>.ErrorResponse($"Customer code '{request.CustomerCode}' already exists");
                }

                // Business Rule 2: Validate required fields
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return ApiResponse<Guid>.ErrorResponse("Customer name is required");
                }

                // Business Rule 3: Validate credit limit if credit days are specified
                if (request.CreditDays.HasValue && request.CreditDays.Value > 0 && !request.CreditLimit.HasValue)
                {
                    return ApiResponse<Guid>.ErrorResponse("Credit limit is required when credit days are specified");
                }

                // Business Rule 4: Validate GST format if provided
                if (!string.IsNullOrWhiteSpace(request.GSTNumber) && request.GSTNumber.Length != 15)
                {
                    return ApiResponse<Guid>.ErrorResponse("GST number must be 15 characters long");
                }

                // Create Customer
                var customer = new Customer
                {
                    CustomerCode = request.CustomerCode.Trim().ToUpper(),
                    CustomerName = request.CustomerName.Trim(),
                    ContactPerson = request.ContactPerson?.Trim(),
                    Email = request.Email?.Trim().ToLower(),
                    Phone = request.Phone?.Trim(),
                    Mobile = request.Mobile?.Trim(),
                    Address = request.Address?.Trim(),
                    City = request.City?.Trim(),
                    State = request.State?.Trim(),
                    Country = request.Country?.Trim() ?? "India",
                    PinCode = request.PinCode?.Trim(),
                    GSTNumber = request.GSTNumber?.Trim().ToUpper(),
                    PANNumber = request.PANNumber?.Trim().ToUpper(),
                    CustomerType = request.CustomerType ?? "Regular",
                    Industry = request.Industry?.Trim(),
                    CreditDays = request.CreditDays ?? 0,
                    CreditLimit = request.CreditLimit ?? 0,
                    PaymentTerms = request.PaymentTerms ?? "Net 30 Days",
                    IsActive = true,
                    Status = "Active",
                    CustomerRating = request.CustomerRating ?? "Standard",
                    Classification = request.Classification ?? "B",
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var customerId = await _customerRepository.InsertAsync(customer);

                return ApiResponse<Guid>.SuccessResponse(customerId, $"Customer '{request.CustomerCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating customer: {ex.Message}");
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
                if (!string.IsNullOrWhiteSpace(request.GSTNumber) && request.GSTNumber.Length != 15)
                {
                    return ApiResponse<bool>.ErrorResponse("GST number must be 15 characters long");
                }

                // Update customer
                existingCustomer.CustomerCode = request.CustomerCode.Trim().ToUpper();
                existingCustomer.CustomerName = request.CustomerName.Trim();
                existingCustomer.ContactPerson = request.ContactPerson?.Trim();
                existingCustomer.Email = request.Email?.Trim().ToLower();
                existingCustomer.Phone = request.Phone?.Trim();
                existingCustomer.Mobile = request.Mobile?.Trim();
                existingCustomer.Address = request.Address?.Trim();
                existingCustomer.City = request.City?.Trim();
                existingCustomer.State = request.State?.Trim();
                existingCustomer.Country = request.Country?.Trim();
                existingCustomer.PinCode = request.PinCode?.Trim();
                existingCustomer.GSTNumber = request.GSTNumber?.Trim().ToUpper();
                existingCustomer.PANNumber = request.PANNumber?.Trim().ToUpper();
                existingCustomer.CustomerType = request.CustomerType;
                existingCustomer.Industry = request.Industry?.Trim();
                existingCustomer.CreditDays = request.CreditDays;
                existingCustomer.CreditLimit = request.CreditLimit;
                existingCustomer.PaymentTerms = request.PaymentTerms?.Trim();
                existingCustomer.IsActive = request.IsActive;
                existingCustomer.Status = request.Status;
                existingCustomer.CustomerRating = request.CustomerRating;
                existingCustomer.Classification = request.Classification;
                existingCustomer.Remarks = request.Remarks?.Trim();
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

        public async Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id)
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

        public async Task<ApiResponse<bool>> ActivateCustomerAsync(Guid id)
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

        public async Task<ApiResponse<bool>> DeactivateCustomerAsync(Guid id)
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
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                Phone = customer.Phone,
                Mobile = customer.Mobile,
                Address = customer.Address,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                PinCode = customer.PinCode,
                GSTNumber = customer.GSTNumber,
                PANNumber = customer.PANNumber,
                CustomerType = customer.CustomerType,
                Industry = customer.Industry,
                CreditDays = customer.CreditDays,
                CreditLimit = customer.CreditLimit,
                PaymentTerms = customer.PaymentTerms,
                IsActive = customer.IsActive,
                Status = customer.Status,
                CustomerRating = customer.CustomerRating,
                Classification = customer.Classification,
                Remarks = customer.Remarks,
                CreatedAt = customer.CreatedAt,
                CreatedBy = customer.CreatedBy,
                UpdatedAt = customer.UpdatedAt,
                UpdatedBy = customer.UpdatedBy
            };
        }
    }
}
