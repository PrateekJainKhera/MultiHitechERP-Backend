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
            const string query = @"
                SELECT jc.Id, jc.JobCardNo, jc.CreationType, jc.OrderId, jc.OrderNo, jc.OrderItemId, jc.ItemSequence,
                       jc.DrawingId, jc.DrawingNumber, jc.DrawingRevision, jc.DrawingName, jc.DrawingSelectionType,
                       jc.ChildPartId, jc.ChildPartName, jc.ChildPartTemplateId,
                       jc.ProcessId, jc.ProcessName, jc.ProcessCode, jc.StepNo, jc.ProcessTemplateId,
                       jc.WorkInstructions, jc.QualityCheckpoints, jc.SpecialNotes,
                       jc.Quantity, jc.Status, jc.Priority, jc.ManufacturingDimensions,
                       jc.ProductionStatus, jc.ActualStartTime, jc.ActualEndTime,
                       jc.CompletedQty, jc.RejectedQty, jc.ReadyForAssembly,
                       jc.CreatedAt, jc.CreatedBy, jc.UpdatedAt, jc.UpdatedBy, jc.Version,
                       p.ModelName AS MachineModelName
                FROM Planning_JobCards jc
                LEFT JOIN Orders_OrderItems oi ON jc.OrderItemId = oi.Id
                LEFT JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE jc.Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToJobCard(reader) : null;
        }

        public async Task<JobCard?> GetByJobCardNoAsync(string jobCardNo)
        {
            const string query = @"
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards WHERE JobCardNo = @JobCardNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardNo", jobCardNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToJobCard(reader) : null;
        }

        public async Task<IEnumerable<JobCard>> GetAllAsync()
        {
            const string query = @"
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards ORDER BY CreatedAt DESC";

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
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards
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

        public async Task<IEnumerable<JobCard>> GetByOrderItemIdAsync(int orderItemId)
        {
            const string query = @"
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards
                WHERE OrderItemId = @OrderItemId
                ORDER BY StepNo, CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderItemId", orderItemId);

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
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards
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
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards
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

        public async Task<int> InsertAsync(JobCard jobCard)
        {
            const string query = @"
                INSERT INTO Planning_JobCards
                (JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                 DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                 ChildPartId, ChildPartName, ChildPartTemplateId,
                 ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                 WorkInstructions, QualityCheckpoints, SpecialNotes,
                 Quantity, Status, Priority, ManufacturingDimensions,
                 CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version)
                VALUES
                (@JobCardNo, @CreationType, @OrderId, @OrderNo, @OrderItemId, @ItemSequence,
                 @DrawingId, @DrawingNumber, @DrawingRevision, @DrawingName, @DrawingSelectionType,
                 @ChildPartId, @ChildPartName, @ChildPartTemplateId,
                 @ProcessId, @ProcessName, @ProcessCode, @StepNo, @ProcessTemplateId,
                 @WorkInstructions, @QualityCheckpoints, @SpecialNotes,
                 @Quantity, @Status, @Priority, @ManufacturingDimensions,
                 @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy, @Version);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            jobCard.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddJobCardParameters(command, jobCard);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
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
                    Status = @Status,
                    Priority = @Priority,
                    ManufacturingDimensions = @ManufacturingDimensions,
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

        public async Task<IEnumerable<JobCard>> GetDependentJobCardsAsync(int jobCardId)
        {
            const string query = @"
                SELECT jc.Id, jc.JobCardNo, jc.CreationType, jc.OrderId, jc.OrderNo,
                       jc.DrawingId, jc.DrawingNumber, jc.DrawingRevision, jc.DrawingName, jc.DrawingSelectionType,
                       jc.ChildPartId, jc.ChildPartName, jc.ChildPartTemplateId,
                       jc.ProcessId, jc.ProcessName, jc.ProcessCode, jc.StepNo, jc.ProcessTemplateId,
                       jc.WorkInstructions, jc.QualityCheckpoints, jc.SpecialNotes,
                       jc.Quantity, jc.Status, jc.Priority, jc.ManufacturingDimensions,
                       jc.CreatedAt, jc.CreatedBy, jc.UpdatedAt, jc.UpdatedBy, jc.Version
                FROM Planning_JobCards jc
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
                SELECT jc.Id, jc.JobCardNo, jc.CreationType, jc.OrderId, jc.OrderNo,
                       jc.DrawingId, jc.DrawingNumber, jc.DrawingRevision, jc.DrawingName, jc.DrawingSelectionType,
                       jc.ChildPartId, jc.ChildPartName, jc.ChildPartTemplateId,
                       jc.ProcessId, jc.ProcessName, jc.ProcessCode, jc.StepNo, jc.ProcessTemplateId,
                       jc.WorkInstructions, jc.QualityCheckpoints, jc.SpecialNotes,
                       jc.Quantity, jc.Status, jc.Priority, jc.ManufacturingDimensions,
                       jc.CreatedAt, jc.CreatedBy, jc.UpdatedAt, jc.UpdatedBy, jc.Version
                FROM Planning_JobCards jc
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

        public async Task<IEnumerable<JobCard>> GetBlockedJobCardsAsync()
        {
            const string query = @"
                SELECT DISTINCT jc.Id, jc.JobCardNo, jc.CreationType, jc.OrderId, jc.OrderNo, jc.OrderItemId, jc.ItemSequence,
                       jc.DrawingId, jc.DrawingNumber, jc.DrawingRevision, jc.DrawingName, jc.DrawingSelectionType,
                       jc.ChildPartId, jc.ChildPartName, jc.ChildPartTemplateId,
                       jc.ProcessId, jc.ProcessName, jc.ProcessCode, jc.StepNo, jc.ProcessTemplateId,
                       jc.WorkInstructions, jc.QualityCheckpoints, jc.SpecialNotes,
                       jc.Quantity, jc.Status, jc.Priority, jc.ManufacturingDimensions,
                       jc.ProductionStatus, jc.ActualStartTime, jc.ActualEndTime,
                       jc.CompletedQty, jc.RejectedQty, jc.ReadyForAssembly,
                       jc.CreatedAt, jc.CreatedBy, jc.UpdatedAt, jc.UpdatedBy, jc.Version
                FROM Planning_JobCards jc
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

        public async Task<bool> UpdateProductionStatusAsync(int id, string productionStatus, DateTime? actualStartTime, DateTime? actualEndTime, int completedQty, int rejectedQty)
        {
            const string query = @"
                UPDATE Planning_JobCards SET
                    ProductionStatus = @ProductionStatus,
                    ActualStartTime  = @ActualStartTime,
                    ActualEndTime    = @ActualEndTime,
                    CompletedQty     = @CompletedQty,
                    RejectedQty      = @RejectedQty,
                    UpdatedAt        = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ProductionStatus", productionStatus);
            command.Parameters.AddWithValue("@ActualStartTime", (object?)actualStartTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ActualEndTime", (object?)actualEndTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompletedQty", completedQty);
            command.Parameters.AddWithValue("@RejectedQty", rejectedQty);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetReadyForAssemblyAsync(int id, bool ready)
        {
            const string query = @"
                UPDATE Planning_JobCards SET ReadyForAssembly = @Ready, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Ready", ready);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<JobCard>> GetByProductionStatusAsync(int orderId, string productionStatus)
        {
            const string query = @"
                SELECT Id, JobCardNo, CreationType, OrderId, OrderNo, OrderItemId, ItemSequence,
                       DrawingId, DrawingNumber, DrawingRevision, DrawingName, DrawingSelectionType,
                       ChildPartId, ChildPartName, ChildPartTemplateId,
                       ProcessId, ProcessName, ProcessCode, StepNo, ProcessTemplateId,
                       WorkInstructions, QualityCheckpoints, SpecialNotes,
                       Quantity, Status, Priority, ManufacturingDimensions,
                       ProductionStatus, ActualStartTime, ActualEndTime,
                       CompletedQty, RejectedQty, ReadyForAssembly,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Version
                FROM Planning_JobCards
                WHERE OrderId = @OrderId AND ProductionStatus = @ProductionStatus
                ORDER BY StepNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ProductionStatus", productionStatus);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var jobCards = new List<JobCard>();
            while (await reader.ReadAsync())
                jobCards.Add(MapToJobCard(reader));

            return jobCards;
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

        private static JobCard MapToJobCard(SqlDataReader reader)
        {
            var jc = new JobCard
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                JobCardNo = reader.GetString(reader.GetOrdinal("JobCardNo")),
                CreationType = reader.GetString(reader.GetOrdinal("CreationType")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                OrderItemId = reader.IsDBNull(reader.GetOrdinal("OrderItemId")) ? null : reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                ItemSequence = reader.IsDBNull(reader.GetOrdinal("ItemSequence")) ? null : reader.GetString(reader.GetOrdinal("ItemSequence")),
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
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                ManufacturingDimensions = reader.IsDBNull(reader.GetOrdinal("ManufacturingDimensions")) ? null : reader.GetString(reader.GetOrdinal("ManufacturingDimensions")),
                ProductionStatus = reader.GetString(reader.GetOrdinal("ProductionStatus")),
                ActualStartTime = reader.IsDBNull(reader.GetOrdinal("ActualStartTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualStartTime")),
                ActualEndTime = reader.IsDBNull(reader.GetOrdinal("ActualEndTime")) ? null : reader.GetDateTime(reader.GetOrdinal("ActualEndTime")),
                CompletedQty = reader.GetInt32(reader.GetOrdinal("CompletedQty")),
                RejectedQty = reader.GetInt32(reader.GetOrdinal("RejectedQty")),
                ReadyForAssembly = reader.GetBoolean(reader.GetOrdinal("ReadyForAssembly")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                Version = reader.GetInt32(reader.GetOrdinal("Version"))
            };
            // MachineModelName is only present in queries that JOIN Orders_OrderItems + Masters_Products
            try
            {
                var ord = reader.GetOrdinal("MachineModelName");
                jc.MachineModelName = reader.IsDBNull(ord) ? null : reader.GetString(ord);
            }
            catch { }
            return jc;
        }

        private static void AddJobCardParameters(SqlCommand command, JobCard jobCard)
        {
            command.Parameters.AddWithValue("@Id", jobCard.Id);
            command.Parameters.AddWithValue("@JobCardNo", jobCard.JobCardNo);
            command.Parameters.AddWithValue("@CreationType", jobCard.CreationType);
            command.Parameters.AddWithValue("@OrderId", jobCard.OrderId);
            command.Parameters.AddWithValue("@OrderNo", (object?)jobCard.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderItemId", (object?)jobCard.OrderItemId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemSequence", (object?)jobCard.ItemSequence ?? DBNull.Value);
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
            command.Parameters.AddWithValue("@Status", jobCard.Status);
            command.Parameters.AddWithValue("@Priority", jobCard.Priority);
            command.Parameters.AddWithValue("@ManufacturingDimensions", (object?)jobCard.ManufacturingDimensions ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", jobCard.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)jobCard.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)jobCard.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)jobCard.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Version", jobCard.Version);
        }
    }
}
