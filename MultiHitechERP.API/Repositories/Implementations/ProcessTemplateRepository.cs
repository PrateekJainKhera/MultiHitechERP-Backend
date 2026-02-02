using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
                    TemplateName, Description, ApplicableTypes, IsActive, CreatedAt, CreatedBy, UpdatedAt
                ) VALUES (
                    @TemplateName, @Description, @ApplicableTypes, @IsActive, @CreatedAt, @CreatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;
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
                    ApplicableTypes = @ApplicableTypes, IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
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
            const string query = @"
                SELECT
                    pts.Id,
                    pts.TemplateId,
                    pts.StepNo,
                    pts.ProcessId,
                    p.ProcessName,
                    pts.IsMandatory,
                    pts.CanBeParallel
                FROM Masters_ProcessTemplateSteps pts
                LEFT JOIN Masters_Processes p ON pts.ProcessId = p.Id
                WHERE pts.TemplateId = @TemplateId
                ORDER BY pts.StepNo";

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
            const string query = @"
                SELECT
                    pts.Id,
                    pts.TemplateId,
                    pts.StepNo,
                    pts.ProcessId,
                    p.ProcessName,
                    pts.IsMandatory,
                    pts.CanBeParallel
                FROM Masters_ProcessTemplateSteps pts
                LEFT JOIN Masters_Processes p ON pts.ProcessId = p.Id
                WHERE pts.Id = @Id";

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
                    TemplateId, StepNo, ProcessId, ProcessName, IsMandatory, CanBeParallel
                ) VALUES (
                    @TemplateId, @StepNo, @ProcessId, @ProcessName, @IsMandatory, @CanBeParallel
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

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
                    ProcessId = @ProcessId, ProcessName = @ProcessName,
                    IsMandatory = @IsMandatory, CanBeParallel = @CanBeParallel
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

        public async Task<IEnumerable<ProcessTemplate>> GetByApplicableTypeAsync(string applicableType)
        {
            const string query = @"
                SELECT * FROM Masters_ProcessTemplates
                WHERE ApplicableTypes LIKE '%' + @ApplicableType + '%'
                ORDER BY TemplateName";

            var templates = new List<ProcessTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ApplicableType", applicableType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToProcessTemplate(reader);
                // Filter to ensure exact match in the JSON array
                if (template.ApplicableTypes != null && template.ApplicableTypes.Contains(applicableType))
                {
                    templates.Add(template);
                }
            }

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

        #endregion

        #region Helper Methods

        private static ProcessTemplate MapToProcessTemplate(SqlDataReader reader)
        {
            var template = new ProcessTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };

            // Deserialize ApplicableTypes from JSON
            var applicableTypesOrdinal = reader.GetOrdinal("ApplicableTypes");
            if (!reader.IsDBNull(applicableTypesOrdinal))
            {
                var applicableTypesJson = reader.GetString(applicableTypesOrdinal);
                if (!string.IsNullOrWhiteSpace(applicableTypesJson))
                {
                    try
                    {
                        template.ApplicableTypes = JsonSerializer.Deserialize<List<string>>(applicableTypesJson);
                    }
                    catch
                    {
                        template.ApplicableTypes = null;
                    }
                }
            }

            return template;
        }

        private static ProcessTemplateStep MapToProcessTemplateStep(SqlDataReader reader)
        {
            return new ProcessTemplateStep
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateId = reader.GetInt32(reader.GetOrdinal("TemplateId")),
                StepNo = reader.GetInt32(reader.GetOrdinal("StepNo")),
                ProcessId = reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessName = reader.IsDBNull(reader.GetOrdinal("ProcessName")) ? null : reader.GetString(reader.GetOrdinal("ProcessName")),
                IsMandatory = reader.GetBoolean(reader.GetOrdinal("IsMandatory")),
                CanBeParallel = reader.GetBoolean(reader.GetOrdinal("CanBeParallel"))
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

            // Serialize ApplicableTypes to JSON
            string? applicableTypesJson = null;
            if (template.ApplicableTypes != null && template.ApplicableTypes.Any())
            {
                applicableTypesJson = JsonSerializer.Serialize(template.ApplicableTypes);
            }
            command.Parameters.AddWithValue("@ApplicableTypes", (object?)applicableTypesJson ?? DBNull.Value);

            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", template.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)template.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", template.UpdatedAt);
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
            command.Parameters.AddWithValue("@ProcessName", (object?)step.ProcessName ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsMandatory", step.IsMandatory);
            command.Parameters.AddWithValue("@CanBeParallel", step.CanBeParallel);
        }

        #endregion
    }
}
