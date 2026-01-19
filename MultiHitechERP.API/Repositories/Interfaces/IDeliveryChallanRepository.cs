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
        Task<DeliveryChallan?> GetByIdAsync(Guid id);
        Task<DeliveryChallan?> GetByChallanNoAsync(string challanNo);
        Task<IEnumerable<DeliveryChallan>> GetAllAsync();
        Task<IEnumerable<DeliveryChallan>> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<DeliveryChallan>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<DeliveryChallan>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<Guid> InsertAsync(DeliveryChallan challan);
        Task<bool> UpdateAsync(DeliveryChallan challan);
        Task<bool> DeleteAsync(Guid id);

        // Status Operations
        Task<bool> UpdateStatusAsync(Guid id, string status);
        Task<bool> DispatchChallanAsync(Guid id, DateTime dispatchedAt);
        Task<bool> DeliverChallanAsync(Guid id, DateTime deliveredAt, string receivedBy);

        // Queries
        Task<IEnumerable<DeliveryChallan>> GetPendingChallansAsync();
        Task<IEnumerable<DeliveryChallan>> GetDispatchedChallansAsync();
        Task<IEnumerable<DeliveryChallan>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<DeliveryChallan>> GetByVehicleNumberAsync(string vehicleNumber);
    }
}
