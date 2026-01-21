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
        Task<ApiResponse<DeliveryChallan>> GetByIdAsync(Guid id);
        Task<ApiResponse<DeliveryChallan>> GetByChallanNoAsync(string challanNo);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateChallanAsync(DeliveryChallan challan);
        Task<ApiResponse<bool>> UpdateChallanAsync(DeliveryChallan challan);
        Task<ApiResponse<bool>> DeleteChallanAsync(Guid id);

        // Queries
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByOrderIdAsync(Guid orderId);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByCustomerIdAsync(Guid customerId);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetByVehicleNumberAsync(string vehicleNumber);

        // Dispatch Operations
        Task<ApiResponse<Guid>> CreateDispatchChallanAsync(Guid orderId, int quantityDispatched, string deliveryAddress, string? transportMode, string? vehicleNumber, string? driverName, string? driverContact);
        Task<ApiResponse<bool>> DispatchChallanAsync(Guid id);
        Task<ApiResponse<bool>> DeliverChallanAsync(Guid id, string receivedBy);
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);

        // Status Queries
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetPendingChallansAsync();
        Task<ApiResponse<IEnumerable<DeliveryChallan>>> GetDispatchedChallansAsync();
    }
}
