using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Inventory;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Inventory operations
    /// </summary>
    public interface IInventoryRepository
    {
        // Basic CRUD
        Task<Inventory?> GetByIdAsync(Guid id);
        Task<Inventory?> GetByMaterialIdAsync(Guid materialId);
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Guid> InsertAsync(Inventory inventory);
        Task<bool> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(Guid id);

        // Stock Queries
        Task<IEnumerable<Inventory>> GetLowStockAsync();
        Task<IEnumerable<Inventory>> GetOutOfStockAsync();
        Task<IEnumerable<Inventory>> GetByCategoryAsync(string category);
        Task<IEnumerable<Inventory>> GetByLocationAsync(string location);
        Task<IEnumerable<Inventory>> GetActiveAsync();

        // Stock Updates
        Task<bool> UpdateStockLevelsAsync(Guid materialId, decimal totalQty, decimal availableQty, decimal allocatedQty, decimal issuedQty);
        Task<bool> UpdateAverageCostAsync(Guid materialId, decimal avgCost, decimal totalValue);
        Task<bool> UpdateStockStatusAsync(Guid materialId, bool isLowStock, bool isOutOfStock);

        // Transactions
        Task<Guid> InsertTransactionAsync(InventoryTransaction transaction);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByMaterialIdAsync(Guid materialId);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByTypeAsync(string transactionType);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByJobCardIdAsync(Guid jobCardId);
        Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(int count);

        // Reconciliation
        Task<bool> ReconcileStockAsync(Guid materialId, decimal actualQuantity, string performedBy, string remarks);
    }
}
