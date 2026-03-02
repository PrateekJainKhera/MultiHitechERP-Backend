using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OrderItemQCRepository : IOrderItemQCRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderItemQCRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Returns order items where:
        ///   - At least one Assembly job card (ChildPartName LIKE '%Assembly%') is Completed
        ///   - No QC record with QCStatus='Passed' exists
        /// Includes the latest QC record (if any) so the UI can show Failed status.
        /// </summary>
        public async Task<IEnumerable<QCPendingItemResponse>> GetPendingItemsAsync()
        {
            const string query = @"
                SELECT
                    oi.Id           AS OrderItemId,
                    oi.OrderId,
                    o.OrderNo,
                    ISNULL(oi.ItemSequence, 'A') AS ItemSequence,
                    oi.ProductName,
                    ISNULL(c.CustomerName, '') AS CustomerName,
                    oi.Quantity,
                    qc.Id           AS QCRecordId,
                    qc.QCStatus,
                    qc.CertificatePath,
                    qc.QCCompletedAt,
                    qc.QCCompletedBy,
                    qc.Notes
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
                -- Must have at least one completed Assembly job card
                INNER JOIN (
                    SELECT DISTINCT OrderItemId
                    FROM Planning_JobCards
                    WHERE ChildPartName LIKE '%Assembly%'
                      AND ProductionStatus = 'Completed'
                      AND OrderItemId IS NOT NULL
                ) asm ON oi.Id = asm.OrderItemId
                -- Latest QC record for this order item (if any)
                LEFT JOIN (
                    SELECT *,
                           ROW_NUMBER() OVER (PARTITION BY OrderItemId ORDER BY CreatedAt DESC) AS rn
                    FROM Production_OrderItemQC
                ) qc ON oi.Id = qc.OrderItemId AND qc.rn = 1
                -- Only items without a Passed QC
                WHERE (qc.QCStatus IS NULL OR qc.QCStatus <> 'Passed')
                ORDER BY o.OrderNo, oi.ItemSequence";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCPendingItemResponse>();
            while (await reader.ReadAsync())
            {
                results.Add(new QCPendingItemResponse
                {
                    OrderItemId    = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                    OrderId        = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    OrderNo        = reader.GetString(reader.GetOrdinal("OrderNo")),
                    ItemSequence   = reader.GetString(reader.GetOrdinal("ItemSequence")),
                    ProductName    = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? "" : reader.GetString(reader.GetOrdinal("ProductName")),
                    CustomerName   = reader.GetString(reader.GetOrdinal("CustomerName")),
                    Quantity       = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    QCRecordId     = reader.IsDBNull(reader.GetOrdinal("QCRecordId")) ? null : reader.GetInt32(reader.GetOrdinal("QCRecordId")),
                    QCStatus       = reader.IsDBNull(reader.GetOrdinal("QCStatus")) ? null : reader.GetString(reader.GetOrdinal("QCStatus")),
                    CertificatePath = reader.IsDBNull(reader.GetOrdinal("CertificatePath")) ? null : reader.GetString(reader.GetOrdinal("CertificatePath")),
                    QCCompletedAt  = reader.IsDBNull(reader.GetOrdinal("QCCompletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("QCCompletedAt")),
                    QCCompletedBy  = reader.IsDBNull(reader.GetOrdinal("QCCompletedBy")) ? null : reader.GetString(reader.GetOrdinal("QCCompletedBy")),
                    Notes          = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                });
            }
            return results;
        }

        public async Task<OrderItemQC?> GetLatestByOrderItemAsync(int orderItemId)
        {
            const string query = @"
                SELECT TOP 1 * FROM Production_OrderItemQC
                WHERE OrderItemId = @OrderItemId
                ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderItemId", orderItemId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return MapToModel(reader);
            return null;
        }

        public async Task<bool> HasPassedQCAsync(int orderItemId)
        {
            const string query = @"
                SELECT COUNT(1) FROM Production_OrderItemQC
                WHERE OrderItemId = @OrderItemId AND QCStatus = 'Passed'";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderItemId", orderItemId);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<int> InsertAsync(OrderItemQC record)
        {
            const string query = @"
                INSERT INTO Production_OrderItemQC
                    (OrderItemId, OrderId, QCStatus, CertificatePath, QCCompletedAt, QCCompletedBy, Notes, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES
                    (@OrderItemId, @OrderId, @QCStatus, @CertificatePath, @QCCompletedAt, @QCCompletedBy, @Notes, GETUTCDATE())";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderItemId",     record.OrderItemId);
            command.Parameters.AddWithValue("@OrderId",         record.OrderId);
            command.Parameters.AddWithValue("@QCStatus",        record.QCStatus);
            command.Parameters.AddWithValue("@CertificatePath", (object?)record.CertificatePath ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCCompletedAt",   (object?)record.QCCompletedAt   ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCCompletedBy",   (object?)record.QCCompletedBy   ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes",           (object?)record.Notes           ?? DBNull.Value);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(OrderItemQC record)
        {
            const string query = @"
                UPDATE Production_OrderItemQC
                SET QCStatus        = @QCStatus,
                    CertificatePath = @CertificatePath,
                    QCCompletedAt   = @QCCompletedAt,
                    QCCompletedBy   = @QCCompletedBy,
                    Notes           = @Notes,
                    UpdatedAt       = GETUTCDATE()
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id",              record.Id);
            command.Parameters.AddWithValue("@QCStatus",        record.QCStatus);
            command.Parameters.AddWithValue("@CertificatePath", (object?)record.CertificatePath ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCCompletedAt",   (object?)record.QCCompletedAt   ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCCompletedBy",   (object?)record.QCCompletedBy   ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes",           (object?)record.Notes           ?? DBNull.Value);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static OrderItemQC MapToModel(IDataReader r) => new OrderItemQC
        {
            Id              = r.GetInt32(r.GetOrdinal("Id")),
            OrderItemId     = r.GetInt32(r.GetOrdinal("OrderItemId")),
            OrderId         = r.GetInt32(r.GetOrdinal("OrderId")),
            QCStatus        = r.GetString(r.GetOrdinal("QCStatus")),
            CertificatePath = r.IsDBNull(r.GetOrdinal("CertificatePath")) ? null : r.GetString(r.GetOrdinal("CertificatePath")),
            QCCompletedAt   = r.IsDBNull(r.GetOrdinal("QCCompletedAt")) ? null : r.GetDateTime(r.GetOrdinal("QCCompletedAt")),
            QCCompletedBy   = r.IsDBNull(r.GetOrdinal("QCCompletedBy")) ? null : r.GetString(r.GetOrdinal("QCCompletedBy")),
            Notes           = r.IsDBNull(r.GetOrdinal("Notes")) ? null : r.GetString(r.GetOrdinal("Notes")),
            CreatedAt       = r.GetDateTime(r.GetOrdinal("CreatedAt")),
            UpdatedAt       = r.IsDBNull(r.GetOrdinal("UpdatedAt")) ? null : r.GetDateTime(r.GetOrdinal("UpdatedAt")),
        };
    }
}
