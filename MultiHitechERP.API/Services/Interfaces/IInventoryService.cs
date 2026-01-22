using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Inventory;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Inventory business logic
    /// </summary>
    public interface IInventoryService
    {
        // Basic CRUD
        Task<ApiResponse<Inventory>> GetByIdAsync(Guid id);
        Task<ApiResponse<Inventory>> GetByMaterialIdAsync(Guid materialId);
        Task<ApiResponse<IEnumerable<Inventory>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateInventoryAsync(Inventory inventory);
        Task<ApiResponse<bool>> UpdateInventoryAsync(Inventory inventory);
        Task<ApiResponse<bool>> DeleteInventoryAsync(Guid id);

        // Stock Queries
        Task<ApiResponse<IEnumerable<Inventory>>> GetLowStockAsync();
        Task<ApiResponse<IEnumerable<Inventory>>> GetOutOfStockAsync();
        Task<ApiResponse<IEnumerable<Inventory>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<Inventory>>> GetByLocationAsync(string location);
        Task<ApiResponse<IEnumerable<Inventory>>> GetActiveAsync();

        // Stock Operations
        Task<ApiResponse<Guid>> RecordStockInAsync(Guid materialId, decimal quantity, string grnNo, Guid? supplierId, decimal? unitCost, string performedBy, string remarks);
        Task<ApiResponse<Guid>> RecordStockOutAsync(Guid materialId, decimal quantity, Guid? jobCardId, Guid? requisitionId, string performedBy, string remarks);
        Task<ApiResponse<Guid>> RecordStockAdjustmentAsync(Guid materialId, decimal quantity, string remarks, string performedBy);
        Task<ApiResponse<bool>> ReconcileStockAsync(Guid materialId, decimal actualQuantity, string performedBy, string remarks);

        // Transactions
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByMaterialIdAsync(Guid materialId);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByTypeAsync(string transactionType);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetRecentTransactionsAsync(int count);

        // Stock Level Management
        Task<ApiResponse<bool>> UpdateStockLevelsAsync(Guid materialId, decimal? minStock, decimal? maxStock, decimal? reorderLevel, decimal? reorderQuantity);
        Task<ApiResponse<bool>> CheckAndUpdateStockStatusAsync(Guid materialId);
    }
}
