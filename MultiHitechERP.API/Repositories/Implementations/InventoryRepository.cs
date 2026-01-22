using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Inventory;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Inventory operations
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public InventoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToInventory(reader);

            return null;
        }

        public async Task<Inventory?> GetByMaterialIdAsync(int materialId)
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE MaterialId = @MaterialId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToInventory(reader);

            return null;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                ORDER BY MaterialCode";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<int> InsertAsync(Inventory inventory)
        {
            const string query = @"
                INSERT INTO Inventory_Stock (
                    Id, MaterialId, MaterialCode, MaterialName, MaterialCategory,
                    TotalQuantity, AvailableQuantity, AllocatedQuantity, IssuedQuantity, ReservedQuantity,
                    UOM, MinimumStock, MaximumStock, ReorderLevel, ReorderQuantity,
                    PrimaryStorageLocation, WarehouseCode,
                    AverageCostPerUnit, TotalStockValue,
                    IsLowStock, IsOutOfStock, IsActive,
                    LastStockInDate, LastStockOutDate, LastCountDate,
                    CreatedAt
                )
                VALUES (
                    @Id, @MaterialId, @MaterialCode, @MaterialName, @MaterialCategory,
                    @TotalQuantity, @AvailableQuantity, @AllocatedQuantity, @IssuedQuantity, @ReservedQuantity,
                    @UOM, @MinimumStock, @MaximumStock, @ReorderLevel, @ReorderQuantity,
                    @PrimaryStorageLocation, @WarehouseCode,
                    @AverageCostPerUnit, @TotalStockValue,
                    @IsLowStock, @IsOutOfStock, @IsActive,
                    @LastStockInDate, @LastStockOutDate, @LastCountDate,
                    @CreatedAt
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = 0;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@MaterialId", inventory.MaterialId);
            command.Parameters.AddWithValue("@MaterialCode", (object?)inventory.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)inventory.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCategory", (object?)inventory.MaterialCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalQuantity", inventory.TotalQuantity);
            command.Parameters.AddWithValue("@AvailableQuantity", inventory.AvailableQuantity);
            command.Parameters.AddWithValue("@AllocatedQuantity", inventory.AllocatedQuantity);
            command.Parameters.AddWithValue("@IssuedQuantity", inventory.IssuedQuantity);
            command.Parameters.AddWithValue("@ReservedQuantity", inventory.ReservedQuantity);
            command.Parameters.AddWithValue("@UOM", inventory.UOM);
            command.Parameters.AddWithValue("@MinimumStock", (object?)inventory.MinimumStock ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaximumStock", (object?)inventory.MaximumStock ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderLevel", (object?)inventory.ReorderLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderQuantity", (object?)inventory.ReorderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@PrimaryStorageLocation", (object?)inventory.PrimaryStorageLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@WarehouseCode", (object?)inventory.WarehouseCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@AverageCostPerUnit", (object?)inventory.AverageCostPerUnit ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalStockValue", (object?)inventory.TotalStockValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLowStock", inventory.IsLowStock);
            command.Parameters.AddWithValue("@IsOutOfStock", inventory.IsOutOfStock);
            command.Parameters.AddWithValue("@IsActive", inventory.IsActive);
            command.Parameters.AddWithValue("@LastStockInDate", (object?)inventory.LastStockInDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastStockOutDate", (object?)inventory.LastStockOutDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastCountDate", (object?)inventory.LastCountDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<bool> UpdateAsync(Inventory inventory)
        {
            const string query = @"
                UPDATE Inventory_Stock
                SET MaterialCode = @MaterialCode,
                    MaterialName = @MaterialName,
                    MaterialCategory = @MaterialCategory,
                    TotalQuantity = @TotalQuantity,
                    AvailableQuantity = @AvailableQuantity,
                    AllocatedQuantity = @AllocatedQuantity,
                    IssuedQuantity = @IssuedQuantity,
                    ReservedQuantity = @ReservedQuantity,
                    UOM = @UOM,
                    MinimumStock = @MinimumStock,
                    MaximumStock = @MaximumStock,
                    ReorderLevel = @ReorderLevel,
                    ReorderQuantity = @ReorderQuantity,
                    PrimaryStorageLocation = @PrimaryStorageLocation,
                    WarehouseCode = @WarehouseCode,
                    AverageCostPerUnit = @AverageCostPerUnit,
                    TotalStockValue = @TotalStockValue,
                    IsLowStock = @IsLowStock,
                    IsOutOfStock = @IsOutOfStock,
                    IsActive = @IsActive,
                    LastStockInDate = @LastStockInDate,
                    LastStockOutDate = @LastStockOutDate,
                    LastCountDate = @LastCountDate,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", inventory.Id);
            command.Parameters.AddWithValue("@MaterialCode", (object?)inventory.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)inventory.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCategory", (object?)inventory.MaterialCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalQuantity", inventory.TotalQuantity);
            command.Parameters.AddWithValue("@AvailableQuantity", inventory.AvailableQuantity);
            command.Parameters.AddWithValue("@AllocatedQuantity", inventory.AllocatedQuantity);
            command.Parameters.AddWithValue("@IssuedQuantity", inventory.IssuedQuantity);
            command.Parameters.AddWithValue("@ReservedQuantity", inventory.ReservedQuantity);
            command.Parameters.AddWithValue("@UOM", inventory.UOM);
            command.Parameters.AddWithValue("@MinimumStock", (object?)inventory.MinimumStock ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaximumStock", (object?)inventory.MaximumStock ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderLevel", (object?)inventory.ReorderLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderQuantity", (object?)inventory.ReorderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@PrimaryStorageLocation", (object?)inventory.PrimaryStorageLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@WarehouseCode", (object?)inventory.WarehouseCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@AverageCostPerUnit", (object?)inventory.AverageCostPerUnit ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalStockValue", (object?)inventory.TotalStockValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLowStock", inventory.IsLowStock);
            command.Parameters.AddWithValue("@IsOutOfStock", inventory.IsOutOfStock);
            command.Parameters.AddWithValue("@IsActive", inventory.IsActive);
            command.Parameters.AddWithValue("@LastStockInDate", (object?)inventory.LastStockInDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastStockOutDate", (object?)inventory.LastStockOutDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastCountDate", (object?)inventory.LastCountDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)inventory.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Inventory_Stock WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                  AND IsLowStock = 1
                ORDER BY AvailableQuantity ASC";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<IEnumerable<Inventory>> GetOutOfStockAsync()
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                  AND IsOutOfStock = 1
                ORDER BY MaterialCode";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<IEnumerable<Inventory>> GetByCategoryAsync(string category)
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                  AND MaterialCategory = @Category
                ORDER BY MaterialCode";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<IEnumerable<Inventory>> GetByLocationAsync(string location)
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                  AND PrimaryStorageLocation = @Location
                ORDER BY MaterialCode";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Location", location);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<IEnumerable<Inventory>> GetActiveAsync()
        {
            const string query = @"
                SELECT * FROM Inventory_Stock
                WHERE IsActive = 1
                  AND TotalQuantity > 0
                ORDER BY MaterialCode";

            var inventories = new List<Inventory>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                inventories.Add(MapToInventory(reader));
            }

            return inventories;
        }

        public async Task<bool> UpdateStockLevelsAsync(int materialId, decimal totalQty, decimal availableQty, decimal allocatedQty, decimal issuedQty)
        {
            const string query = @"
                UPDATE Inventory_Stock
                SET TotalQuantity = @TotalQuantity,
                    AvailableQuantity = @AvailableQuantity,
                    AllocatedQuantity = @AllocatedQuantity,
                    IssuedQuantity = @IssuedQuantity,
                    UpdatedAt = @UpdatedAt
                WHERE MaterialId = @MaterialId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);
            command.Parameters.AddWithValue("@TotalQuantity", totalQty);
            command.Parameters.AddWithValue("@AvailableQuantity", availableQty);
            command.Parameters.AddWithValue("@AllocatedQuantity", allocatedQty);
            command.Parameters.AddWithValue("@IssuedQuantity", issuedQty);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAverageCostAsync(int materialId, decimal avgCost, decimal totalValue)
        {
            const string query = @"
                UPDATE Inventory_Stock
                SET AverageCostPerUnit = @AverageCostPerUnit,
                    TotalStockValue = @TotalStockValue,
                    UpdatedAt = @UpdatedAt
                WHERE MaterialId = @MaterialId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);
            command.Parameters.AddWithValue("@AverageCostPerUnit", avgCost);
            command.Parameters.AddWithValue("@TotalStockValue", totalValue);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStockStatusAsync(int materialId, bool isLowStock, bool isOutOfStock)
        {
            const string query = @"
                UPDATE Inventory_Stock
                SET IsLowStock = @IsLowStock,
                    IsOutOfStock = @IsOutOfStock,
                    UpdatedAt = @UpdatedAt
                WHERE MaterialId = @MaterialId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);
            command.Parameters.AddWithValue("@IsLowStock", isLowStock);
            command.Parameters.AddWithValue("@IsOutOfStock", isOutOfStock);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // Transaction Methods
        public async Task<int> InsertTransactionAsync(InventoryTransaction transaction)
        {
            const string query = @"
                INSERT INTO Inventory_Transactions (
                    Id, MaterialId, TransactionType, TransactionNo, TransactionDate,
                    Quantity, UOM,
                    ReferenceType, ReferenceId, ReferenceNo,
                    FromLocation, ToLocation,
                    UnitCost, TotalCost, BalanceQuantity,
                    Remarks, PerformedBy,
                    JobCardId, RequisitionId,
                    SupplierId, GRNNo,
                    CreatedAt, CreatedBy
                )
                VALUES (
                    @Id, @MaterialId, @TransactionType, @TransactionNo, @TransactionDate,
                    @Quantity, @UOM,
                    @ReferenceType, @ReferenceId, @ReferenceNo,
                    @FromLocation, @ToLocation,
                    @UnitCost, @TotalCost, @BalanceQuantity,
                    @Remarks, @PerformedBy,
                    @JobCardId, @RequisitionId,
                    @SupplierId, @GRNNo,
                    @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = 0;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@MaterialId", transaction.MaterialId);
            command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
            command.Parameters.AddWithValue("@TransactionNo", transaction.TransactionNo);
            command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
            command.Parameters.AddWithValue("@Quantity", transaction.Quantity);
            command.Parameters.AddWithValue("@UOM", transaction.UOM);
            command.Parameters.AddWithValue("@ReferenceType", (object?)transaction.ReferenceType ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReferenceId", (object?)transaction.ReferenceId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReferenceNo", (object?)transaction.ReferenceNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromLocation", (object?)transaction.FromLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToLocation", (object?)transaction.ToLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnitCost", (object?)transaction.UnitCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalCost", (object?)transaction.TotalCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@BalanceQuantity", transaction.BalanceQuantity);
            command.Parameters.AddWithValue("@Remarks", (object?)transaction.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@PerformedBy", (object?)transaction.PerformedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@JobCardId", (object?)transaction.JobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequisitionId", (object?)transaction.RequisitionId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SupplierId", (object?)transaction.SupplierId ?? DBNull.Value);
            command.Parameters.AddWithValue("@GRNNo", (object?)transaction.GRNNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)transaction.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByMaterialIdAsync(int materialId)
        {
            const string query = @"
                SELECT * FROM Inventory_Transactions
                WHERE MaterialId = @MaterialId
                ORDER BY TransactionDate DESC, CreatedAt DESC";

            var transactions = new List<InventoryTransaction>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(MapToTransaction(reader));
            }

            return transactions;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByTypeAsync(string transactionType)
        {
            const string query = @"
                SELECT * FROM Inventory_Transactions
                WHERE TransactionType = @TransactionType
                ORDER BY TransactionDate DESC";

            var transactions = new List<InventoryTransaction>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TransactionType", transactionType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(MapToTransaction(reader));
            }

            return transactions;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT * FROM Inventory_Transactions
                WHERE TransactionDate >= @StartDate
                  AND TransactionDate <= @EndDate
                ORDER BY TransactionDate DESC";

            var transactions = new List<InventoryTransaction>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(MapToTransaction(reader));
            }

            return transactions;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByJobCardIdAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Inventory_Transactions
                WHERE JobCardId = @JobCardId
                ORDER BY TransactionDate DESC";

            var transactions = new List<InventoryTransaction>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(MapToTransaction(reader));
            }

            return transactions;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetRecentTransactionsAsync(int count)
        {
            const string query = @"
                SELECT TOP (@Count) * FROM Inventory_Transactions
                ORDER BY CreatedAt DESC";

            var transactions = new List<InventoryTransaction>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Count", count);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(MapToTransaction(reader));
            }

            return transactions;
        }

        public async Task<bool> ReconcileStockAsync(int materialId, decimal actualQuantity, string performedBy, string remarks)
        {
            // Get current stock
            var inventory = await GetByMaterialIdAsync(materialId);
            if (inventory == null)
                return false;

            var difference = actualQuantity - inventory.TotalQuantity;

            // Create adjustment transaction
            var transaction = new InventoryTransaction
            {
                MaterialId = materialId,
                TransactionType = "Adjustment",
                TransactionNo = $"ADJ-{DateTime.UtcNow:yyyyMMddHHmmss}",
                TransactionDate = DateTime.UtcNow,
                Quantity = difference,
                UOM = inventory.UOM,
                ReferenceType = "StockCount",
                BalanceQuantity = actualQuantity,
                Remarks = $"Stock reconciliation: {remarks}",
                PerformedBy = performedBy,
                CreatedBy = performedBy
            };

            await InsertTransactionAsync(transaction);

            // Update inventory
            const string query = @"
                UPDATE Inventory_Stock
                SET TotalQuantity = @TotalQuantity,
                    AvailableQuantity = AvailableQuantity + @Difference,
                    LastCountDate = @LastCountDate,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE MaterialId = @MaterialId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);
            command.Parameters.AddWithValue("@TotalQuantity", actualQuantity);
            command.Parameters.AddWithValue("@Difference", difference);
            command.Parameters.AddWithValue("@LastCountDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", performedBy);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // Mapping Methods
        private static Inventory MapToInventory(IDataReader reader)
        {
            return new Inventory
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MaterialId = reader.GetInt32(reader.GetOrdinal("MaterialId")),
                MaterialCode = reader.IsDBNull(reader.GetOrdinal("MaterialCode")) ? null : reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialName = reader.IsDBNull(reader.GetOrdinal("MaterialName")) ? null : reader.GetString(reader.GetOrdinal("MaterialName")),
                MaterialCategory = reader.IsDBNull(reader.GetOrdinal("MaterialCategory")) ? null : reader.GetString(reader.GetOrdinal("MaterialCategory")),
                TotalQuantity = reader.GetDecimal(reader.GetOrdinal("TotalQuantity")),
                AvailableQuantity = reader.GetDecimal(reader.GetOrdinal("AvailableQuantity")),
                AllocatedQuantity = reader.GetDecimal(reader.GetOrdinal("AllocatedQuantity")),
                IssuedQuantity = reader.GetDecimal(reader.GetOrdinal("IssuedQuantity")),
                ReservedQuantity = reader.GetDecimal(reader.GetOrdinal("ReservedQuantity")),
                UOM = reader.GetString(reader.GetOrdinal("UOM")),
                MinimumStock = reader.IsDBNull(reader.GetOrdinal("MinimumStock")) ? null : reader.GetDecimal(reader.GetOrdinal("MinimumStock")),
                MaximumStock = reader.IsDBNull(reader.GetOrdinal("MaximumStock")) ? null : reader.GetDecimal(reader.GetOrdinal("MaximumStock")),
                ReorderLevel = reader.IsDBNull(reader.GetOrdinal("ReorderLevel")) ? null : reader.GetDecimal(reader.GetOrdinal("ReorderLevel")),
                ReorderQuantity = reader.IsDBNull(reader.GetOrdinal("ReorderQuantity")) ? null : reader.GetDecimal(reader.GetOrdinal("ReorderQuantity")),
                PrimaryStorageLocation = reader.IsDBNull(reader.GetOrdinal("PrimaryStorageLocation")) ? null : reader.GetString(reader.GetOrdinal("PrimaryStorageLocation")),
                WarehouseCode = reader.IsDBNull(reader.GetOrdinal("WarehouseCode")) ? null : reader.GetString(reader.GetOrdinal("WarehouseCode")),
                AverageCostPerUnit = reader.IsDBNull(reader.GetOrdinal("AverageCostPerUnit")) ? null : reader.GetDecimal(reader.GetOrdinal("AverageCostPerUnit")),
                TotalStockValue = reader.IsDBNull(reader.GetOrdinal("TotalStockValue")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalStockValue")),
                IsLowStock = reader.GetBoolean(reader.GetOrdinal("IsLowStock")),
                IsOutOfStock = reader.GetBoolean(reader.GetOrdinal("IsOutOfStock")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                LastStockInDate = reader.IsDBNull(reader.GetOrdinal("LastStockInDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastStockInDate")),
                LastStockOutDate = reader.IsDBNull(reader.GetOrdinal("LastStockOutDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastStockOutDate")),
                LastCountDate = reader.IsDBNull(reader.GetOrdinal("LastCountDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastCountDate")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static InventoryTransaction MapToTransaction(IDataReader reader)
        {
            return new InventoryTransaction
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MaterialId = reader.GetInt32(reader.GetOrdinal("MaterialId")),
                TransactionType = reader.GetString(reader.GetOrdinal("TransactionType")),
                TransactionNo = reader.GetString(reader.GetOrdinal("TransactionNo")),
                TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                UOM = reader.GetString(reader.GetOrdinal("UOM")),
                ReferenceType = reader.IsDBNull(reader.GetOrdinal("ReferenceType")) ? null : reader.GetString(reader.GetOrdinal("ReferenceType")),
                ReferenceId = reader.IsDBNull(reader.GetOrdinal("ReferenceId")) ? null : reader.GetInt32(reader.GetOrdinal("ReferenceId")),
                ReferenceNo = reader.IsDBNull(reader.GetOrdinal("ReferenceNo")) ? null : reader.GetString(reader.GetOrdinal("ReferenceNo")),
                FromLocation = reader.IsDBNull(reader.GetOrdinal("FromLocation")) ? null : reader.GetString(reader.GetOrdinal("FromLocation")),
                ToLocation = reader.IsDBNull(reader.GetOrdinal("ToLocation")) ? null : reader.GetString(reader.GetOrdinal("ToLocation")),
                UnitCost = reader.IsDBNull(reader.GetOrdinal("UnitCost")) ? null : reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                TotalCost = reader.IsDBNull(reader.GetOrdinal("TotalCost")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                BalanceQuantity = reader.GetDecimal(reader.GetOrdinal("BalanceQuantity")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                PerformedBy = reader.IsDBNull(reader.GetOrdinal("PerformedBy")) ? null : reader.GetString(reader.GetOrdinal("PerformedBy")),
                JobCardId = reader.IsDBNull(reader.GetOrdinal("JobCardId")) ? null : reader.GetInt32(reader.GetOrdinal("JobCardId")),
                RequisitionId = reader.IsDBNull(reader.GetOrdinal("RequisitionId")) ? null : reader.GetInt32(reader.GetOrdinal("RequisitionId")),
                SupplierId = reader.IsDBNull(reader.GetOrdinal("SupplierId")) ? null : reader.GetInt32(reader.GetOrdinal("SupplierId")),
                GRNNo = reader.IsDBNull(reader.GetOrdinal("GRNNo")) ? null : reader.GetString(reader.GetOrdinal("GRNNo")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}
