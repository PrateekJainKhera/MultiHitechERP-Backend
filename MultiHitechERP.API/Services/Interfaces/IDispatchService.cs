using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Dispatch;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Dispatch business logic
    /// Manages delivery challans and dispatch operations
    /// </summary>
    public interface IDispatchService
    {
        // Basic CRUD Operations
        Task<ApiResponse<DeliveryChallan>> GetByIdAsync(int id);
        Task<ApiResponse<DeliveryChallan>> GetByChallanNoAsync(string challanNo);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetAllAsync();
        Task<ApiResponse<int>> CreateChallanAsync(DeliveryChallan challan);
        Task<ApiResponse<bool>> UpdateChallanAsync(DeliveryChallan challan);
        Task<ApiResponse<bool>> DeleteChallanAsync(int id);

        // Queries
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByOrderIdAsync(int orderId);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByCustomerIdAsync(int customerId);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByVehicleNumberAsync(string vehicleNumber);

        // Dispatch Operations
        Task<ApiResponse<int>> CreateDispatchChallanAsync(int orderId, int quantityDispatched, string deliveryAddress, string? transportMode, string? vehicleNumber, string? driverName, string? driverContact);
        Task<ApiResponse<bool>> DispatchChallanAsync(int id);
        Task<ApiResponse<bool>> DeliverChallanAsync(int id, string receivedBy);
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);

        // Status Queries
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetPendingChallansAsync();
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetDispatchedChallansAsync();
    }
}
