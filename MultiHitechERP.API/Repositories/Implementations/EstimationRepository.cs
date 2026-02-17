using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Sales;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class EstimationRepository : IEstimationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EstimationRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Estimation>> GetAllAsync()
        {
            const string query = @"
                SELECT e.*, c.CustomerName AS CustomerNameJoin
                FROM Sales_Estimations e
                LEFT JOIN Masters_Customers c ON e.CustomerId = c.Id
                ORDER BY e.CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var list = new List<Estimation>();
            while (await reader.ReadAsync())
                list.Add(MapToEstimation(reader));

            foreach (var est in list)
                est.Items = (List<EstimationItem>)await GetItemsAsync(est.Id);

            return list;
        }

        public async Task<Estimation?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT e.*, c.CustomerName AS CustomerNameJoin
                FROM Sales_Estimations e
                LEFT JOIN Masters_Customers c ON e.CustomerId = c.Id
                WHERE e.Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;
            var estimation = MapToEstimation(reader);
            reader.Close();

            estimation.Items = (List<EstimationItem>)await GetItemsInConnectionAsync(connection, id);
            return estimation;
        }

        public async Task<IEnumerable<Estimation>> GetByCustomerIdAsync(int customerId)
        {
            const string query = @"
                SELECT e.*, c.CustomerName AS CustomerNameJoin
                FROM Sales_Estimations e
                LEFT JOIN Masters_Customers c ON e.CustomerId = c.Id
                WHERE e.CustomerId = @CustomerId
                ORDER BY e.CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var list = new List<Estimation>();
            while (await reader.ReadAsync())
                list.Add(MapToEstimation(reader));

            foreach (var est in list)
                est.Items = (List<EstimationItem>)await GetItemsAsync(est.Id);

            return list;
        }

        public async Task<IEnumerable<Estimation>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT e.*, c.CustomerName AS CustomerNameJoin
                FROM Sales_Estimations e
                LEFT JOIN Masters_Customers c ON e.CustomerId = c.Id
                WHERE e.Status = @Status
                ORDER BY e.CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var list = new List<Estimation>();
            while (await reader.ReadAsync())
                list.Add(MapToEstimation(reader));

            foreach (var est in list)
                est.Items = (List<EstimationItem>)await GetItemsAsync(est.Id);

            return list;
        }

        public async Task<int> InsertAsync(Estimation estimation)
        {
            const string insertEst = @"
                INSERT INTO Sales_Estimations
                    (EstimateNo, BaseEstimateNo, RevisionNumber, CustomerId, CustomerName, Status,
                     SubTotal, DiscountType, DiscountValue, DiscountAmount, TotalAmount,
                     ValidUntil, Notes, TermsAndConditions, CreatedAt, CreatedBy)
                VALUES
                    (@EstimateNo, @BaseEstimateNo, @RevisionNumber, @CustomerId, @CustomerName, @Status,
                     @SubTotal, @DiscountType, @DiscountValue, @DiscountAmount, @TotalAmount,
                     @ValidUntil, @Notes, @TermsAndConditions, @CreatedAt, @CreatedBy);
                SELECT SCOPE_IDENTITY();";

            const string insertItem = @"
                INSERT INTO Sales_EstimationItems
                    (EstimationId, ProductId, ProductName, PartCode, Quantity, UnitPrice, TotalPrice, Notes)
                VALUES
                    (@EstimationId, @ProductId, @ProductName, @PartCode, @Quantity, @UnitPrice, @TotalPrice, @Notes)";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand(insertEst, connection, transaction);
                cmd.Parameters.AddWithValue("@EstimateNo", estimation.EstimateNo);
                cmd.Parameters.AddWithValue("@BaseEstimateNo", estimation.BaseEstimateNo);
                cmd.Parameters.AddWithValue("@RevisionNumber", estimation.RevisionNumber);
                cmd.Parameters.AddWithValue("@CustomerId", estimation.CustomerId);
                cmd.Parameters.AddWithValue("@CustomerName", (object?)estimation.CustomerName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", estimation.Status);
                cmd.Parameters.AddWithValue("@SubTotal", estimation.SubTotal);
                cmd.Parameters.AddWithValue("@DiscountType", (object?)estimation.DiscountType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountValue", estimation.DiscountValue);
                cmd.Parameters.AddWithValue("@DiscountAmount", estimation.DiscountAmount);
                cmd.Parameters.AddWithValue("@TotalAmount", estimation.TotalAmount);
                cmd.Parameters.AddWithValue("@ValidUntil", estimation.ValidUntil.Date);
                cmd.Parameters.AddWithValue("@Notes", (object?)estimation.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TermsAndConditions", (object?)estimation.TermsAndConditions ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAt", estimation.CreatedAt);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)estimation.CreatedBy ?? DBNull.Value);

                var result = await cmd.ExecuteScalarAsync();
                var estimationId = Convert.ToInt32(result);

                foreach (var item in estimation.Items)
                {
                    using var itemCmd = new SqlCommand(insertItem, connection, transaction);
                    itemCmd.Parameters.AddWithValue("@EstimationId", estimationId);
                    itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                    itemCmd.Parameters.AddWithValue("@ProductName", (object?)item.ProductName ?? DBNull.Value);
                    itemCmd.Parameters.AddWithValue("@PartCode", (object?)item.PartCode ?? DBNull.Value);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@TotalPrice", item.TotalPrice);
                    itemCmd.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
                    await itemCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return estimationId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateStatusAsync(int id, string status, string? updatedBy = null, int? convertedOrderId = null, string? rejectionReason = null)
        {
            var setParts = new List<string> { "Status = @Status" };

            if (status == "Approved")
            {
                setParts.Add("ApprovedBy = @UpdatedBy");
                setParts.Add("ApprovedAt = GETUTCDATE()");
            }
            else if (status == "Rejected")
            {
                setParts.Add("RejectedBy = @UpdatedBy");
                setParts.Add("RejectedAt = GETUTCDATE()");
                setParts.Add("RejectionReason = @RejectionReason");
            }
            else if (status == "Converted")
            {
                setParts.Add("ConvertedOrderId = @ConvertedOrderId");
                setParts.Add("ConvertedAt = GETUTCDATE()");
            }

            var query = $"UPDATE Sales_Estimations SET {string.Join(", ", setParts)} WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@RejectionReason", (object?)rejectionReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConvertedOrderId", (object?)convertedOrderId ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Sales_Estimations WHERE Id = @Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<int> GetNextSequenceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var prefix = $"EST-{year:D4}{month:D2}-";

            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(BaseEstimateNo, 12, 3) AS INT)), 0)
                FROM Sales_Estimations
                WHERE BaseEstimateNo LIKE @Prefix + '%'
                  AND LEN(BaseEstimateNo) = 14
                  AND ISNUMERIC(SUBSTRING(BaseEstimateNo, 12, 3)) = 1";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Prefix", prefix);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) + 1;
        }

        private async Task<IEnumerable<EstimationItem>> GetItemsAsync(int estimationId)
        {
            const string query = "SELECT * FROM Sales_EstimationItems WHERE EstimationId = @EstimationId ORDER BY Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EstimationId", estimationId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var items = new List<EstimationItem>();
            while (await reader.ReadAsync())
                items.Add(MapToEstimationItem(reader));
            return items;
        }

        private async Task<IEnumerable<EstimationItem>> GetItemsInConnectionAsync(SqlConnection connection, int estimationId)
        {
            const string query = "SELECT * FROM Sales_EstimationItems WHERE EstimationId = @EstimationId ORDER BY Id";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EstimationId", estimationId);
            using var reader = await command.ExecuteReaderAsync();

            var items = new List<EstimationItem>();
            while (await reader.ReadAsync())
                items.Add(MapToEstimationItem(reader));
            return items;
        }

        private static Estimation MapToEstimation(SqlDataReader reader)
        {
            return new Estimation
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                EstimateNo = reader.GetString(reader.GetOrdinal("EstimateNo")),
                BaseEstimateNo = reader.GetString(reader.GetOrdinal("BaseEstimateNo")),
                RevisionNumber = reader.GetInt32(reader.GetOrdinal("RevisionNumber")),
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName"))
                    ? (reader.IsDBNull(reader.GetOrdinal("CustomerNameJoin")) ? null : reader.GetString(reader.GetOrdinal("CustomerNameJoin")))
                    : reader.GetString(reader.GetOrdinal("CustomerName")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                DiscountType = reader.IsDBNull(reader.GetOrdinal("DiscountType")) ? null : reader.GetString(reader.GetOrdinal("DiscountType")),
                DiscountValue = reader.GetDecimal(reader.GetOrdinal("DiscountValue")),
                DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                ValidUntil = reader.GetDateTime(reader.GetOrdinal("ValidUntil")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                RejectedBy = reader.IsDBNull(reader.GetOrdinal("RejectedBy")) ? null : reader.GetString(reader.GetOrdinal("RejectedBy")),
                RejectedAt = reader.IsDBNull(reader.GetOrdinal("RejectedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("RejectedAt")),
                RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                ConvertedOrderId = reader.IsDBNull(reader.GetOrdinal("ConvertedOrderId")) ? null : reader.GetInt32(reader.GetOrdinal("ConvertedOrderId")),
                ConvertedAt = reader.IsDBNull(reader.GetOrdinal("ConvertedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ConvertedAt")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                TermsAndConditions = reader.IsDBNull(reader.GetOrdinal("TermsAndConditions")) ? null : reader.GetString(reader.GetOrdinal("TermsAndConditions")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
            };
        }

        private static EstimationItem MapToEstimationItem(SqlDataReader reader)
        {
            return new EstimationItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                EstimationId = reader.GetInt32(reader.GetOrdinal("EstimationId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                PartCode = reader.IsDBNull(reader.GetOrdinal("PartCode")) ? null : reader.GetString(reader.GetOrdinal("PartCode")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
            };
        }
    }
}
