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
        Task<ApiResponse<Inventory>> GetByIdAsync(int id);
        Task<ApiResponse<Inventory>> GetByMaterialIdAsync(int materialId);
        Task<ApiResponse<IEnumerable<Inventory>>> GetAllAsync();
        Task<ApiResponse<int>> CreateInventoryAsync(Inventory inventory);
        Task<ApiResponse<bool>> UpdateInventoryAsync(Inventory inventory);
        Task<ApiResponse<bool>> DeleteInventoryAsync(int id);

        // Stock Queries
        Task<ApiResponse<IEnumerable<Inventory>>> GetLowStockAsync();
        Task<ApiResponse<IEnumerable<Inventory>>> GetOutOfStockAsync();
        Task<ApiResponse<IEnumerable<Inventory>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<Inventory>>> GetByLocationAsync(string location);
        Task<ApiResponse<IEnumerable<Inventory>>> GetActiveAsync();

        // Stock Operations
        Task<ApiResponse<int>> RecordStockInAsync(int materialId, decimal quantity, string grnNo, int? supplierId, decimal? unitCost, string performedBy, string remarks);
        Task<ApiResponse<int>> RecordStockOutAsync(int materialId, decimal quantity, int? jobCardId, int? requisitionId, string performedBy, string remarks);
        Task<ApiResponse<int>> RecordStockAdjustmentAsync(int materialId, decimal quantity, string remarks, string performedBy);
        Task<ApiResponse<bool>> ReconcileStockAsync(int materialId, decimal actualQuantity, string performedBy, string remarks);

        // Component Stock
        Task<(decimal currentStock, decimal availableStock, string uom, string location)> GetComponentStockAsync(int componentId);

        // Component Receipt
        Task<ApiResponse<int>> ReceiveComponentAsync(
            int componentId,
            string componentName,
            string partNumber,
            decimal quantity,
            string unit,
            decimal? unitCost,
            int? supplierId,
            string supplierName,
            string invoiceNo,
            DateTime? invoiceDate,
            string poNo,
            DateTime? poDate,
            DateTime receiptDate,
            string storageLocation,
            string remarks,
            string receivedBy,
            int? warehouseId = null);

        // Transactions
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByMaterialIdAsync(int materialId);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByTypeAsync(string transactionType);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetRecentTransactionsAsync(int count);

        // Stock Level Management
        Task<ApiResponse<bool>> UpdateStockLevelsAsync(int materialId, decimal? minStock, decimal? maxStock, decimal? reorderLevel, decimal? reorderQuantity);
        Task<ApiResponse<bool>> CheckAndUpdateStockStatusAsync(int materialId);
    }
}
