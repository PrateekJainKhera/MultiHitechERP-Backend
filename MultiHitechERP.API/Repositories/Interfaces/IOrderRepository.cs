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
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByOrderNoAsync(string orderNo);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<int> InsertAsync(Order order);
        Task<bool> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);

        // Business Operations
        Task<bool> UpdateDrawingReviewStatusAsync(int id, string status, string reviewedBy, string? notes);
        Task<bool> UpdatePlanningStatusAsync(int id, string status);
        Task<bool> UpdateQuantitiesAsync(int id, int qtyCompleted, int qtyRejected, int qtyInProgress);
        Task<bool> UpdateStatusAsync(int id, string status);

        // Queries
        Task<IEnumerable<Order>> GetPendingDrawingReviewAsync();
        Task<IEnumerable<Order>> GetReadyForPlanningAsync();
        Task<IEnumerable<Order>> GetInProgressOrdersAsync();
        Task<IEnumerable<Order>> GetDelayedOrdersAsync();

        // Optimistic Locking
        Task<bool> UpdateWithVersionCheckAsync(Order order);
        Task<int> GetVersionAsync(int id);
    }
}
