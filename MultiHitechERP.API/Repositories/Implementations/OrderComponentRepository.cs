using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OrderComponentRepository : IOrderComponentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderComponentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static OrderComponent Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            OrderId = r.GetInt32(r.GetOrdinal("OrderId")),
            OrderItemId = r.IsDBNull(r.GetOrdinal("OrderItemId")) ? null : r.GetInt32(r.GetOrdinal("OrderItemId")),
            OrderNo = r.IsDBNull(r.GetOrdinal("OrderNo")) ? null : r.GetString(r.GetOrdinal("OrderNo")),
            ComponentId = r.GetInt32(r.GetOrdinal("ComponentId")),
            ComponentName = r.IsDBNull(r.GetOrdinal("ComponentName")) ? null : r.GetString(r.GetOrdinal("ComponentName")),
            PartNumber = r.IsDBNull(r.GetOrdinal("PartNumber")) ? null : r.GetString(r.GetOrdinal("PartNumber")),
            UOM = r.IsDBNull(r.GetOrdinal("UOM")) ? null : r.GetString(r.GetOrdinal("UOM")),
            ReservedQty = r.GetDecimal(r.GetOrdinal("ReservedQty")),
            ConsumedQty = r.GetDecimal(r.GetOrdinal("ConsumedQty")),
            Status = r.GetString(r.GetOrdinal("Status")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt")),
            UpdatedAt = r.GetDateTime(r.GetOrdinal("UpdatedAt")),
            UpdatedBy = r.IsDBNull(r.GetOrdinal("UpdatedBy")) ? null : r.GetString(r.GetOrdinal("UpdatedBy")),
        };

        private async Task<List<OrderComponent>> QueryAsync(string sql, Action<SqlCommand> bind)
        {
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            bind(cmd);
            using var r = await cmd.ExecuteReaderAsync();
            var list = new List<OrderComponent>();
            while (await r.ReadAsync()) list.Add(Map((SqlDataReader)r));
            return list;
        }

        public async Task<IEnumerable<OrderComponent>> GetByOrderAsync(int orderId) =>
            await QueryAsync("SELECT * FROM Stores_OrderComponents WHERE OrderId = @OrderId ORDER BY ComponentName",
                c => c.Parameters.AddWithValue("@OrderId", orderId));

        public async Task<IEnumerable<OrderComponent>> GetReservedForOrderItemAsync(int orderId, int? orderItemId) =>
            await QueryAsync(
                "SELECT * FROM Stores_OrderComponents WHERE OrderId = @OrderId AND ISNULL(OrderItemId,0) = @OrderItemId AND ConsumedQty = 0",
                c => { c.Parameters.AddWithValue("@OrderId", orderId); c.Parameters.AddWithValue("@OrderItemId", orderItemId ?? 0); });

        public async Task DeleteReservedForOrderItemAsync(int orderId, int? orderItemId)
        {
            const string sql = "DELETE FROM Stores_OrderComponents WHERE OrderId = @OrderId AND ISNULL(OrderItemId,0) = @OrderItemId AND ConsumedQty = 0";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@OrderItemId", orderItemId ?? 0);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> InsertAsync(OrderComponent oc)
        {
            const string sql = @"
                INSERT INTO Stores_OrderComponents
                    (OrderId, OrderItemId, OrderNo, ComponentId, ComponentName, PartNumber, UOM, ReservedQty, ConsumedQty, Status, CreatedAt, UpdatedAt, UpdatedBy)
                VALUES
                    (@OrderId, @OrderItemId, @OrderNo, @ComponentId, @ComponentName, @PartNumber, @UOM, @ReservedQty, @ConsumedQty, @Status, GETUTCDATE(), GETUTCDATE(), @By);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@OrderId", oc.OrderId);
            cmd.Parameters.AddWithValue("@OrderItemId", (object?)oc.OrderItemId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderNo", (object?)oc.OrderNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ComponentId", oc.ComponentId);
            cmd.Parameters.AddWithValue("@ComponentName", (object?)oc.ComponentName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PartNumber", (object?)oc.PartNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UOM", (object?)oc.UOM ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ReservedQty", oc.ReservedQty);
            cmd.Parameters.AddWithValue("@ConsumedQty", oc.ConsumedQty);
            cmd.Parameters.AddWithValue("@Status", oc.Status);
            cmd.Parameters.AddWithValue("@By", (object?)oc.UpdatedBy ?? DBNull.Value);
            return (int)(await cmd.ExecuteScalarAsync() ?? 0);
        }

        public async Task<OrderComponent?> GetByOrderComponentAsync(int orderId, int componentId)
        {
            var rows = await QueryAsync(
                "SELECT TOP 1 * FROM Stores_OrderComponents WHERE OrderId = @OrderId AND ComponentId = @ComponentId ORDER BY Id DESC",
                c => { c.Parameters.AddWithValue("@OrderId", orderId); c.Parameters.AddWithValue("@ComponentId", componentId); });
            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task AddConsumedAsync(int id, decimal addConsumed, decimal releaseReserved, string status, string? by)
        {
            const string sql = @"
                UPDATE Stores_OrderComponents
                   SET ConsumedQty = ConsumedQty + @AddConsumed,
                       ReservedQty = CASE WHEN ReservedQty - @ReleaseReserved < 0 THEN 0 ELSE ReservedQty - @ReleaseReserved END,
                       Status = @Status,
                       UpdatedAt = GETUTCDATE(),
                       UpdatedBy = @By
                 WHERE Id = @Id";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@AddConsumed", addConsumed);
            cmd.Parameters.AddWithValue("@ReleaseReserved", releaseReserved);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@By", (object?)by ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
