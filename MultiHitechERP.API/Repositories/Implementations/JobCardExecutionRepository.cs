using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// JobCardExecution repository implementation using ADO.NET
    /// Tracks actual production execution with time and quantity tracking
    /// </summary>
    public class JobCardExecutionRepository : IJobCardExecutionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JobCardExecutionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JobCardExecution?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Production_JobCardExecutions WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToExecution(reader) : null;
        }

        public async Task<IEnumerable<JobCardExecution>> GetAllAsync()
        {
            const string query = "SELECT * FROM Production_JobCardExecutions ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<IEnumerable<JobCardExecution>> GetByJobCardIdAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE JobCardId = @JobCardId
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<IEnumerable<JobCardExecution>> GetByMachineIdAsync(Guid machineId)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE MachineId = @MachineId
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<IEnumerable<JobCardExecution>> GetByOperatorIdAsync(Guid operatorId)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE OperatorId = @OperatorId
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OperatorId", operatorId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<Guid> InsertAsync(JobCardExecution execution)
        {
            const string query = @"
                INSERT INTO Production_JobCardExecutions
                (Id, JobCardId, JobCardNo, OrderNo, MachineId, MachineName, OperatorId, OperatorName,
                 StartTime, EndTime, PausedTime, ResumedTime, TotalTimeMin, IdleTimeMin,
                 QuantityStarted, QuantityCompleted, QuantityRejected, QuantityInProgress,
                 ExecutionStatus, Notes, IssuesEncountered, CreatedAt, CreatedBy)
                VALUES
                (@Id, @JobCardId, @JobCardNo, @OrderNo, @MachineId, @MachineName, @OperatorId, @OperatorName,
                 @StartTime, @EndTime, @PausedTime, @ResumedTime, @TotalTimeMin, @IdleTimeMin,
                 @QuantityStarted, @QuantityCompleted, @QuantityRejected, @QuantityInProgress,
                 @ExecutionStatus, @Notes, @IssuesEncountered, @CreatedAt, @CreatedBy)";

            var executionId = Guid.NewGuid();
            execution.Id = executionId;
            execution.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddExecutionParameters(command, execution);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return executionId;
        }

        public async Task<bool> UpdateAsync(JobCardExecution execution)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions SET
                    JobCardId = @JobCardId,
                    JobCardNo = @JobCardNo,
                    OrderNo = @OrderNo,
                    MachineId = @MachineId,
                    MachineName = @MachineName,
                    OperatorId = @OperatorId,
                    OperatorName = @OperatorName,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    PausedTime = @PausedTime,
                    ResumedTime = @ResumedTime,
                    TotalTimeMin = @TotalTimeMin,
                    IdleTimeMin = @IdleTimeMin,
                    QuantityStarted = @QuantityStarted,
                    QuantityCompleted = @QuantityCompleted,
                    QuantityRejected = @QuantityRejected,
                    QuantityInProgress = @QuantityInProgress,
                    ExecutionStatus = @ExecutionStatus,
                    Notes = @Notes,
                    IssuesEncountered = @IssuesEncountered
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddExecutionParameters(command, execution);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Production_JobCardExecutions WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> StartExecutionAsync(Guid id, DateTime startTime)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions
                SET StartTime = @StartTime,
                    ExecutionStatus = 'Started'
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@StartTime", startTime);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> PauseExecutionAsync(Guid id, DateTime pausedTime)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions
                SET PausedTime = @PausedTime,
                    ExecutionStatus = 'Paused'
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@PausedTime", pausedTime);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ResumeExecutionAsync(Guid id, DateTime resumedTime)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions
                SET ResumedTime = @ResumedTime,
                    ExecutionStatus = 'InProgress',
                    IdleTimeMin = ISNULL(IdleTimeMin, 0) + DATEDIFF(MINUTE, PausedTime, @ResumedTime)
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ResumedTime", resumedTime);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> CompleteExecutionAsync(Guid id, DateTime endTime, int totalTimeMin)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions
                SET EndTime = @EndTime,
                    TotalTimeMin = @TotalTimeMin,
                    ExecutionStatus = 'Completed'
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@EndTime", endTime);
            command.Parameters.AddWithValue("@TotalTimeMin", totalTimeMin);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateQuantitiesAsync(Guid id, int? completed, int? rejected, int? inProgress)
        {
            const string query = @"
                UPDATE Production_JobCardExecutions
                SET QuantityCompleted = ISNULL(@Completed, QuantityCompleted),
                    QuantityRejected = ISNULL(@Rejected, QuantityRejected),
                    QuantityInProgress = ISNULL(@InProgress, QuantityInProgress)
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Completed", (object?)completed ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rejected", (object?)rejected ?? DBNull.Value);
            command.Parameters.AddWithValue("@InProgress", (object?)inProgress ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<JobCardExecution>> GetActiveExecutionsAsync()
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE ExecutionStatus IN ('Started', 'InProgress', 'Paused')
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<IEnumerable<JobCardExecution>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE ExecutionStatus = @Status
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<IEnumerable<JobCardExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE StartTime >= @StartDate AND StartTime <= @EndDate
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<JobCardExecution?> GetCurrentExecutionForJobCardAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT TOP 1 * FROM Production_JobCardExecutions
                WHERE JobCardId = @JobCardId
                AND ExecutionStatus IN ('Started', 'InProgress', 'Paused')
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToExecution(reader) : null;
        }

        public async Task<IEnumerable<JobCardExecution>> GetExecutionHistoryForJobCardAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT * FROM Production_JobCardExecutions
                WHERE JobCardId = @JobCardId
                ORDER BY StartTime DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var executions = new List<JobCardExecution>();
            while (await reader.ReadAsync())
            {
                executions.Add(MapToExecution(reader));
            }

            return executions;
        }

        public async Task<int> GetTotalExecutionTimeForJobCardAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT ISNULL(SUM(TotalTimeMin), 0) AS TotalTime
                FROM Production_JobCardExecutions
                WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<int> GetTotalCompletedQuantityForJobCardAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT ISNULL(SUM(QuantityCompleted), 0) AS TotalCompleted
                FROM Production_JobCardExecutions
                WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Helper Methods
        private static JobCardExecution MapToExecution(SqlDataReader reader)
        {
            return new JobCardExecution
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                JobCardId = reader.GetGuid(reader.GetOrdinal("JobCardId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo"))
                    ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo"))
                    ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                MachineId = reader.IsDBNull(reader.GetOrdinal("MachineId"))
                    ? null : reader.GetGuid(reader.GetOrdinal("MachineId")),
                MachineName = reader.IsDBNull(reader.GetOrdinal("MachineName"))
                    ? null : reader.GetString(reader.GetOrdinal("MachineName")),
                OperatorId = reader.IsDBNull(reader.GetOrdinal("OperatorId"))
                    ? null : reader.GetGuid(reader.GetOrdinal("OperatorId")),
                OperatorName = reader.IsDBNull(reader.GetOrdinal("OperatorName"))
                    ? null : reader.GetString(reader.GetOrdinal("OperatorName")),
                StartTime = reader.GetDateTime(reader.GetOrdinal("StartTime")),
                EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("EndTime")),
                PausedTime = reader.IsDBNull(reader.GetOrdinal("PausedTime"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("PausedTime")),
                ResumedTime = reader.IsDBNull(reader.GetOrdinal("ResumedTime"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ResumedTime")),
                TotalTimeMin = reader.IsDBNull(reader.GetOrdinal("TotalTimeMin"))
                    ? null : reader.GetInt32(reader.GetOrdinal("TotalTimeMin")),
                IdleTimeMin = reader.IsDBNull(reader.GetOrdinal("IdleTimeMin"))
                    ? null : reader.GetInt32(reader.GetOrdinal("IdleTimeMin")),
                QuantityStarted = reader.IsDBNull(reader.GetOrdinal("QuantityStarted"))
                    ? null : reader.GetInt32(reader.GetOrdinal("QuantityStarted")),
                QuantityCompleted = reader.IsDBNull(reader.GetOrdinal("QuantityCompleted"))
                    ? null : reader.GetInt32(reader.GetOrdinal("QuantityCompleted")),
                QuantityRejected = reader.IsDBNull(reader.GetOrdinal("QuantityRejected"))
                    ? null : reader.GetInt32(reader.GetOrdinal("QuantityRejected")),
                QuantityInProgress = reader.IsDBNull(reader.GetOrdinal("QuantityInProgress"))
                    ? null : reader.GetInt32(reader.GetOrdinal("QuantityInProgress")),
                ExecutionStatus = reader.GetString(reader.GetOrdinal("ExecutionStatus")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes"))
                    ? null : reader.GetString(reader.GetOrdinal("Notes")),
                IssuesEncountered = reader.IsDBNull(reader.GetOrdinal("IssuesEncountered"))
                    ? null : reader.GetString(reader.GetOrdinal("IssuesEncountered")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }

        private static void AddExecutionParameters(SqlCommand command, JobCardExecution execution)
        {
            command.Parameters.AddWithValue("@Id", execution.Id);
            command.Parameters.AddWithValue("@JobCardId", execution.JobCardId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)execution.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)execution.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineId", (object?)execution.MachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineName", (object?)execution.MachineName ?? DBNull.Value);
            command.Parameters.AddWithValue("@OperatorId", (object?)execution.OperatorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OperatorName", (object?)execution.OperatorName ?? DBNull.Value);
            command.Parameters.AddWithValue("@StartTime", execution.StartTime);
            command.Parameters.AddWithValue("@EndTime", (object?)execution.EndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@PausedTime", (object?)execution.PausedTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ResumedTime", (object?)execution.ResumedTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@TotalTimeMin", (object?)execution.TotalTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@IdleTimeMin", (object?)execution.IdleTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityStarted", (object?)execution.QuantityStarted ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityCompleted", (object?)execution.QuantityCompleted ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityRejected", (object?)execution.QuantityRejected ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityInProgress", (object?)execution.QuantityInProgress ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExecutionStatus", execution.ExecutionStatus);
            command.Parameters.AddWithValue("@Notes", (object?)execution.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IssuesEncountered", (object?)execution.IssuesEncountered ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", execution.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)execution.CreatedBy ?? DBNull.Value);
        }
    }
}
