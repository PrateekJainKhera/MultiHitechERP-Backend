using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Orders;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Order operations
    /// </summary>
    public interface IOrderRepository
    {
        // Basic CRUD Operations
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> GetByOrderNoAsync(string orderNo);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<Order>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<Guid> InsertAsync(Order order);
        Task<bool> UpdateAsync(Order order);
        Task<bool> DeleteAsync(Guid id);

        // Business Operations
        Task<bool> UpdateDrawingReviewStatusAsync(Guid id, string status, string reviewedBy, string? notes);
        Task<bool> UpdatePlanningStatusAsync(Guid id, string status);
        Task<bool> UpdateQuantitiesAsync(Guid id, int qtyCompleted, int qtyRejected, int qtyInProgress);
        Task<bool> UpdateStatusAsync(Guid id, string status);

        // Queries
        Task<IEnumerable<Order>> GetPendingDrawingReviewAsync();
        Task<IEnumerable<Order>> GetReadyForPlanningAsync();
        Task<IEnumerable<Order>> GetInProgressOrdersAsync();
        Task<IEnumerable<Order>> GetDelayedOrdersAsync();

        // Optimistic Locking
        Task<bool> UpdateWithVersionCheckAsync(Order order);
        Task<int> GetVersionAsync(Guid id);
    }
}
