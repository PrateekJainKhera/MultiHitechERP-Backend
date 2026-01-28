using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Planning;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// JobCard repository implementation using ADO.NET
    /// </summary>
    public class JobCardRepository : IJobCardRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IJobCardDependencyRepository _dependencyRepository;

        public JobCardRepository(IDbConnectionFactory connectionFactory, IJobCardDependencyRepository dependencyRepository)
        {
            _connectionFactory = connectionFactory;
            _dependencyRepository = dependencyRepository;
        }

        public async Task<JobCard?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Planning_JobCards WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToJobCard(reader) : null;
        }

        public async Task<JobCard?> GetByJobCardNoAsync(string jobCardNo)
        {
            const string query = "SELECT * FROM Planning_JobCards WHERE JobCardNo = @JobCardNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardNo", jobCardNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToJobCard(reader) : null;
        }

        public async Task<IEnumerable<JobCard>> GetAllAsync()
        {
            const string query = "SELECT * FROM Planning_JobCards ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetByOrderIdAsync(int orderId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE OrderId = @OrderId
                ORDER BY StepNo, CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetByProcessIdAsync(int processId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE ProcessId = @ProcessId
                ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessId", processId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE Status = @Status
                ORDER BY Priority DESC, CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetReadyForSchedulingAsync()
        {
            const string query = @"
                SELECT jc.* FROM Planning_JobCards jc
                WHERE jc.Status = 'Pending'
                AND jc.MaterialStatus = 'Available'
                AND jc.ScheduleStatus = 'Not Scheduled'
                AND NOT EXISTS (
                    SELECT 1 FROM Planning_JobCardDependencies dep
                    WHERE dep.DependentJobCardId = jc.Id AND dep.IsResolved = 0
                )
                ORDER BY jc.Priority DESC, jc.CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetScheduledJobCardsAsync()
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE ScheduleStatus = 'Scheduled'
                ORDER BY ScheduledStartDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetInProgressJobCardsAsync()
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE Status = 'In Progress'
                ORDER BY ActualStartTime";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetBlockedJobCardsAsync()
        {
            const string query = @"
                SELECT DISTINCT jc.* FROM Planning_JobCards jc
                INNER JOIN Planning_JobCardDependencies dep ON dep.DependentJobCardId = jc.Id
                WHERE dep.IsResolved = 0
                AND jc.Status IN ('Pending', 'Ready')
                ORDER BY jc.Priority DESC, jc.CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetByMachineIdAsync(int machineId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE AssignedMachineId = @MachineId
                ORDER BY ScheduledStartDate, CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetByOperatorIdAsync(int operatorId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCards
                WHERE AssignedOperatorId = @OperatorId
                ORDER BY ScheduledStartDate, CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OperatorId", operatorId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<int> InsertAsync(JobCard jobCard)
        {
            const string query = @"
                INSERT INTO Planning_JobCards
                (JobCardNo, CreationType, OrderId, OrderNo, DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                 ChildPartId, ChildPartName, ChildPartTemplateId, ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                 WorkInstructions, QualityCheckpoints, SpecialNotes,
                 Quantity, CompletedQty, RejectedQty, ReworkQty, InProgressQty, Status,
                 AssignedMachineId, AssignedMachineName, AssignedOperatorId, AssignedOperatorName,
                 EstimatedSetupTimeMin, EstimatedCycleTimeMin, EstimatedTotalTimeMin, ActualStartTime, ActualEndTime, ActualTimeMin,
                 MaterialStatus, MaterialStatusUpdatedAt, ManufacturingDimensions, Priority,
                 ScheduleStatus, ScheduledStartDate, ScheduledEndDate, IsRework, ReworkOrderId, ParentJobCardId,
                 CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version)
                VALUES
                (@JobCardNo, @CreationType, @OrderId, @OrderNo, @DrawingId, @DrawingNumber, @DrawingRevision, @DrawingName, @DrawingSelectionType,
                 @ChildPartId, @ChildPartName, @ChildPartTemplateId, @ProcessId, @ProcessName, @ProcessCode, @StepNo, @ProcessTemplateId,
                 @WorkInstructions, @QualityCheckpoints, @SpecialNotes,
                 @Quantity, @CompletedQty, @RejectedQty, @ReworkQty, @InProgressQty, @Status,
                 @AssignedMachineId, @AssignedMachineName, @AssignedOperatorId, @AssignedOperatorName,
                 @EstimatedSetupTimeMin, @EstimatedCycleTimeMin, @EstimatedTotalTimeMin, @ActualStartTime, @ActualEndTime, @ActualTimeMin,
                 @MaterialStatus, @MaterialStatusUpdatedAt, @ManufacturingDimensions, @Priority,
                 @ScheduleStatus, @ScheduledStartDate, @ScheduledEndDate, @IsRework, @ReworkOrderId, @ParentJobCardId,
                 @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy, @Version);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            jobCard.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddJobCardParameters(command, jobCard);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var jobCardId = result != null ? Convert.ToInt32(result) : 0;

            return jobCardId;
        }

        public async Task<bool> UpdateAsync(JobCard jobCard)
        {
            const string query = @"
                UPDATE Planning_JobCards SET
                    JobCardNo = @JobCardNo,
                    CreationType = @CreationType,
                    OrderId = @OrderId,
                    OrderNo = @OrderNo,
                    DrawingId = @DrawingId,
                    DrawingNumber = @DrawingNumber,
                    DrawingRevision = @DrawingRevision,
                    DrawingName = @DrawingName,
                    DrawingSelectionType = @DrawingSelectionType,
                    ChildPartId = @ChildPartId,
                    ChildPartName = @ChildPartName,
                    ChildPartTemplateId = @ChildPartTemplateId,
                    ProcessId = @ProcessId,
                    ProcessName = @ProcessName,
                    ProcessCode = @ProcessCode,
                    StepNo = @StepNo,
                    ProcessTemplateId = @ProcessTemplateId,
                    WorkInstructions = @WorkInstructions,
                    QualityCheckpoints = @QualityCheckpoints,
                    SpecialNotes = @SpecialNotes,
                    Quantity = @Quantity,
                    CompletedQty = @CompletedQty,
                    RejectedQty = @RejectedQty,
                    ReworkQty = @ReworkQty,
                    InProgressQty = @InProgressQty,
                    Status = @Status,
                    AssignedMachineId = @AssignedMachineId,
                    AssignedMachineName = @AssignedMachineName,
                    AssignedOperatorId = @AssignedOperatorId,
                    AssignedOperatorName = @AssignedOperatorName,
                    EstimatedSetupTimeMin = @EstimatedSetupTimeMin,
                    EstimatedCycleTimeMin = @EstimatedCycleTimeMin,
                    EstimatedTotalTimeMin = @EstimatedTotalTimeMin,
                    ActualStartTime = @ActualStartTime,
                    ActualEndTime = @ActualEndTime,
                    ActualTimeMin = @ActualTimeMin,
                    MaterialStatus = @MaterialStatus,
                    MaterialStatusUpdatedAt = @MaterialStatusUpdatedAt,
                    ManufacturingDimensions = @ManufacturingDimensions,
                    Priority = @Priority,
                    ScheduleStatus = @ScheduleStatus,
                    ScheduledStartDate = @ScheduledStartDate,
                    ScheduledEndDate = @ScheduledEndDate,
                    IsRework = @IsRework,
                    ReworkOrderId = @ReworkOrderId,
                    ParentJobCardId = @ParentJobCardId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @Version
                WHERE Id = @Id";

            jobCard.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddJobCardParameters(command, jobCard);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateWithVersionCheckAsync(JobCard jobCard)
        {
            const string query = @"
                UPDATE Planning_JobCards SET
                    JobCardNo = @JobCardNo,
                    Status = @Status,
                    CompletedQty = @CompletedQty,
                    RejectedQty = @RejectedQty,
                    ReworkQty = @ReworkQty,
                    InProgressQty = @InProgressQty,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @Version + 1
                WHERE Id = @Id AND Version = @Version";

            jobCard.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", jobCard.Id);
            command.Parameters.AddWithValue("@JobCardNo", jobCard.JobCardNo);
            command.Parameters.AddWithValue("@Status", jobCard.Status);
            command.Parameters.AddWithValue("@CompletedQty", jobCard.CompletedQty);
            command.Parameters.AddWithValue("@RejectedQty", jobCard.RejectedQty);
            command.Parameters.AddWithValue("@ReworkQty", jobCard.ReworkQty);
            command.Parameters.AddWithValue("@InProgressQty", jobCard.InProgressQty);
            command.Parameters.AddWithValue("@UpdatedAt", jobCard.UpdatedAt ?? DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)jobCard.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Version", jobCard.Version);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Planning_JobCards WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET Status = @Status, UpdatedAt = @UpdatedAt
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

        public async Task<bool> UpdateMaterialStatusAsync(int id, string materialStatus)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET MaterialStatus = @MaterialStatus, MaterialStatusUpdatedAt = @MaterialStatusUpdatedAt, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@MaterialStatus", materialStatus);
            command.Parameters.AddWithValue("@MaterialStatusUpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateScheduleStatusAsync(int id, string scheduleStatus, DateTime? startDate, DateTime? endDate)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET ScheduleStatus = @ScheduleStatus,
                    ScheduledStartDate = @ScheduledStartDate,
                    ScheduledEndDate = @ScheduledEndDate,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ScheduleStatus", scheduleStatus);
            command.Parameters.AddWithValue("@ScheduledStartDate", (object?)startDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScheduledEndDate", (object?)endDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> AssignMachineAsync(int id, int machineId, string machineName)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET AssignedMachineId = @MachineId, AssignedMachineName = @MachineName, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@MachineId", machineId);
            command.Parameters.AddWithValue("@MachineName", machineName);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> AssignOperatorAsync(int id, int operatorId, string operatorName)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET AssignedOperatorId = @OperatorId, AssignedOperatorName = @OperatorName, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@OperatorId", operatorId);
            command.Parameters.AddWithValue("@OperatorName", operatorName);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> StartExecutionAsync(int id, DateTime startTime)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET Status = 'In Progress', ActualStartTime = @ActualStartTime, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ActualStartTime", startTime);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> CompleteExecutionAsync(int id, DateTime endTime, int actualTimeMin)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET Status = 'Completed', ActualEndTime = @ActualEndTime, ActualTimeMin = @ActualTimeMin, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ActualEndTime", endTime);
            command.Parameters.AddWithValue("@ActualTimeMin", actualTimeMin);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            // When job card completes, mark all dependencies as resolved
            await _dependencyRepository.MarkAllResolvedForPrerequisiteAsync(id);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateQuantitiesAsync(int id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty)
        {
            const string query = @"
                UPDATE Planning_JobCards
                SET CompletedQty = @CompletedQty,
                    RejectedQty = @RejectedQty,
                    ReworkQty = @ReworkQty,
                    InProgressQty = @InProgressQty,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@CompletedQty", completedQty);
            command.Parameters.AddWithValue("@RejectedQty", rejectedQty);
            command.Parameters.AddWithValue("@ReworkQty", reworkQty);
            command.Parameters.AddWithValue("@InProgressQty", inProgressQty);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<JobCard>> GetDependentJobCardsAsync(int jobCardId)
        {
            const string query = @"
                SELECT jc.* FROM Planning_JobCards jc
                INNER JOIN Planning_JobCardDependencies dep ON dep.DependentJobCardId = jc.Id
                WHERE dep.PrerequisiteJobCardId = @JobCardId
                ORDER BY jc.StepNo, jc.CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<IEnumerable<JobCard>> GetPrerequisiteJobCardsAsync(int jobCardId)
        {
            const string query = @"
                SELECT jc.* FROM Planning_JobCards jc
                INNER JOIN Planning_JobCardDependencies dep ON dep.PrerequisiteJobCardId = jc.Id
                WHERE dep.DependentJobCardId = @JobCardId
                ORDER BY jc.StepNo, jc.CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
            {
                jobCards.Add(MapToJobCard(reader));
            }

            return jobCards;
        }

        public async Task<bool> HasUnresolvedDependenciesAsync(int jobCardId)
        {
            return await _dependencyRepository.HasUnresolvedDependenciesAsync(jobCardId);
        }

        public async Task<int> GetVersionAsync(int id)
        {
            const string query = "SELECT Version FROM Planning_JobCards WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        // Helper Methods
        private static JobCard MapToJobCard(SqlDataReader reader)
        {
            return new JobCard
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                JobCardNo = reader.GetString(reader.GetOrdinal("JobCardNo")),
                CreationType = reader.GetString(reader.GetOrdinal("CreationType")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                DrawingId = reader.IsDBNull(reader.GetOrdinal("DrawingId")) ? null : reader.GetInt32(reader.GetOrdinal("DrawingId")),
                DrawingNumber = reader.IsDBNull(reader.GetOrdinal("DrawingNumber")) ? null : reader.GetString(reader.GetOrdinal("DrawingNumber")),
                DrawingRevision = reader.IsDBNull(reader.GetOrdinal("DrawingRevision")) ? null : reader.GetString(reader.GetOrdinal("DrawingRevision")),
                DrawingName = reader.IsDBNull(reader.GetOrdinal("DrawingName")) ? null : reader.GetString(reader.GetOrdinal("DrawingName")),
                DrawingSelectionType = reader.GetString(reader.GetOrdinal("DrawingSelectionType")),
                ChildPartId = reader.IsDBNull(reader.GetOrdinal("ChildPartId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartId")),
                ChildPartName = reader.IsDBNull(reader.GetOrdinal("ChildPartName")) ? null : reader.GetString(reader.GetOrdinal("ChildPartName")),
                ChildPartTemplateId = reader.IsDBNull(reader.GetOrdinal("ChildPartTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId")),
                ProcessId = reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessName = reader.IsDBNull(reader.GetOrdinal("ProcessName")) ? null : reader.GetString(reader.GetOrdinal("ProcessName")),
                ProcessCode = reader.IsDBNull(reader.GetOrdinal("ProcessCode")) ? null : reader.GetString(reader.GetOrdinal("ProcessCode")),
                StepNo = reader.IsDBNull(reader.GetOrdinal("StepNo")) ? null : reader.GetInt32(reader.GetOrdinal("StepNo")),
                ProcessTemplateId = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                WorkInstructions = reader.IsDBNull(reader.GetOrdinal("WorkInstructions")) ? null : reader.GetString(reader.GetOrdinal("WorkInstructions")),
                QualityCheckpoints = reader.IsDBNull(reader.GetOrdinal("QualityCheckpoints")) ? null : reader.GetString(reader.GetOrdinal("QualityCheckpoints")),
                SpecialNotes = reader.IsDBNull(reader.GetOrdinal("SpecialNotes")) ? null : reader.GetString(reader.GetOrdinal("SpecialNotes")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                CompletedQty = reader.GetInt32(reader.GetOrdinal("CompletedQty")),
                RejectedQty = reader.GetInt32(reader.GetOrdinal("RejectedQty")),
                ReworkQty = reader.GetInt32(reader.GetOrdinal("ReworkQty")),
                InProgressQty = reader.GetInt32(reader.GetOrdinal("InProgressQty")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AssignedMachineId = reader.IsDBNull(reader.GetOrdinal("AssignedMachineId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedMachineId")),
                AssignedMachineName = reader.IsDBNull(reader.GetOrdinal("AssignedMachineName")) ? null : reader.GetString(reader.GetOrdinal("AssignedMachineName")),
                AssignedOperatorId = reader.IsDBNull(reader.GetOrdinal("AssignedOperatorId")) ? null : reader.GetInt32(reader.GetOrdinal("AssignedOperatorId")),
                AssignedOperatorName = reader.IsDBNull(reader.GetOrdinal("AssignedOperatorName")) ? null : reader.GetString(reader.GetOrdinal("AssignedOperatorName")),
                EstimatedSetupTimeMin = reader.IsDBNull(reader.GetOrdinal("EstimatedSetupTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("EstimatedSetupTimeMin")),
                EstimatedCycleTimeMin = reader.IsDBNull(reader.GetOrdinal("EstimatedCycleTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("EstimatedCycleTimeMin")),
                EstimatedTotalTimeMin = reader.IsDBNull(reader.GetOrdinal("EstimatedTotalTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("EstimatedTotalTimeMin")),
                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                ActualEndTime = reader.IsDBNull(reader.GetOrdinal("ActualEndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualEndTime")),
                ActualTimeMin = reader.IsDBNull(reader.GetOrdinal("ActualTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("ActualTimeMin")),
                MaterialStatus = reader.GetString(reader.GetOrdinal("MaterialStatus")),
                MaterialStatusUpdatedAt = reader.IsDBNull(reader.GetOrdinal("MaterialStatusUpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("MaterialStatusUpdatedAt")),
                ManufacturingDimensions = reader.IsDBNull(reader.GetOrdinal("ManufacturingDimensions")) ? null : reader.GetString(reader.GetOrdinal("ManufacturingDimensions")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                ScheduleStatus = reader.GetString(reader.GetOrdinal("ScheduleStatus")),
                ScheduledStartDate = reader.IsDBNull(reader.GetOrdinal("ScheduledStartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ScheduledStartDate")),
                ScheduledEndDate = reader.IsDBNull(reader.GetOrdinal("ScheduledEndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ScheduledEndDate")),
                IsRework = reader.GetBoolean(reader.GetOrdinal("IsRework")),
                ReworkOrderId = reader.IsDBNull(reader.GetOrdinal("ReworkOrderId")) ? null : reader.GetInt32(reader.GetOrdinal("ReworkOrderId")),
                ParentJobCardId = reader.IsDBNull(reader.GetOrdinal("ParentJobCardId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentJobCardId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                Version = reader.GetInt32(reader.GetOrdinal("Version"))
            };
        }

        private static void AddJobCardParameters(SqlCommand command, JobCard jobCard)
        {
            command.Parameters.AddWithValue("@Id", jobCard.Id);
            command.Parameters.AddWithValue("@JobCardNo", jobCard.JobCardNo);
            command.Parameters.AddWithValue("@CreationType", jobCard.CreationType);
            command.Parameters.AddWithValue("@OrderId", jobCard.OrderId);
            command.Parameters.AddWithValue("@OrderNo", (object?)jobCard.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingId", (object?)jobCard.DrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)jobCard.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingRevision", (object?)jobCard.DrawingRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingName", (object?)jobCard.DrawingName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingSelectionType", jobCard.DrawingSelectionType);
            command.Parameters.AddWithValue("@ChildPartId", (object?)jobCard.ChildPartId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartName", (object?)jobCard.ChildPartName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartTemplateId", (object?)jobCard.ChildPartTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessId", jobCard.ProcessId);
            command.Parameters.AddWithValue("@ProcessName", (object?)jobCard.ProcessName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessCode", (object?)jobCard.ProcessCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@StepNo", (object?)jobCard.StepNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)jobCard.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@WorkInstructions", (object?)jobCard.WorkInstructions ?? DBNull.Value);
            command.Parameters.AddWithValue("@QualityCheckpoints", (object?)jobCard.QualityCheckpoints ?? DBNull.Value);
            command.Parameters.AddWithValue("@SpecialNotes", (object?)jobCard.SpecialNotes ?? DBNull.Value);
            command.Parameters.AddWithValue("@Quantity", jobCard.Quantity);
            command.Parameters.AddWithValue("@CompletedQty", jobCard.CompletedQty);
            command.Parameters.AddWithValue("@RejectedQty", jobCard.RejectedQty);
            command.Parameters.AddWithValue("@ReworkQty", jobCard.ReworkQty);
            command.Parameters.AddWithValue("@InProgressQty", jobCard.InProgressQty);
            command.Parameters.AddWithValue("@Status", jobCard.Status);
            command.Parameters.AddWithValue("@AssignedMachineId", (object?)jobCard.AssignedMachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@AssignedMachineName", (object?)jobCard.AssignedMachineName ?? DBNull.Value);
            command.Parameters.AddWithValue("@AssignedOperatorId", (object?)jobCard.AssignedOperatorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@AssignedOperatorName", (object?)jobCard.AssignedOperatorName ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedSetupTimeMin", (object?)jobCard.EstimatedSetupTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedCycleTimeMin", (object?)jobCard.EstimatedCycleTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedTotalTimeMin", (object?)jobCard.EstimatedTotalTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualStartTime", (object?)jobCard.ActualStartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualEndTime", (object?)jobCard.ActualEndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualTimeMin", (object?)jobCard.ActualTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialStatus", jobCard.MaterialStatus);
            command.Parameters.AddWithValue("@MaterialStatusUpdatedAt", (object?)jobCard.MaterialStatusUpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@ManufacturingDimensions", (object?)jobCard.ManufacturingDimensions ?? DBNull.Value);
            command.Parameters.AddWithValue("@Priority", jobCard.Priority);
            command.Parameters.AddWithValue("@ScheduleStatus", jobCard.ScheduleStatus);
            command.Parameters.AddWithValue("@ScheduledStartDate", (object?)jobCard.ScheduledStartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScheduledEndDate", (object?)jobCard.ScheduledEndDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsRework", jobCard.IsRework);
            command.Parameters.AddWithValue("@ReworkOrderId", (object?)jobCard.ReworkOrderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ParentJobCardId", (object?)jobCard.ParentJobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", jobCard.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)jobCard.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)jobCard.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)jobCard.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Version", jobCard.Version);
        }
    }
}
