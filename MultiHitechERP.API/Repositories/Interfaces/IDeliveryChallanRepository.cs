using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Dispatch;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Delivery Challan operations
    /// </summary>
    public interface IDeliveryChallanRepository
    {
        // Basic CRUD Operations
        Task<DeliveryChallan?> GetByIdAsync(int id);
        Task<DeliveryChallan?> GetByChallanNoAsync(string challanNo);
        Task<IEnumerable<DeliveryChallan>> GetAllAsync();
        Task<IEnumerable<DeliveryChallan>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<DeliveryChallan>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<DeliveryChallan>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<int> InsertAsync(DeliveryChallan challan);
        Task<bool> UpdateAsync(DeliveryChallan challan);
        Task<bool> DeleteAsync(int id);

        // Status Operations
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> DispatchChallanAsync(int id, DateTime dispatchedAt);
        Task<bool> DeliverChallanAsync(int id, DateTime deliveredAt, string receivedBy);

        // Queries
        Task<IEnumerable<DeliveryChallan>> GetPendingChallansAsync();
        Task<IEnumerable<DeliveryChallan>> GetDispatchedChallansAsync();
        Task<IEnumerable<DeliveryChallan>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DeliveryChallan>> GetByVehicleNumberAsync(string vehicleNumber);
    }
}
