using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Models.Dispatch;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for OrderItem operations
    /// </summary>
    public interface IOrderItemRepository
    {
        /// <summary>
        /// Get all order items across all orders
        /// </summary>
        Task<List<OrderItem>> GetAllAsync();

        /// <summary>
        /// Get all items for a specific order
        /// </summary>
        Task<List<OrderItem>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Get a specific order item by ID
        /// </summary>
        Task<OrderItem?> GetByIdAsync(int id);

        /// <summary>
        /// Create a new order item
        /// </summary>
        Task<int> CreateAsync(OrderItem orderItem);

        /// <summary>
        /// Create multiple order items for an order
        /// </summary>
        Task<List<int>> CreateBatchAsync(List<OrderItem> orderItems);

        /// <summary>
        /// Update an existing order item
        /// </summary>
        Task<bool> UpdateAsync(OrderItem orderItem);

        /// <summary>
        /// Delete an order item
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Get the next available item sequence for an order (A, B, C, etc.)
        /// </summary>
        Task<string> GetNextItemSequenceAsync(int orderId);

        /// <summary>
        /// Check if a product already exists in an order
        /// </summary>
        Task<bool> ProductExistsInOrderAsync(int orderId, int productId);

        /// <summary>
        /// Update item quantities (completed, rejected, in progress)
        /// </summary>
        Task<bool> UpdateQuantitiesAsync(int itemId, int qtyCompleted, int qtyRejected, int qtyInProgress, int qtyScrap);

        /// <summary>
        /// Update item status
        /// </summary>
        Task<bool> UpdateStatusAsync(int itemId, string status);

        Task<bool> UpdateQtyDispatchedAsync(int itemId, int qtyToAdd);
        Task<List<ReadyToDispatchItem>> GetReadyToDispatchAsync();
    }
}
