using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// MaterialIssue repository implementation using ADO.NET
    /// Tracks physical material issuance to production
    /// </summary>
    public class MaterialIssueRepository : IMaterialIssueRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialIssueRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MaterialIssue?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Stores_MaterialIssues WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToIssue(reader) : null;
        }

        public async Task<MaterialIssue?> GetByIssueNoAsync(string issueNo)
        {
            const string query = "SELECT * FROM Stores_MaterialIssues WHERE IssueNo = @IssueNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IssueNo", issueNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToIssue(reader) : null;
        }

        public async Task<IEnumerable<MaterialIssue>> GetAllAsync()
        {
            const string query = "SELECT * FROM Stores_MaterialIssues ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByRequisitionIdAsync(int requisitionId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE RequisitionId = @RequisitionId
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByJobCardNoAsync(string jobCardNo)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE JobCardNo = @JobCardNo
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardNo", jobCardNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByOrderNoAsync(string orderNo)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE OrderNo = @OrderNo
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderNo", orderNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<int> InsertAsync(MaterialIssue issue)
        {
            const string query = @"
                INSERT INTO Stores_MaterialIssues
                (IssueNo, IssueDate, RequisitionId, JobCardNo, OrderNo, MaterialName, MaterialGrade,
                 TotalPieces, TotalIssuedLengthMM, TotalIssuedWeightKG, Status,
                 IssuedById, IssuedByName, ReceivedById, ReceivedByName)
                VALUES
                (@IssueNo, @IssueDate, @RequisitionId, @JobCardNo, @OrderNo, @MaterialName, @MaterialGrade,
                 @TotalPieces, @TotalIssuedLengthMM, @TotalIssuedWeightKG, @Status,
                 @IssuedById, @IssuedByName, @ReceivedById, @ReceivedByName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddIssueParameters(command, issue);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? (int)result : 0;
        }

        public async Task<bool> UpdateAsync(MaterialIssue issue)
        {
            const string query = @"
                UPDATE Stores_MaterialIssues SET
                    IssueNo = @IssueNo,
                    IssueDate = @IssueDate,
                    RequisitionId = @RequisitionId,
                    JobCardNo = @JobCardNo,
                    OrderNo = @OrderNo,
                    MaterialName = @MaterialName,
                    MaterialGrade = @MaterialGrade,
                    TotalPieces = @TotalPieces,
                    TotalIssuedLengthMM = @TotalIssuedLengthMM,
                    TotalIssuedWeightKG = @TotalIssuedWeightKG,
                    Status = @Status,
                    IssuedById = @IssuedById,
                    IssuedByName = @IssuedByName,
                    ReceivedById = @ReceivedById,
                    ReceivedByName = @ReceivedByName
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddIssueParameters(command, issue);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Stores_MaterialIssues WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE Status = @Status
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE IssueDate >= @StartDate AND IssueDate <= @EndDate
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByIssuedByAsync(string issuedById)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE IssuedById = @IssuedById
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@IssuedById", issuedById);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<IEnumerable<MaterialIssue>> GetByReceivedByAsync(string receivedById)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialIssues
                WHERE ReceivedById = @ReceivedById
                ORDER BY IssueDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ReceivedById", receivedById);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var issues = new List<MaterialIssue>();
            while (await reader.ReadAsync())
            {
                issues.Add(MapToIssue(reader));
            }

            return issues;
        }

        public async Task<decimal> GetTotalIssuedQuantityAsync(int requisitionId)
        {
            const string query = @"
                SELECT ISNULL(SUM(TotalIssuedLengthMM), 0) AS TotalIssued
                FROM Stores_MaterialIssues
                WHERE RequisitionId = @RequisitionId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToDecimal(result) : 0m;
        }

        // Helper Methods
        private static MaterialIssue MapToIssue(SqlDataReader reader)
        {
            return new MaterialIssue
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                IssueNo = reader.GetString(reader.GetOrdinal("IssueNo")),
                IssueDate = reader.GetDateTime(reader.GetOrdinal("IssueDate")),
                RequisitionId = reader.GetInt32(reader.GetOrdinal("RequisitionId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo"))
                    ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo"))
                    ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                MaterialName = reader.IsDBNull(reader.GetOrdinal("MaterialName"))
                    ? null : reader.GetString(reader.GetOrdinal("MaterialName")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade"))
                    ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                TotalPieces = reader.IsDBNull(reader.GetOrdinal("TotalPieces"))
                    ? null : reader.GetInt32(reader.GetOrdinal("TotalPieces")),
                TotalIssuedLengthMM = reader.IsDBNull(reader.GetOrdinal("TotalIssuedLengthMM"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("TotalIssuedLengthMM")),
                TotalIssuedWeightKG = reader.IsDBNull(reader.GetOrdinal("TotalIssuedWeightKG"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("TotalIssuedWeightKG")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                IssuedById = reader.IsDBNull(reader.GetOrdinal("IssuedById"))
                    ? null : reader.GetString(reader.GetOrdinal("IssuedById")),
                IssuedByName = reader.IsDBNull(reader.GetOrdinal("IssuedByName"))
                    ? null : reader.GetString(reader.GetOrdinal("IssuedByName")),
                ReceivedById = reader.IsDBNull(reader.GetOrdinal("ReceivedById"))
                    ? null : reader.GetString(reader.GetOrdinal("ReceivedById")),
                ReceivedByName = reader.IsDBNull(reader.GetOrdinal("ReceivedByName"))
                    ? null : reader.GetString(reader.GetOrdinal("ReceivedByName"))
            };
        }

        private static void AddIssueParameters(SqlCommand command, MaterialIssue issue)
        {
            command.Parameters.AddWithValue("@Id", issue.Id);
            command.Parameters.AddWithValue("@IssueNo", issue.IssueNo);
            command.Parameters.AddWithValue("@IssueDate", issue.IssueDate);
            command.Parameters.AddWithValue("@RequisitionId", issue.RequisitionId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)issue.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)issue.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)issue.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)issue.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalPieces", (object?)issue.TotalPieces ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalIssuedLengthMM", (object?)issue.TotalIssuedLengthMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalIssuedWeightKG", (object?)issue.TotalIssuedWeightKG ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", issue.Status);
            command.Parameters.AddWithValue("@IssuedById", (object?)issue.IssuedById ?? DBNull.Value);
            command.Parameters.AddWithValue("@IssuedByName", (object?)issue.IssuedByName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceivedById", (object?)issue.ReceivedById ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceivedByName", (object?)issue.ReceivedByName ?? DBNull.Value);
        }
    }
}
