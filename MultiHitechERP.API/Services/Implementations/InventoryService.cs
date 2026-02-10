using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Inventory;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Inventory business logic
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMaterialRepository _materialRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IMaterialRepository materialRepository)
        {
            _inventoryRepository = inventoryRepository;
            _materialRepository = materialRepository;
        }

        public async Task<ApiResponse<Inventory>> GetByIdAsync(int id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return ApiResponse<Inventory>.ErrorResponse("Inventory record not found");

            return ApiResponse<Inventory>.SuccessResponse(inventory);
        }

        public async Task<ApiResponse<Inventory>> GetByMaterialIdAsync(int materialId)
        {
            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return ApiResponse<Inventory>.ErrorResponse("Inventory record not found for this material");

            return ApiResponse<Inventory>.SuccessResponse(inventory);
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetAllAsync()
        {
            var inventories = await _inventoryRepository.GetAllAsync();
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<int>> CreateInventoryAsync(Inventory inventory)
        {
            // Validate material exists
            var material = await _materialRepository.GetByIdAsync(inventory.MaterialId);
            if (material == null)
                return ApiResponse<int>.ErrorResponse("Material not found");

            // Check if inventory already exists for this material
            var existing = await _inventoryRepository.GetByMaterialIdAsync(inventory.MaterialId);
            if (existing != null)
                return ApiResponse<int>.ErrorResponse("Inventory record already exists for this material");

            // Set material details
            inventory.MaterialCode = null; // MaterialCode removed from Material master
            inventory.MaterialName = material.MaterialName;
            inventory.MaterialCategory = material.Grade; // Use Grade as category reference
            inventory.UOM = "KG"; // Default UOM

            // Initialize stock status
            inventory.IsOutOfStock = inventory.TotalQuantity == 0;
            inventory.IsLowStock = inventory.MinimumStock.HasValue && inventory.AvailableQuantity <= inventory.MinimumStock.Value;

            var id = await _inventoryRepository.InsertAsync(inventory);
            return ApiResponse<int>.SuccessResponse(id, "Inventory record created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateInventoryAsync(Inventory inventory)
        {
            var existing = await _inventoryRepository.GetByIdAsync(inventory.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Inventory record not found");

            var success = await _inventoryRepository.UpdateAsync(inventory);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update inventory record");

            return ApiResponse<bool>.SuccessResponse(true, "Inventory record updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteInventoryAsync(int id)
        {
            var existing = await _inventoryRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Inventory record not found");

            var success = await _inventoryRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete inventory record");

            return ApiResponse<bool>.SuccessResponse(true, "Inventory record deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetLowStockAsync()
        {
            var inventories = await _inventoryRepository.GetLowStockAsync();
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetOutOfStockAsync()
        {
            var inventories = await _inventoryRepository.GetOutOfStockAsync();
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return ApiResponse<IEnumerable<Inventory>>.ErrorResponse("Category is required");

            var inventories = await _inventoryRepository.GetByCategoryAsync(category);
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetByLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return ApiResponse<IEnumerable<Inventory>>.ErrorResponse("Location is required");

            var inventories = await _inventoryRepository.GetByLocationAsync(location);
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<IEnumerable<Inventory>>> GetActiveAsync()
        {
            var inventories = await _inventoryRepository.GetActiveAsync();
            return ApiResponse<IEnumerable<Inventory>>.SuccessResponse(inventories);
        }

        public async Task<ApiResponse<int>> RecordStockInAsync(
            int materialId,
            decimal quantity,
            string grnNo,
            int? supplierId,
            decimal? unitCost,
            string performedBy,
            string remarks)
        {
            if (quantity <= 0)
                return ApiResponse<int>.ErrorResponse("Quantity must be greater than zero");

            var material = await _materialRepository.GetByIdAsync(materialId);
            if (material == null)
                return ApiResponse<int>.ErrorResponse("Material not found");

            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
            {
                // Create new inventory record
                inventory = new Inventory
                {
                    MaterialId = materialId,
                    MaterialCode = null, // MaterialCode removed from Material master
                    MaterialName = material.MaterialName,
                    MaterialCategory = material.Grade, // Use Grade as category reference
                    UOM = "KG", // Default UOM
                    TotalQuantity = quantity,
                    AvailableQuantity = quantity,
                    AllocatedQuantity = 0,
                    IssuedQuantity = 0,
                    LastStockInDate = DateTime.UtcNow
                };

                await _inventoryRepository.InsertAsync(inventory);
                inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            }
            else
            {
                // Update existing inventory
                var newTotal = inventory.TotalQuantity + quantity;
                var newAvailable = inventory.AvailableQuantity + quantity;

                await _inventoryRepository.UpdateStockLevelsAsync(
                    materialId,
                    newTotal,
                    newAvailable,
                    inventory.AllocatedQuantity,
                    inventory.IssuedQuantity
                );

                inventory.TotalQuantity = newTotal;
                inventory.AvailableQuantity = newAvailable;
            }

            // Update average cost if provided
            if (unitCost.HasValue)
            {
                var currentValue = (inventory.TotalQuantity - quantity) * (inventory.AverageCostPerUnit ?? 0);
                var newValue = quantity * unitCost.Value;
                var totalValue = currentValue + newValue;
                var avgCost = totalValue / inventory.TotalQuantity;

                await _inventoryRepository.UpdateAverageCostAsync(materialId, avgCost, totalValue);
            }

            // Create transaction
            var transaction = new InventoryTransaction
            {
                MaterialId = materialId,
                TransactionType = "StockIn",
                TransactionNo = $"SI-{DateTime.UtcNow:yyyyMMddHHmmss}",
                TransactionDate = DateTime.UtcNow,
                Quantity = quantity,
                UOM = inventory.UOM,
                ReferenceType = "GRN",
                ReferenceNo = grnNo,
                UnitCost = unitCost,
                TotalCost = unitCost.HasValue ? quantity * unitCost.Value : null,
                BalanceQuantity = inventory.TotalQuantity,
                Remarks = remarks,
                PerformedBy = performedBy,
                SupplierId = supplierId,
                GRNNo = grnNo,
                CreatedBy = performedBy
            };

            var transactionId = await _inventoryRepository.InsertTransactionAsync(transaction);

            // Check and update stock status
            await CheckAndUpdateStockStatusAsync(materialId);

            return ApiResponse<int>.SuccessResponse(transactionId, $"Stock in recorded: {quantity} {inventory.UOM}");
        }

        public async Task<ApiResponse<int>> RecordStockOutAsync(
            int materialId,
            decimal quantity,
            int? jobCardId,
            int? requisitionId,
            string performedBy,
            string remarks)
        {
            if (quantity <= 0)
                return ApiResponse<int>.ErrorResponse("Quantity must be greater than zero");

            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return ApiResponse<int>.ErrorResponse("Inventory record not found for this material");

            if (inventory.AvailableQuantity < quantity)
                return ApiResponse<int>.ErrorResponse($"Insufficient stock. Available: {inventory.AvailableQuantity} {inventory.UOM}");

            // Update inventory
            var newTotal = inventory.TotalQuantity - quantity;
            var newAvailable = inventory.AvailableQuantity - quantity;
            var newIssued = inventory.IssuedQuantity + quantity;

            await _inventoryRepository.UpdateStockLevelsAsync(
                materialId,
                newTotal,
                newAvailable,
                inventory.AllocatedQuantity,
                newIssued
            );

            // Create transaction
            var transaction = new InventoryTransaction
            {
                MaterialId = materialId,
                TransactionType = "StockOut",
                TransactionNo = $"SO-{DateTime.UtcNow:yyyyMMddHHmmss}",
                TransactionDate = DateTime.UtcNow,
                Quantity = -quantity, // Negative for stock out
                UOM = inventory.UOM,
                ReferenceType = "MaterialIssue",
                BalanceQuantity = newTotal,
                Remarks = remarks,
                PerformedBy = performedBy,
                JobCardId = jobCardId,
                RequisitionId = requisitionId,
                CreatedBy = performedBy
            };

            var transactionId = await _inventoryRepository.InsertTransactionAsync(transaction);

            // Check and update stock status
            await CheckAndUpdateStockStatusAsync(materialId);

            return ApiResponse<int>.SuccessResponse(transactionId, $"Stock out recorded: {quantity} {inventory.UOM}");
        }

        public async Task<ApiResponse<int>> RecordStockAdjustmentAsync(
            int materialId,
            decimal quantity,
            string remarks,
            string performedBy)
        {
            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return ApiResponse<int>.ErrorResponse("Inventory record not found for this material");

            // Update inventory
            var newTotal = inventory.TotalQuantity + quantity;
            var newAvailable = inventory.AvailableQuantity + quantity;

            if (newTotal < 0 || newAvailable < 0)
                return ApiResponse<int>.ErrorResponse("Adjustment would result in negative stock");

            await _inventoryRepository.UpdateStockLevelsAsync(
                materialId,
                newTotal,
                newAvailable,
                inventory.AllocatedQuantity,
                inventory.IssuedQuantity
            );

            // Create transaction
            var transaction = new InventoryTransaction
            {
                MaterialId = materialId,
                TransactionType = "Adjustment",
                TransactionNo = $"ADJ-{DateTime.UtcNow:yyyyMMddHHmmss}",
                TransactionDate = DateTime.UtcNow,
                Quantity = quantity,
                UOM = inventory.UOM,
                ReferenceType = "ManualAdjustment",
                BalanceQuantity = newTotal,
                Remarks = remarks,
                PerformedBy = performedBy,
                CreatedBy = performedBy
            };

            var transactionId = await _inventoryRepository.InsertTransactionAsync(transaction);

            // Check and update stock status
            await CheckAndUpdateStockStatusAsync(materialId);

            return ApiResponse<int>.SuccessResponse(transactionId, $"Stock adjusted by {quantity} {inventory.UOM}");
        }

        public async Task<ApiResponse<bool>> ReconcileStockAsync(
            int materialId,
            decimal actualQuantity,
            string performedBy,
            string remarks)
        {
            if (actualQuantity < 0)
                return ApiResponse<bool>.ErrorResponse("Actual quantity cannot be negative");

            var success = await _inventoryRepository.ReconcileStockAsync(materialId, actualQuantity, performedBy, remarks);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to reconcile stock");

            // Check and update stock status
            await CheckAndUpdateStockStatusAsync(materialId);

            return ApiResponse<bool>.SuccessResponse(true, "Stock reconciled successfully");
        }

        public async Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByMaterialIdAsync(int materialId)
        {
            var transactions = await _inventoryRepository.GetTransactionsByMaterialIdAsync(materialId);
            return ApiResponse<IEnumerable<InventoryTransaction>>.SuccessResponse(transactions);
        }

        public async Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByTypeAsync(string transactionType)
        {
            if (string.IsNullOrWhiteSpace(transactionType))
                return ApiResponse<IEnumerable<InventoryTransaction>>.ErrorResponse("Transaction type is required");

            var transactions = await _inventoryRepository.GetTransactionsByTypeAsync(transactionType);
            return ApiResponse<IEnumerable<InventoryTransaction>>.SuccessResponse(transactions);
        }

        public async Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return ApiResponse<IEnumerable<InventoryTransaction>>.ErrorResponse("Start date must be before end date");

            var transactions = await _inventoryRepository.GetTransactionsByDateRangeAsync(startDate, endDate);
            return ApiResponse<IEnumerable<InventoryTransaction>>.SuccessResponse(transactions);
        }

        public async Task<ApiResponse<IEnumerable<InventoryTransaction>>> GetRecentTransactionsAsync(int count)
        {
            if (count <= 0 || count > 1000)
                return ApiResponse<IEnumerable<InventoryTransaction>>.ErrorResponse("Count must be between 1 and 1000");

            var transactions = await _inventoryRepository.GetRecentTransactionsAsync(count);
            return ApiResponse<IEnumerable<InventoryTransaction>>.SuccessResponse(transactions);
        }

        public async Task<ApiResponse<bool>> UpdateStockLevelsAsync(
            int materialId,
            decimal? minStock,
            decimal? maxStock,
            decimal? reorderLevel,
            decimal? reorderQuantity)
        {
            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return ApiResponse<bool>.ErrorResponse("Inventory record not found for this material");

            // Validate stock levels
            if (minStock.HasValue && maxStock.HasValue && minStock.Value > maxStock.Value)
                return ApiResponse<bool>.ErrorResponse("Minimum stock cannot be greater than maximum stock");

            if (reorderLevel.HasValue && minStock.HasValue && reorderLevel.Value < minStock.Value)
                return ApiResponse<bool>.ErrorResponse("Reorder level should be greater than or equal to minimum stock");

            // Update inventory
            inventory.MinimumStock = minStock;
            inventory.MaximumStock = maxStock;
            inventory.ReorderLevel = reorderLevel;
            inventory.ReorderQuantity = reorderQuantity;

            var success = await _inventoryRepository.UpdateAsync(inventory);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update stock levels");

            // Check and update stock status
            await CheckAndUpdateStockStatusAsync(materialId);

            return ApiResponse<bool>.SuccessResponse(true, "Stock levels updated successfully");
        }

        public async Task<ApiResponse<bool>> CheckAndUpdateStockStatusAsync(int materialId)
        {
            var inventory = await _inventoryRepository.GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return ApiResponse<bool>.ErrorResponse("Inventory record not found");

            bool isOutOfStock = inventory.AvailableQuantity == 0;
            bool isLowStock = false;

            if (!isOutOfStock && inventory.ReorderLevel.HasValue)
            {
                isLowStock = inventory.AvailableQuantity <= inventory.ReorderLevel.Value;
            }
            else if (!isOutOfStock && inventory.MinimumStock.HasValue)
            {
                isLowStock = inventory.AvailableQuantity <= inventory.MinimumStock.Value;
            }

            await _inventoryRepository.UpdateStockStatusAsync(materialId, isLowStock, isOutOfStock);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<(decimal currentStock, decimal availableStock, string uom, string location)> GetComponentStockAsync(int componentId)
        {
            return await _inventoryRepository.GetComponentStockAsync(componentId);
        }

        public async Task<ApiResponse<int>> ReceiveComponentAsync(
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
            string receivedBy)
        {
            try
            {
                // Generate receipt transaction number
                string transactionNo = $"CR-{DateTime.UtcNow:yyyyMMddHHmmss}";

                // Upsert inventory using UpsertFromGRNAsync (works for components too)
                // Using ItemType = "Component" to differentiate from raw materials
                await _inventoryRepository.UpsertFromGRNAsync(
                    componentId,
                    partNumber,
                    componentName,
                    quantity,
                    unit,
                    storageLocation,
                    receivedBy,
                    "Component"  // ✅ Pass "Component" as ItemType
                );

                // Create transaction record
                var transaction = new InventoryTransaction
                {
                    MaterialId = componentId, // Using MaterialId field for ComponentId (backwards compatible)
                    TransactionType = "ComponentReceipt",
                    TransactionNo = transactionNo,
                    TransactionDate = receiptDate,
                    Quantity = quantity,
                    UOM = unit,
                    ReferenceType = "ComponentReceipt",
                    ReferenceNo = invoiceNo ?? poNo ?? transactionNo,
                    FromLocation = supplierName ?? "External Supplier",
                    ToLocation = storageLocation,
                    UnitCost = unitCost,
                    TotalCost = unitCost.HasValue ? unitCost.Value * quantity : null,
                    BalanceQuantity = quantity, // This will be updated by the repository
                    Remarks = remarks ?? $"Component receipt from {supplierName}",
                    PerformedBy = receivedBy,
                    SupplierId = supplierId,
                    GRNNo = invoiceNo,
                    CreatedBy = receivedBy
                };

                var transactionId = await _inventoryRepository.InsertTransactionAsync(transaction);

                return ApiResponse<int>.SuccessResponse(
                    transactionId,
                    $"Component received successfully. Transaction: {transactionNo}");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Failed to receive component: {ex.Message}");
            }
        }
    }
}
