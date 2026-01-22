using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Quality;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// QC Result Repository implementation using ADO.NET
    /// Manages quality inspection records, defects, and approval workflow
    /// </summary>
    public class QCResultRepository : IQCResultRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public QCResultRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<QCResult?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToQCResult(reader);
            }

            return null;
        }

        public async Task<IEnumerable<QCResult>> GetAllAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<int> InsertAsync(QCResult qcResult)
        {
            const string query = @"
                INSERT INTO Quality_QCResults (
                    Id, JobCardId, JobCardNo, OrderId, OrderNo,
                    InspectionType, InspectionDate, InspectedBy,
                    QuantityInspected, QuantityPassed, QuantityRejected, QuantityRework,
                    QCStatus, DefectDescription, DefectCategory, RejectionReason,
                    MeasurementData, CorrectiveAction, RequiresRework,
                    ApprovedBy, ApprovedAt, Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @Id, @JobCardId, @JobCardNo, @OrderId, @OrderNo,
                    @InspectionType, @InspectionDate, @InspectedBy,
                    @QuantityInspected, @QuantityPassed, @QuantityRejected, @QuantityRework,
                    @QCStatus, @DefectDescription, @DefectCategory, @RejectionReason,
                    @MeasurementData, @CorrectiveAction, @RequiresRework,
                    @ApprovedBy, @ApprovedAt, @Remarks, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            var id = 0;

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@JobCardId", qcResult.JobCardId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)qcResult.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderId", (object?)qcResult.OrderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)qcResult.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@InspectionType", qcResult.InspectionType);
            command.Parameters.AddWithValue("@InspectionDate", qcResult.InspectionDate);
            command.Parameters.AddWithValue("@InspectedBy", (object?)qcResult.InspectedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityInspected", qcResult.QuantityInspected);
            command.Parameters.AddWithValue("@QuantityPassed", qcResult.QuantityPassed);
            command.Parameters.AddWithValue("@QuantityRejected", qcResult.QuantityRejected);
            command.Parameters.AddWithValue("@QuantityRework", (object?)qcResult.QuantityRework ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCStatus", qcResult.QCStatus);
            command.Parameters.AddWithValue("@DefectDescription", (object?)qcResult.DefectDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefectCategory", (object?)qcResult.DefectCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@RejectionReason", (object?)qcResult.RejectionReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@MeasurementData", (object?)qcResult.MeasurementData ?? DBNull.Value);
            command.Parameters.AddWithValue("@CorrectiveAction", (object?)qcResult.CorrectiveAction ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequiresRework", qcResult.RequiresRework);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)qcResult.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedAt", (object?)qcResult.ApprovedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)qcResult.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)qcResult.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return id;
        }

        public async Task<bool> UpdateAsync(QCResult qcResult)
        {
            const string query = @"
                UPDATE Quality_QCResults SET
                    JobCardId = @JobCardId,
                    JobCardNo = @JobCardNo,
                    OrderId = @OrderId,
                    OrderNo = @OrderNo,
                    InspectionType = @InspectionType,
                    InspectionDate = @InspectionDate,
                    InspectedBy = @InspectedBy,
                    QuantityInspected = @QuantityInspected,
                    QuantityPassed = @QuantityPassed,
                    QuantityRejected = @QuantityRejected,
                    QuantityRework = @QuantityRework,
                    QCStatus = @QCStatus,
                    DefectDescription = @DefectDescription,
                    DefectCategory = @DefectCategory,
                    RejectionReason = @RejectionReason,
                    MeasurementData = @MeasurementData,
                    CorrectiveAction = @CorrectiveAction,
                    RequiresRework = @RequiresRework,
                    ApprovedBy = @ApprovedBy,
                    ApprovedAt = @ApprovedAt,
                    Remarks = @Remarks
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", qcResult.Id);
            command.Parameters.AddWithValue("@JobCardId", qcResult.JobCardId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)qcResult.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderId", (object?)qcResult.OrderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)qcResult.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@InspectionType", qcResult.InspectionType);
            command.Parameters.AddWithValue("@InspectionDate", qcResult.InspectionDate);
            command.Parameters.AddWithValue("@InspectedBy", (object?)qcResult.InspectedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityInspected", qcResult.QuantityInspected);
            command.Parameters.AddWithValue("@QuantityPassed", qcResult.QuantityPassed);
            command.Parameters.AddWithValue("@QuantityRejected", qcResult.QuantityRejected);
            command.Parameters.AddWithValue("@QuantityRework", (object?)qcResult.QuantityRework ?? DBNull.Value);
            command.Parameters.AddWithValue("@QCStatus", qcResult.QCStatus);
            command.Parameters.AddWithValue("@DefectDescription", (object?)qcResult.DefectDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefectCategory", (object?)qcResult.DefectCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@RejectionReason", (object?)qcResult.RejectionReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@MeasurementData", (object?)qcResult.MeasurementData ?? DBNull.Value);
            command.Parameters.AddWithValue("@CorrectiveAction", (object?)qcResult.CorrectiveAction ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequiresRework", qcResult.RequiresRework);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)qcResult.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedAt", (object?)qcResult.ApprovedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)qcResult.Remarks ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Quality_QCResults WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<QCResult>> GetByJobCardIdAsync(int jobCardId)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE JobCardId = @JobCardId ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByOrderIdAsync(int orderId)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE OrderId = @OrderId ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByInspectionTypeAsync(string inspectionType)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE InspectionType = @InspectionType ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@InspectionType", inspectionType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByQCStatusAsync(string qcStatus)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE QCStatus = @QCStatus ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@QCStatus", qcStatus);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByInspectorAsync(string inspectorName)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE InspectedBy = @InspectorName ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@InspectorName", inspectorName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE InspectionDate BETWEEN @StartDate AND @EndDate ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetDefectiveResultsAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE QuantityRejected > 0 OR QuantityRework > 0 ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetByDefectCategoryAsync(string category)
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE DefectCategory = @Category ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetReworkRequiredAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE RequiresRework = 1 ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<bool> UpdateQCStatusAsync(int id, string qcStatus)
        {
            const string query = "UPDATE Quality_QCResults SET QCStatus = @QCStatus WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@QCStatus", qcStatus);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ApproveQCResultAsync(int id, string approvedBy, DateTime approvedAt)
        {
            const string query = @"
                UPDATE Quality_QCResults
                SET ApprovedBy = @ApprovedBy,
                    ApprovedAt = @ApprovedAt,
                    QCStatus = 'Approved'
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ApprovedBy", approvedBy);
            command.Parameters.AddWithValue("@ApprovedAt", approvedAt);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<QCResult>> GetPendingQCAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE QCStatus = 'Pending' ORDER BY InspectionDate ASC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetFailedQCAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE QCStatus = 'Failed' ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<IEnumerable<QCResult>> GetApprovedResultsAsync()
        {
            const string query = "SELECT * FROM Quality_QCResults WHERE QCStatus = 'Approved' AND ApprovedBy IS NOT NULL ORDER BY ApprovedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var results = new List<QCResult>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToQCResult(reader));
            }

            return results;
        }

        public async Task<int> GetTotalInspectedQuantityForJobCardAsync(int jobCardId)
        {
            const string query = "SELECT ISNULL(SUM(QuantityInspected), 0) FROM Quality_QCResults WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> GetTotalPassedQuantityForJobCardAsync(int jobCardId)
        {
            const string query = "SELECT ISNULL(SUM(QuantityPassed), 0) FROM Quality_QCResults WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> GetTotalRejectedQuantityForJobCardAsync(int jobCardId)
        {
            const string query = "SELECT ISNULL(SUM(QuantityRejected), 0) FROM Quality_QCResults WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<decimal> GetPassRateForJobCardAsync(int jobCardId)
        {
            const string query = @"
                SELECT
                    CASE
                        WHEN SUM(QuantityInspected) = 0 THEN 0
                        ELSE CAST(SUM(QuantityPassed) AS DECIMAL(10,2)) / SUM(QuantityInspected) * 100
                    END
                FROM Quality_QCResults
                WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        public async Task<decimal> GetOverallPassRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            string query = @"
                SELECT
                    CASE
                        WHEN SUM(QuantityInspected) = 0 THEN 0
                        ELSE CAST(SUM(QuantityPassed) AS DECIMAL(10,2)) / SUM(QuantityInspected) * 100
                    END
                FROM Quality_QCResults";

            if (startDate.HasValue && endDate.HasValue)
            {
                query += " WHERE InspectionDate BETWEEN @StartDate AND @EndDate";
            }

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            if (startDate.HasValue && endDate.HasValue)
            {
                command.Parameters.AddWithValue("@StartDate", startDate.Value);
                command.Parameters.AddWithValue("@EndDate", endDate.Value);
            }

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }

        public async Task<QCResult?> GetLatestResultForJobCardAsync(int jobCardId)
        {
            const string query = "SELECT TOP 1 * FROM Quality_QCResults WHERE JobCardId = @JobCardId ORDER BY InspectionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToQCResult(reader);
            }

            return null;
        }

        private static QCResult MapToQCResult(IDataReader reader)
        {
            return new QCResult
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                JobCardId = reader.GetInt32(reader.GetOrdinal("JobCardId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo")) ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId")) ? null : reader.GetInt32(reader.GetOrdinal("OrderId")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                InspectionType = reader.GetString(reader.GetOrdinal("InspectionType")),
                InspectionDate = reader.GetDateTime(reader.GetOrdinal("InspectionDate")),
                InspectedBy = reader.IsDBNull(reader.GetOrdinal("InspectedBy")) ? null : reader.GetString(reader.GetOrdinal("InspectedBy")),
                QuantityInspected = reader.GetInt32(reader.GetOrdinal("QuantityInspected")),
                QuantityPassed = reader.GetInt32(reader.GetOrdinal("QuantityPassed")),
                QuantityRejected = reader.GetInt32(reader.GetOrdinal("QuantityRejected")),
                QuantityRework = reader.IsDBNull(reader.GetOrdinal("QuantityRework")) ? null : reader.GetInt32(reader.GetOrdinal("QuantityRework")),
                QCStatus = reader.GetString(reader.GetOrdinal("QCStatus")),
                DefectDescription = reader.IsDBNull(reader.GetOrdinal("DefectDescription")) ? null : reader.GetString(reader.GetOrdinal("DefectDescription")),
                DefectCategory = reader.IsDBNull(reader.GetOrdinal("DefectCategory")) ? null : reader.GetString(reader.GetOrdinal("DefectCategory")),
                RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                MeasurementData = reader.IsDBNull(reader.GetOrdinal("MeasurementData")) ? null : reader.GetString(reader.GetOrdinal("MeasurementData")),
                CorrectiveAction = reader.IsDBNull(reader.GetOrdinal("CorrectiveAction")) ? null : reader.GetString(reader.GetOrdinal("CorrectiveAction")),
                RequiresRework = reader.GetBoolean(reader.GetOrdinal("RequiresRework")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}
