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
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByMaterialIdAsync(int materialId);
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<int> InsertAsync(Inventory inventory);
        Task<bool> UpdateAsync(Inventory inventory);
        Task<bool> DeleteAsync(int id);

        // Stock Queries
        Task<IEnumerable<Inventory>> GetLowStockAsync();
        Task<IEnumerable<Inventory>> GetOutOfStockAsync();
        Task<IEnumerable<Inventory>> GetByCategoryAsync(string category);
        Task<IEnumerable<Inventory>> GetByLocationAsync(string location);
        Task<IEnumerable<Inventory>> GetActiveAsync();

        // Stock Updates
        Task<bool> UpdateStockLevelsAsync(int materialId, decimal totalQty, decimal availableQty, decimal allocatedQty, decimal issuedQty);
        Task<bool> UpdateAverageCostAsync(int materialId, decimal avgCost, decimal totalValue);
        Task<bool> UpdateStockStatusAsync(int materialId, bool isLowStock, bool isOutOfStock);

        // Transactions
        Task<int> InsertTransactionAsync(InventoryTransaction transaction);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByMaterialIdAsync(int materialId);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByTypeAsync(string transactionType);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByJobCardIdAsync(int jobCardId);
        Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(int count);

        // Reconciliation
        Task<bool> ReconcileStockAsync(int materialId, decimal actualQuantity, string performedBy, string remarks);

        // Component Stock
        Task<(decimal currentStock, decimal availableStock, string uom, string location)> GetComponentStockAsync(int componentId);
        Task<bool> DeductComponentStockAsync(int componentId, decimal quantity, string updatedBy);

        // GRN Integration - Upsert inventory from GRN data
        Task<bool> UpsertFromGRNAsync(
            int materialId,
            string materialCode,
            string materialName,
            decimal lengthToAdd,
            string uom,
            string location,
            string updatedBy,
            string itemType = "RawMaterial");  // Added itemType parameter
    }
}
