using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly string _connectionString;

        public OrderItemRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            var items = new List<OrderItem>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT
                    Id, OrderId, ItemSequence, ProductId, ProductName,
                    Quantity, QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    DueDate, Priority, Status, PlanningStatus,
                    PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    MaterialGradeApproved, MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy, MaterialGradeRemark,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Orders_OrderItems
                ORDER BY OrderId, ItemSequence", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(MapToOrderItem(reader));
            }

            return items;
        }

        public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            var items = new List<OrderItem>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT
                    Id, OrderId, ItemSequence, ProductId, ProductName,
                    Quantity, QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    DueDate, Priority, Status, PlanningStatus,
                    PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    MaterialGradeApproved, MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy, MaterialGradeRemark,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Orders_OrderItems
                WHERE OrderId = @OrderId
                ORDER BY ItemSequence", connection);

            command.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(MapToOrderItem(reader));
            }

            return items;
        }

        public async Task<OrderItem?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT
                    Id, OrderId, ItemSequence, ProductId, ProductName,
                    Quantity, QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    DueDate, Priority, Status, PlanningStatus,
                    PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    MaterialGradeApproved, MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy, MaterialGradeRemark,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Orders_OrderItems
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToOrderItem(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(OrderItem orderItem)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO Orders_OrderItems (
                    OrderId, ItemSequence, ProductId, ProductName,
                    Quantity, QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    DueDate, Priority, Status, PlanningStatus,
                    PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    MaterialGradeApproved, MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy, MaterialGradeRemark,
                    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                ) VALUES (
                    @OrderId, @ItemSequence, @ProductId, @ProductName,
                    @Quantity, @QtyCompleted, @QtyRejected, @QtyInProgress, @QtyScrap,
                    @DueDate, @Priority, @Status, @PlanningStatus,
                    @PrimaryDrawingId, @DrawingSource, @LinkedProductTemplateId,
                    @CurrentProcess, @CurrentMachine, @CurrentOperator,
                    @ProductionStartDate, @ProductionEndDate,
                    @MaterialGradeApproved, @MaterialGradeApprovalDate,
                    @MaterialGradeApprovedBy, @MaterialGradeRemark,
                    @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

            AddOrderItemParameters(command, orderItem);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<List<int>> CreateBatchAsync(List<OrderItem> orderItems)
        {
            var createdIds = new List<int>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var item in orderItems)
                {
                    var command = new SqlCommand(@"
                        INSERT INTO Orders_OrderItems (
                            OrderId, ItemSequence, ProductId, ProductName,
                            Quantity, QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                            DueDate, Priority, Status, PlanningStatus,
                            PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                            CurrentProcess, CurrentMachine, CurrentOperator,
                            ProductionStartDate, ProductionEndDate,
                            MaterialGradeApproved, MaterialGradeApprovalDate,
                            MaterialGradeApprovedBy, MaterialGradeRemark,
                            CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                        ) VALUES (
                            @OrderId, @ItemSequence, @ProductId, @ProductName,
                            @Quantity, @QtyCompleted, @QtyRejected, @QtyInProgress, @QtyScrap,
                            @DueDate, @Priority, @Status, @PlanningStatus,
                            @PrimaryDrawingId, @DrawingSource, @LinkedProductTemplateId,
                            @CurrentProcess, @CurrentMachine, @CurrentOperator,
                            @ProductionStartDate, @ProductionEndDate,
                            @MaterialGradeApproved, @MaterialGradeApprovalDate,
                            @MaterialGradeApprovedBy, @MaterialGradeRemark,
                            @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", connection, transaction);

                    AddOrderItemParameters(command, item);

                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        createdIds.Add(Convert.ToInt32(result));
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return createdIds;
        }

        public async Task<bool> UpdateAsync(OrderItem orderItem)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Orders_OrderItems SET
                    Quantity = @Quantity,
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    QtyScrap = @QtyScrap,
                    DueDate = @DueDate,
                    Priority = @Priority,
                    Status = @Status,
                    PlanningStatus = @PlanningStatus,
                    PrimaryDrawingId = @PrimaryDrawingId,
                    DrawingSource = @DrawingSource,
                    LinkedProductTemplateId = @LinkedProductTemplateId,
                    CurrentProcess = @CurrentProcess,
                    CurrentMachine = @CurrentMachine,
                    CurrentOperator = @CurrentOperator,
                    ProductionStartDate = @ProductionStartDate,
                    ProductionEndDate = @ProductionEndDate,
                    MaterialGradeApproved = @MaterialGradeApproved,
                    MaterialGradeApprovalDate = @MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy = @MaterialGradeApprovedBy,
                    MaterialGradeRemark = @MaterialGradeRemark,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", orderItem.Id);
            AddOrderItemParameters(command, orderItem);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Orders_OrderItems WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<string> GetNextItemSequenceAsync(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT MAX(ItemSequence)
                FROM Orders_OrderItems
                WHERE OrderId = @OrderId", connection);

            command.Parameters.AddWithValue("@OrderId", orderId);

            var result = await command.ExecuteScalarAsync();

            if (result == null || result == DBNull.Value)
            {
                return "A"; // First item
            }

            var lastSequence = result.ToString();
            if (string.IsNullOrEmpty(lastSequence))
            {
                return "A";
            }

            // Increment the letter (A -> B -> C, etc.)
            var lastChar = lastSequence[0];
            var nextChar = (char)(lastChar + 1);
            return nextChar.ToString();
        }

        public async Task<bool> ProductExistsInOrderAsync(int orderId, int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Orders_OrderItems
                WHERE OrderId = @OrderId AND ProductId = @ProductId", connection);

            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ProductId", productId);

            var count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        public async Task<bool> UpdateQuantitiesAsync(int itemId, int qtyCompleted, int qtyRejected, int qtyInProgress, int qtyScrap)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Orders_OrderItems SET
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    QtyScrap = @QtyScrap,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", itemId);
            command.Parameters.AddWithValue("@QtyCompleted", qtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", qtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", qtyInProgress);
            command.Parameters.AddWithValue("@QtyScrap", qtyScrap);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(int itemId, string status)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Orders_OrderItems SET
                    Status = @Status,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", itemId);
            command.Parameters.AddWithValue("@Status", status);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Helper methods
        private OrderItem MapToOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                ItemSequence = reader.GetString(reader.GetOrdinal("ItemSequence")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                QtyCompleted = reader.GetInt32(reader.GetOrdinal("QtyCompleted")),
                QtyRejected = reader.GetInt32(reader.GetOrdinal("QtyRejected")),
                QtyInProgress = reader.GetInt32(reader.GetOrdinal("QtyInProgress")),
                QtyScrap = reader.GetInt32(reader.GetOrdinal("QtyScrap")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                PlanningStatus = reader.GetString(reader.GetOrdinal("PlanningStatus")),
                PrimaryDrawingId = reader.IsDBNull(reader.GetOrdinal("PrimaryDrawingId")) ? null : reader.GetInt32(reader.GetOrdinal("PrimaryDrawingId")),
                DrawingSource = reader.IsDBNull(reader.GetOrdinal("DrawingSource")) ? null : reader.GetString(reader.GetOrdinal("DrawingSource")),
                LinkedProductTemplateId = reader.IsDBNull(reader.GetOrdinal("LinkedProductTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("LinkedProductTemplateId")),
                CurrentProcess = reader.IsDBNull(reader.GetOrdinal("CurrentProcess")) ? null : reader.GetString(reader.GetOrdinal("CurrentProcess")),
                CurrentMachine = reader.IsDBNull(reader.GetOrdinal("CurrentMachine")) ? null : reader.GetString(reader.GetOrdinal("CurrentMachine")),
                CurrentOperator = reader.IsDBNull(reader.GetOrdinal("CurrentOperator")) ? null : reader.GetString(reader.GetOrdinal("CurrentOperator")),
                ProductionStartDate = reader.IsDBNull(reader.GetOrdinal("ProductionStartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ProductionStartDate")),
                ProductionEndDate = reader.IsDBNull(reader.GetOrdinal("ProductionEndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ProductionEndDate")),
                MaterialGradeApproved = reader.GetBoolean(reader.GetOrdinal("MaterialGradeApproved")),
                MaterialGradeApprovalDate = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovalDate")) ? null : reader.GetDateTime(reader.GetOrdinal("MaterialGradeApprovalDate")),
                MaterialGradeApprovedBy = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("MaterialGradeApprovedBy")),
                MaterialGradeRemark = reader.IsDBNull(reader.GetOrdinal("MaterialGradeRemark")) ? null : reader.GetString(reader.GetOrdinal("MaterialGradeRemark")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private void AddOrderItemParameters(SqlCommand command, OrderItem item)
        {
            command.Parameters.AddWithValue("@OrderId", item.OrderId);
            command.Parameters.AddWithValue("@ItemSequence", item.ItemSequence);
            command.Parameters.AddWithValue("@ProductId", item.ProductId);
            command.Parameters.AddWithValue("@ProductName", (object?)item.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@Quantity", item.Quantity);
            command.Parameters.AddWithValue("@QtyCompleted", item.QtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", item.QtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", item.QtyInProgress);
            command.Parameters.AddWithValue("@QtyScrap", item.QtyScrap);
            command.Parameters.AddWithValue("@DueDate", item.DueDate);
            command.Parameters.AddWithValue("@Priority", item.Priority);
            command.Parameters.AddWithValue("@Status", item.Status);
            command.Parameters.AddWithValue("@PlanningStatus", item.PlanningStatus);
            command.Parameters.AddWithValue("@PrimaryDrawingId", (object?)item.PrimaryDrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingSource", (object?)item.DrawingSource ?? DBNull.Value);
            command.Parameters.AddWithValue("@LinkedProductTemplateId", (object?)item.LinkedProductTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentProcess", (object?)item.CurrentProcess ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentMachine", (object?)item.CurrentMachine ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentOperator", (object?)item.CurrentOperator ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductionStartDate", (object?)item.ProductionStartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductionEndDate", (object?)item.ProductionEndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeApproved", item.MaterialGradeApproved);
            command.Parameters.AddWithValue("@MaterialGradeApprovalDate", (object?)item.MaterialGradeApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeApprovedBy", (object?)item.MaterialGradeApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeRemark", (object?)item.MaterialGradeRemark ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)item.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)item.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)item.UpdatedBy ?? DBNull.Value);
        }
    }
}
