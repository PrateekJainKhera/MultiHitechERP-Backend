using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ScheduleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MachineSchedule?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToSchedule(reader) : null;
        }

        public async Task<IEnumerable<MachineSchedule>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                ORDER BY ScheduledStartTime DESC";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<IEnumerable<MachineSchedule>> GetByMachineIdAsync(int machineId)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE MachineId = @MachineId
                ORDER BY ScheduledStartTime DESC";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<IEnumerable<MachineSchedule>> GetByJobCardIdAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE JobCardId = @JobCardId
                ORDER BY ScheduledStartTime";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<IEnumerable<MachineSchedule>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE Status = @Status
                ORDER BY ScheduledStartTime";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<IEnumerable<MachineSchedule>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE ScheduledStartTime >= @StartDate AND ScheduledStartTime <= @EndDate
                ORDER BY ScheduledStartTime";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<IEnumerable<MachineSchedule>> GetByMachineAndDateRangeAsync(int machineId, DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT * FROM Scheduling_MachineSchedules
                WHERE MachineId = @MachineId
                  AND ScheduledStartTime >= @StartDate
                  AND ScheduledStartTime <= @EndDate
                ORDER BY ScheduledStartTime";

            var schedules = new List<MachineSchedule>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                schedules.Add(MapToSchedule(reader));

            return schedules;
        }

        public async Task<bool> HasConflictAsync(int machineId, DateTime startTime, DateTime endTime, int? excludeScheduleId = null)
        {
            const string query = @"
                SELECT COUNT(1) FROM Scheduling_MachineSchedules
                WHERE MachineId = @MachineId
                  AND Status IN ('Scheduled', 'InProgress')
                  AND (@ExcludeId IS NULL OR Id != @ExcludeId)
                  AND (
                      (ScheduledStartTime >= @StartTime AND ScheduledStartTime < @EndTime)
                      OR (ScheduledEndTime > @StartTime AND ScheduledEndTime <= @EndTime)
                      OR (ScheduledStartTime <= @StartTime AND ScheduledEndTime >= @EndTime)
                  )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId);
            command.Parameters.AddWithValue("@StartTime", startTime);
            command.Parameters.AddWithValue("@EndTime", endTime);
            command.Parameters.AddWithValue("@ExcludeId", (object?)excludeScheduleId ?? DBNull.Value);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<int> InsertAsync(MachineSchedule schedule)
        {
            const string query = @"
                INSERT INTO Scheduling_MachineSchedules (
                    JobCardId, JobCardNo, MachineId, MachineCode, MachineName,
                    ScheduledStartTime, ScheduledEndTime, EstimatedDurationMinutes,
                    Status, SchedulingMethod, SuggestedBySystem, ConfirmedBy, ConfirmedAt,
                    ProcessId, ProcessName, ProcessCode,
                    Notes, CreatedAt, CreatedBy
                ) VALUES (
                    @JobCardId, @JobCardNo, @MachineId, @MachineCode, @MachineName,
                    @ScheduledStartTime, @ScheduledEndTime, @EstimatedDurationMinutes,
                    @Status, @SchedulingMethod, @SuggestedBySystem, @ConfirmedBy, @ConfirmedAt,
                    @ProcessId, @ProcessName, @ProcessCode,
                    @Notes, @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            schedule.CreatedAt = DateTime.UtcNow;
            AddScheduleParameters(command, schedule);

            await connection.OpenAsync();
            var scheduleId = (int)await command.ExecuteScalarAsync();
            schedule.Id = scheduleId;

            return scheduleId;
        }

        public async Task<bool> UpdateAsync(MachineSchedule schedule)
        {
            const string query = @"
                UPDATE Scheduling_MachineSchedules SET
                    ScheduledStartTime = @ScheduledStartTime,
                    ScheduledEndTime = @ScheduledEndTime,
                    EstimatedDurationMinutes = @EstimatedDurationMinutes,
                    Status = @Status,
                    ActualStartTime = @ActualStartTime,
                    ActualEndTime = @ActualEndTime,
                    ActualDurationMinutes = @ActualDurationMinutes,
                    IsRescheduled = @IsRescheduled,
                    RescheduledFromId = @RescheduledFromId,
                    RescheduledReason = @RescheduledReason,
                    RescheduledAt = @RescheduledAt,
                    RescheduledBy = @RescheduledBy,
                    Notes = @Notes,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            schedule.UpdatedAt = DateTime.UtcNow;
            AddScheduleParameters(command, schedule);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Scheduling_MachineSchedules WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status, string? updatedBy = null)
        {
            const string query = @"
                UPDATE Scheduling_MachineSchedules
                SET Status = @Status, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static MachineSchedule MapToSchedule(SqlDataReader reader)
        {
            return new MachineSchedule
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                JobCardId = reader.GetInt32(reader.GetOrdinal("JobCardId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo")) ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                MachineId = reader.IsDBNull(reader.GetOrdinal("MachineId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("MachineId")),
                MachineCode = reader.IsDBNull(reader.GetOrdinal("MachineCode")) ? null : reader.GetString(reader.GetOrdinal("MachineCode")),
                MachineName = reader.IsDBNull(reader.GetOrdinal("MachineName")) ? null : reader.GetString(reader.GetOrdinal("MachineName")),
                ScheduledStartTime = reader.GetDateTime(reader.GetOrdinal("ScheduledStartTime")),
                ScheduledEndTime = reader.GetDateTime(reader.GetOrdinal("ScheduledEndTime")),
                EstimatedDurationMinutes = reader.GetInt32(reader.GetOrdinal("EstimatedDurationMinutes")),
                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                ActualEndTime = reader.IsDBNull(reader.GetOrdinal("ActualEndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualEndTime")),
                ActualDurationMinutes = reader.IsDBNull(reader.GetOrdinal("ActualDurationMinutes")) ? null : reader.GetInt32(reader.GetOrdinal("ActualDurationMinutes")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                SchedulingMethod = reader.GetString(reader.GetOrdinal("SchedulingMethod")),
                SuggestedBySystem = reader.GetBoolean(reader.GetOrdinal("SuggestedBySystem")),
                ConfirmedBy = reader.IsDBNull(reader.GetOrdinal("ConfirmedBy")) ? null : reader.GetString(reader.GetOrdinal("ConfirmedBy")),
                ConfirmedAt = reader.IsDBNull(reader.GetOrdinal("ConfirmedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ConfirmedAt")),
                ProcessId = reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessName = reader.IsDBNull(reader.GetOrdinal("ProcessName")) ? null : reader.GetString(reader.GetOrdinal("ProcessName")),
                ProcessCode = reader.IsDBNull(reader.GetOrdinal("ProcessCode")) ? null : reader.GetString(reader.GetOrdinal("ProcessCode")),
                IsRescheduled = reader.GetBoolean(reader.GetOrdinal("IsRescheduled")),
                RescheduledFromId = reader.IsDBNull(reader.GetOrdinal("RescheduledFromId")) ? null : reader.GetInt32(reader.GetOrdinal("RescheduledFromId")),
                RescheduledReason = reader.IsDBNull(reader.GetOrdinal("RescheduledReason")) ? null : reader.GetString(reader.GetOrdinal("RescheduledReason")),
                RescheduledAt = reader.IsDBNull(reader.GetOrdinal("RescheduledAt")) ? null : reader.GetDateTime(reader.GetOrdinal("RescheduledAt")),
                RescheduledBy = reader.IsDBNull(reader.GetOrdinal("RescheduledBy")) ? null : reader.GetString(reader.GetOrdinal("RescheduledBy")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddScheduleParameters(SqlCommand command, MachineSchedule schedule)
        {
            command.Parameters.AddWithValue("@Id", schedule.Id);
            command.Parameters.AddWithValue("@JobCardId", schedule.JobCardId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)schedule.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineId", (object?)schedule.MachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineCode", (object?)schedule.MachineCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineName", (object?)schedule.MachineName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScheduledStartTime", schedule.ScheduledStartTime);
            command.Parameters.AddWithValue("@ScheduledEndTime", schedule.ScheduledEndTime);
            command.Parameters.AddWithValue("@EstimatedDurationMinutes", schedule.EstimatedDurationMinutes);
            command.Parameters.AddWithValue("@ActualStartTime", (object?)schedule.ActualStartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualEndTime", (object?)schedule.ActualEndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualDurationMinutes", (object?)schedule.ActualDurationMinutes ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", schedule.Status);
            command.Parameters.AddWithValue("@SchedulingMethod", schedule.SchedulingMethod);
            command.Parameters.AddWithValue("@SuggestedBySystem", schedule.SuggestedBySystem);
            command.Parameters.AddWithValue("@ConfirmedBy", (object?)schedule.ConfirmedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConfirmedAt", (object?)schedule.ConfirmedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessId", schedule.ProcessId);
            command.Parameters.AddWithValue("@ProcessName", (object?)schedule.ProcessName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessCode", (object?)schedule.ProcessCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsRescheduled", schedule.IsRescheduled);
            command.Parameters.AddWithValue("@RescheduledFromId", (object?)schedule.RescheduledFromId ?? DBNull.Value);
            command.Parameters.AddWithValue("@RescheduledReason", (object?)schedule.RescheduledReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@RescheduledAt", (object?)schedule.RescheduledAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@RescheduledBy", (object?)schedule.RescheduledBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)schedule.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", schedule.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)schedule.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)schedule.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)schedule.UpdatedBy ?? DBNull.Value);
        }
    }
}
