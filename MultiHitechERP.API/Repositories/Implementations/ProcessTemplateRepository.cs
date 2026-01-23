using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProcessTemplateRepository : IProcessTemplateRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProcessTemplateRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #region Template CRUD Operations

        public async Task<ProcessTemplate?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcessTemplate(reader) : null;
        }

        public async Task<ProcessTemplate?> GetByNameAsync(string templateName)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcessTemplate(reader) : null;
        }

        public async Task<IEnumerable<ProcessTemplate>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProcessTemplate>> GetActiveTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE IsActive = 1 ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<int> InsertAsync(ProcessTemplate template)
        {
            const string query = @"
                INSERT INTO Masters_ProcessTemplates (
                    TemplateName, Description, ProductId, ProductCode, ProductName,
                    ChildPartId, ChildPartName, TemplateType, IsActive, Status,
                    IsDefault, ApprovedBy, ApprovalDate, Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @TemplateName, @Description, @ProductId, @ProductCode, @ProductName,
                    @ChildPartId, @ChildPartName, @TemplateType, @IsActive, @Status,
                    @IsDefault, @ApprovedBy, @ApprovalDate, @Remarks, @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.CreatedAt = DateTime.UtcNow;
            AddTemplateParameters(command, template);

            await connection.OpenAsync();
            var templateId = (int)await command.ExecuteScalarAsync();
            template.Id = templateId;

            return templateId;
        }

        public async Task<bool> UpdateAsync(ProcessTemplate template)
        {
            const string query = @"
                UPDATE Masters_ProcessTemplates SET
                    TemplateName = @TemplateName, Description = @Description,
                    ProductId = @ProductId, ProductCode = @ProductCode, ProductName = @ProductName,
                    ChildPartId = @ChildPartId, ChildPartName = @ChildPartName,
                    TemplateType = @TemplateType, IsActive = @IsActive, Status = @Status,
                    IsDefault = @IsDefault, ApprovedBy = @ApprovedBy, ApprovalDate = @ApprovalDate,
                    Remarks = @Remarks, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.UpdatedAt = DateTime.UtcNow;
            AddTemplateParameters(command, template);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Delete template steps first (if not using CASCADE)
            await DeleteAllStepsAsync(id);

            const string query = "DELETE FROM Masters_ProcessTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        #endregion

        #region Template Steps Operations

        public async Task<IEnumerable<ProcessTemplateStep>> GetStepsByTemplateIdAsync(int templateId)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplateSteps WHERE TemplateId = @TemplateId ORDER BY StepNo";

            var steps = new List<ProcessTemplateStep>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                steps.Add(MapToProcessTemplateStep(reader));

            return steps;
        }

        public async Task<ProcessTemplateStep?> GetStepByIdAsync(int stepId)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplateSteps WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", stepId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcessTemplateStep(reader) : null;
        }

        public async Task<int> InsertStepAsync(ProcessTemplateStep step)
        {
            const string query = @"
                INSERT INTO Masters_ProcessTemplateSteps (
                    TemplateId, StepNo, ProcessId, ProcessCode, ProcessName,
                    DefaultMachineId, DefaultMachineName, MachineType,
                    SetupTimeMin, CycleTimeMin, CycleTimePerPiece,
                    IsParallel, ParallelGroupNo, DependsOnSteps,
                    RequiresQC, QCCheckpoints, WorkInstructions, SafetyInstructions,
                    ToolingRequired, Remarks, CreatedAt
                ) VALUES (
                    @TemplateId, @StepNo, @ProcessId, @ProcessCode, @ProcessName,
                    @DefaultMachineId, @DefaultMachineName, @MachineType,
                    @SetupTimeMin, @CycleTimeMin, @CycleTimePerPiece,
                    @IsParallel, @ParallelGroupNo, @DependsOnSteps,
                    @RequiresQC, @QCCheckpoints, @WorkInstructions, @SafetyInstructions,
                    @ToolingRequired, @Remarks, @CreatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            step.CreatedAt = DateTime.UtcNow;
            AddStepParameters(command, step);

            await connection.OpenAsync();
            var stepId = (int)await command.ExecuteScalarAsync();
            step.Id = stepId;

            return stepId;
        }

        public async Task<bool> UpdateStepAsync(ProcessTemplateStep step)
        {
            const string query = @"
                UPDATE Masters_ProcessTemplateSteps SET
                    TemplateId = @TemplateId, StepNo = @StepNo,
                    ProcessId = @ProcessId, ProcessCode = @ProcessCode, ProcessName = @ProcessName,
                    DefaultMachineId = @DefaultMachineId, DefaultMachineName = @DefaultMachineName, MachineType = @MachineType,
                    SetupTimeMin = @SetupTimeMin, CycleTimeMin = @CycleTimeMin, CycleTimePerPiece = @CycleTimePerPiece,
                    IsParallel = @IsParallel, ParallelGroupNo = @ParallelGroupNo, DependsOnSteps = @DependsOnSteps,
                    RequiresQC = @RequiresQC, QCCheckpoints = @QCCheckpoints,
                    WorkInstructions = @WorkInstructions, SafetyInstructions = @SafetyInstructions,
                    ToolingRequired = @ToolingRequired, Remarks = @Remarks
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddStepParameters(command, step);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteStepAsync(int stepId)
        {
            const string query = "DELETE FROM Masters_ProcessTemplateSteps WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", stepId);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAllStepsAsync(int templateId)
        {
            const string query = "DELETE FROM Masters_ProcessTemplateSteps WHERE TemplateId = @TemplateId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateId", templateId);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() >= 0; // Can be 0 if no steps exist
        }

        #endregion

        #region Queries

        public async Task<IEnumerable<ProcessTemplate>> GetByProductIdAsync(int productId)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE ProductId = @ProductId ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProcessTemplate>> GetByChildPartIdAsync(int childPartId)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE ChildPartId = @ChildPartId ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartId", childPartId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProcessTemplate>> GetByTemplateTypeAsync(string templateType)
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE TemplateType = @TemplateType ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateType", templateType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProcessTemplate>> GetDefaultTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ProcessTemplates WHERE IsDefault = 1 ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProcessTemplate(reader));

            return templates;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_ProcessTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<bool> ApproveTemplateAsync(int id, string approvedBy)
        {
            const string query = @"
                UPDATE Masters_ProcessTemplates
                SET ApprovedBy = @ApprovedBy, ApprovalDate = @ApprovalDate, Status = 'Approved', UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ApprovedBy", approvedBy);
            command.Parameters.AddWithValue("@ApprovalDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        #endregion

        #region Helper Methods

        private static ProcessTemplate MapToProcessTemplate(SqlDataReader reader)
        {
            return new ProcessTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                ProductId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? null : reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                ChildPartId = reader.IsDBNull(reader.GetOrdinal("ChildPartId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartId")),
                ChildPartName = reader.IsDBNull(reader.GetOrdinal("ChildPartName")) ? null : reader.GetString(reader.GetOrdinal("ChildPartName")),
                TemplateType = reader.IsDBNull(reader.GetOrdinal("TemplateType")) ? null : reader.GetString(reader.GetOrdinal("TemplateType")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                IsDefault = reader.GetBoolean(reader.GetOrdinal("IsDefault")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovalDate = reader.IsDBNull(reader.GetOrdinal("ApprovalDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovalDate")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static ProcessTemplateStep MapToProcessTemplateStep(SqlDataReader reader)
        {
            return new ProcessTemplateStep
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateId = reader.GetInt32(reader.GetOrdinal("TemplateId")),
                StepNo = reader.GetInt32(reader.GetOrdinal("StepNo")),
                ProcessId = reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessCode = reader.IsDBNull(reader.GetOrdinal("ProcessCode")) ? null : reader.GetString(reader.GetOrdinal("ProcessCode")),
                ProcessName = reader.IsDBNull(reader.GetOrdinal("ProcessName")) ? null : reader.GetString(reader.GetOrdinal("ProcessName")),
                DefaultMachineId = reader.IsDBNull(reader.GetOrdinal("DefaultMachineId")) ? null : reader.GetInt32(reader.GetOrdinal("DefaultMachineId")),
                DefaultMachineName = reader.IsDBNull(reader.GetOrdinal("DefaultMachineName")) ? null : reader.GetString(reader.GetOrdinal("DefaultMachineName")),
                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? null : reader.GetString(reader.GetOrdinal("MachineType")),
                SetupTimeMin = reader.IsDBNull(reader.GetOrdinal("SetupTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("SetupTimeMin")),
                CycleTimeMin = reader.IsDBNull(reader.GetOrdinal("CycleTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("CycleTimeMin")),
                CycleTimePerPiece = reader.IsDBNull(reader.GetOrdinal("CycleTimePerPiece")) ? null : reader.GetDecimal(reader.GetOrdinal("CycleTimePerPiece")),
                IsParallel = reader.GetBoolean(reader.GetOrdinal("IsParallel")),
                ParallelGroupNo = reader.IsDBNull(reader.GetOrdinal("ParallelGroupNo")) ? null : reader.GetInt32(reader.GetOrdinal("ParallelGroupNo")),
                DependsOnSteps = reader.IsDBNull(reader.GetOrdinal("DependsOnSteps")) ? null : reader.GetString(reader.GetOrdinal("DependsOnSteps")),
                RequiresQC = reader.GetBoolean(reader.GetOrdinal("RequiresQC")),
                QCCheckpoints = reader.IsDBNull(reader.GetOrdinal("QCCheckpoints")) ? null : reader.GetString(reader.GetOrdinal("QCCheckpoints")),
                WorkInstructions = reader.IsDBNull(reader.GetOrdinal("WorkInstructions")) ? null : reader.GetString(reader.GetOrdinal("WorkInstructions")),
                SafetyInstructions = reader.IsDBNull(reader.GetOrdinal("SafetyInstructions")) ? null : reader.GetString(reader.GetOrdinal("SafetyInstructions")),
                ToolingRequired = reader.IsDBNull(reader.GetOrdinal("ToolingRequired")) ? null : reader.GetString(reader.GetOrdinal("ToolingRequired")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        private static void AddTemplateParameters(SqlCommand command, ProcessTemplate template)
        {
            if (template.Id > 0)
            {
                command.Parameters.AddWithValue("@Id", template.Id);
            }
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", (object?)template.ProductId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCode", (object?)template.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)template.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartId", (object?)template.ChildPartId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartName", (object?)template.ChildPartName ?? DBNull.Value);
            command.Parameters.AddWithValue("@TemplateType", (object?)template.TemplateType ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)template.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsDefault", template.IsDefault);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)template.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalDate", (object?)template.ApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)template.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", template.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)template.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)template.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)template.UpdatedBy ?? DBNull.Value);
        }

        private static void AddStepParameters(SqlCommand command, ProcessTemplateStep step)
        {
            if (step.Id > 0)
            {
                command.Parameters.AddWithValue("@Id", step.Id);
            }
            command.Parameters.AddWithValue("@TemplateId", step.TemplateId);
            command.Parameters.AddWithValue("@StepNo", step.StepNo);
            command.Parameters.AddWithValue("@ProcessId", step.ProcessId);
            command.Parameters.AddWithValue("@ProcessCode", (object?)step.ProcessCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessName", (object?)step.ProcessName ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachineId", (object?)step.DefaultMachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachineName", (object?)step.DefaultMachineName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineType", (object?)step.MachineType ?? DBNull.Value);
            command.Parameters.AddWithValue("@SetupTimeMin", (object?)step.SetupTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@CycleTimeMin", (object?)step.CycleTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@CycleTimePerPiece", (object?)step.CycleTimePerPiece ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsParallel", step.IsParallel);
            command.Parameters.AddWithValue("@ParallelGroupNo", (object?)step.ParallelGroupNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@DependsOnSteps", (object?)step.DependsOnSteps ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequiresQC", step.RequiresQC);
            command.Parameters.AddWithValue("@QCCheckpoints", (object?)step.QCCheckpoints ?? DBNull.Value);
            command.Parameters.AddWithValue("@WorkInstructions", (object?)step.WorkInstructions ?? DBNull.Value);
            command.Parameters.AddWithValue("@SafetyInstructions", (object?)step.SafetyInstructions ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToolingRequired", (object?)step.ToolingRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)step.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", step.CreatedAt);
        }

        #endregion
    }
}
