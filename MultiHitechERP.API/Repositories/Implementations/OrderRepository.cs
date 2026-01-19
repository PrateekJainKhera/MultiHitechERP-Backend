using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// ADO.NET implementation of Order repository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToOrder(reader);
            }

            return null;
        }

        public async Task<Order?> GetByOrderNoAsync(string orderNo)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE OrderNo = @OrderNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderNo", orderNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToOrder(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE CustomerId = @CustomerId
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CustomerId", customerId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByProductIdAsync(Guid productId)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE ProductId = @ProductId
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status = @Status
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<Guid> InsertAsync(Order order)
        {
            const string query = @"
                INSERT INTO Orders (
                    Id, OrderNo, OrderDate, DueDate, AdjustedDueDate,
                    CustomerId, ProductId, Quantity, OriginalQuantity,
                    QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    Status, Priority, PlanningStatus,
                    DrawingReviewStatus, DrawingReviewedBy, DrawingReviewedAt, DrawingReviewNotes,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    DelayReason, RescheduleCount,
                    MaterialGradeApproved, MaterialGradeApprovalDate, MaterialGradeApprovedBy,
                    OrderValue, AdvancePayment, BalancePayment,
                    CreatedAt, CreatedBy, Version
                ) VALUES (
                    @Id, @OrderNo, @OrderDate, @DueDate, @AdjustedDueDate,
                    @CustomerId, @ProductId, @Quantity, @OriginalQuantity,
                    @QtyCompleted, @QtyRejected, @QtyInProgress, @QtyScrap,
                    @Status, @Priority, @PlanningStatus,
                    @DrawingReviewStatus, @DrawingReviewedBy, @DrawingReviewedAt, @DrawingReviewNotes,
                    @CurrentProcess, @CurrentMachine, @CurrentOperator,
                    @ProductionStartDate, @ProductionEndDate,
                    @DelayReason, @RescheduleCount,
                    @MaterialGradeApproved, @MaterialGradeApprovalDate, @MaterialGradeApprovedBy,
                    @OrderValue, @AdvancePayment, @BalancePayment,
                    @CreatedAt, @CreatedBy, @Version
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            order.Id = Guid.NewGuid();
            order.CreatedAt = DateTime.UtcNow;

            AddOrderParameters(command, order);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return order.Id;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            const string query = @"
                UPDATE Orders SET
                    OrderDate = @OrderDate,
                    DueDate = @DueDate,
                    AdjustedDueDate = @AdjustedDueDate,
                    CustomerId = @CustomerId,
                    ProductId = @ProductId,
                    Quantity = @Quantity,
                    OriginalQuantity = @OriginalQuantity,
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    QtyScrap = @QtyScrap,
                    Status = @Status,
                    Priority = @Priority,
                    PlanningStatus = @PlanningStatus,
                    DrawingReviewStatus = @DrawingReviewStatus,
                    DrawingReviewedBy = @DrawingReviewedBy,
                    DrawingReviewedAt = @DrawingReviewedAt,
                    DrawingReviewNotes = @DrawingReviewNotes,
                    CurrentProcess = @CurrentProcess,
                    CurrentMachine = @CurrentMachine,
                    CurrentOperator = @CurrentOperator,
                    ProductionStartDate = @ProductionStartDate,
                    ProductionEndDate = @ProductionEndDate,
                    DelayReason = @DelayReason,
                    RescheduleCount = @RescheduleCount,
                    MaterialGradeApproved = @MaterialGradeApproved,
                    MaterialGradeApprovalDate = @MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy = @MaterialGradeApprovedBy,
                    OrderValue = @OrderValue,
                    AdvancePayment = @AdvancePayment,
                    BalancePayment = @BalancePayment,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @Version
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            order.UpdatedAt = DateTime.UtcNow;

            AddOrderParameters(command, order);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Orders WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateDrawingReviewStatusAsync(Guid id, string status, string reviewedBy, string? notes)
        {
            const string query = @"
                UPDATE Orders SET
                    DrawingReviewStatus = @Status,
                    DrawingReviewedBy = @ReviewedBy,
                    DrawingReviewedAt = @ReviewedAt,
                    DrawingReviewNotes = @Notes,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@ReviewedBy", reviewedBy);
            command.Parameters.AddWithValue("@ReviewedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePlanningStatusAsync(Guid id, string status)
        {
            const string query = @"
                UPDATE Orders SET
                    PlanningStatus = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateQuantitiesAsync(Guid id, int qtyCompleted, int qtyRejected, int qtyInProgress)
        {
            const string query = @"
                UPDATE Orders SET
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@QtyCompleted", qtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", qtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", qtyInProgress);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            const string query = @"
                UPDATE Orders SET
                    Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Order>> GetPendingDrawingReviewAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE DrawingReviewStatus = 'Pending'
                ORDER BY OrderDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetReadyForPlanningAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE DrawingReviewStatus = 'Approved'
                  AND PlanningStatus = 'Not Planned'
                ORDER BY Priority DESC, DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetInProgressOrdersAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status = 'In Progress'
                ORDER BY DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetDelayedOrdersAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status != 'Completed'
                  AND Status != 'Cancelled'
                  AND DueDate < GETDATE()
                ORDER BY DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<bool> UpdateWithVersionCheckAsync(Order order)
        {
            const string query = @"
                UPDATE Orders SET
                    OrderDate = @OrderDate,
                    DueDate = @DueDate,
                    Status = @Status,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @NewVersion
                WHERE Id = @Id AND Version = @CurrentVersion";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var newVersion = order.Version + 1;

            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@DueDate", order.DueDate);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)order.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentVersion", order.Version);
            command.Parameters.AddWithValue("@NewVersion", newVersion);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                order.Version = newVersion;
                return true;
            }

            return false;
        }

        public async Task<int> GetVersionAsync(Guid id)
        {
            const string query = "SELECT Version FROM Orders WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Helper Methods

        private static Order MapToOrder(SqlDataReader reader)
        {
            return new Order
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                OrderNo = reader.GetString(reader.GetOrdinal("OrderNo")),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                AdjustedDueDate = reader.IsDBNull(reader.GetOrdinal("AdjustedDueDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("AdjustedDueDate")),

                CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId")),
                ProductId = reader.GetGuid(reader.GetOrdinal("ProductId")),

                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                OriginalQuantity = reader.GetInt32(reader.GetOrdinal("OriginalQuantity")),
                QtyCompleted = reader.GetInt32(reader.GetOrdinal("QtyCompleted")),
                QtyRejected = reader.GetInt32(reader.GetOrdinal("QtyRejected")),
                QtyInProgress = reader.GetInt32(reader.GetOrdinal("QtyInProgress")),
                QtyScrap = reader.GetInt32(reader.GetOrdinal("QtyScrap")),

                Status = reader.GetString(reader.GetOrdinal("Status")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                PlanningStatus = reader.GetString(reader.GetOrdinal("PlanningStatus")),

                DrawingReviewStatus = reader.GetString(reader.GetOrdinal("DrawingReviewStatus")),
                DrawingReviewedBy = reader.IsDBNull(reader.GetOrdinal("DrawingReviewedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("DrawingReviewedBy")),
                DrawingReviewedAt = reader.IsDBNull(reader.GetOrdinal("DrawingReviewedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("DrawingReviewedAt")),
                DrawingReviewNotes = reader.IsDBNull(reader.GetOrdinal("DrawingReviewNotes"))
                    ? null : reader.GetString(reader.GetOrdinal("DrawingReviewNotes")),

                CurrentProcess = reader.IsDBNull(reader.GetOrdinal("CurrentProcess"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentProcess")),
                CurrentMachine = reader.IsDBNull(reader.GetOrdinal("CurrentMachine"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentMachine")),
                CurrentOperator = reader.IsDBNull(reader.GetOrdinal("CurrentOperator"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentOperator")),

                ProductionStartDate = reader.IsDBNull(reader.GetOrdinal("ProductionStartDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ProductionStartDate")),
                ProductionEndDate = reader.IsDBNull(reader.GetOrdinal("ProductionEndDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ProductionEndDate")),

                DelayReason = reader.IsDBNull(reader.GetOrdinal("DelayReason"))
                    ? null : reader.GetString(reader.GetOrdinal("DelayReason")),
                RescheduleCount = reader.GetInt32(reader.GetOrdinal("RescheduleCount")),

                MaterialGradeApproved = reader.GetBoolean(reader.GetOrdinal("MaterialGradeApproved")),
                MaterialGradeApprovalDate = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovalDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("MaterialGradeApprovalDate")),
                MaterialGradeApprovedBy = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("MaterialGradeApprovedBy")),

                OrderValue = reader.IsDBNull(reader.GetOrdinal("OrderValue"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("OrderValue")),
                AdvancePayment = reader.IsDBNull(reader.GetOrdinal("AdvancePayment"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("AdvancePayment")),
                BalancePayment = reader.IsDBNull(reader.GetOrdinal("BalancePayment"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("BalancePayment")),

                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                Version = reader.GetInt32(reader.GetOrdinal("Version"))
            };
        }

        private static void AddOrderParameters(SqlCommand command, Order order)
        {
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@OrderNo", order.OrderNo);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@DueDate", order.DueDate);
            command.Parameters.AddWithValue("@AdjustedDueDate", (object?)order.AdjustedDueDate ?? DBNull.Value);

            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@ProductId", order.ProductId);

            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@OriginalQuantity", order.OriginalQuantity);
            command.Parameters.AddWithValue("@QtyCompleted", order.QtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", order.QtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", order.QtyInProgress);
            command.Parameters.AddWithValue("@QtyScrap", order.QtyScrap);

            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@Priority", order.Priority);
            command.Parameters.AddWithValue("@PlanningStatus", order.PlanningStatus);

            command.Parameters.AddWithValue("@DrawingReviewStatus", order.DrawingReviewStatus);
            command.Parameters.AddWithValue("@DrawingReviewedBy", (object?)order.DrawingReviewedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingReviewedAt", (object?)order.DrawingReviewedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingReviewNotes", (object?)order.DrawingReviewNotes ?? DBNull.Value);

            command.Parameters.AddWithValue("@CurrentProcess", (object?)order.CurrentProcess ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentMachine", (object?)order.CurrentMachine ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentOperator", (object?)order.CurrentOperator ?? DBNull.Value);

            command.Parameters.AddWithValue("@ProductionStartDate", (object?)order.ProductionStartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductionEndDate", (object?)order.ProductionEndDate ?? DBNull.Value);

            command.Parameters.AddWithValue("@DelayReason", (object?)order.DelayReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@RescheduleCount", order.RescheduleCount);

            command.Parameters.AddWithValue("@MaterialGradeApproved", order.MaterialGradeApproved);
            command.Parameters.AddWithValue("@MaterialGradeApprovalDate", (object?)order.MaterialGradeApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeApprovedBy", (object?)order.MaterialGradeApprovedBy ?? DBNull.Value);

            command.Parameters.AddWithValue("@OrderValue", (object?)order.OrderValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@AdvancePayment", (object?)order.AdvancePayment ?? DBNull.Value);
            command.Parameters.AddWithValue("@BalancePayment", (object?)order.BalancePayment ?? DBNull.Value);

            command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)order.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)order.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)order.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Version", order.Version);
        }
    }
}
