using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ChildPartTemplateRepository : IChildPartTemplateRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ChildPartTemplateRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ChildPartTemplate?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToChildPartTemplate(reader) : null;
        }

        public async Task<ChildPartTemplate?> GetByNameAsync(string templateName)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToChildPartTemplate(reader) : null;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetActiveTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE IsActive = 1 ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<int> InsertAsync(ChildPartTemplate template)
        {
            const string query = @"
                INSERT INTO Masters_ChildPartTemplates (
                    TemplateName, ChildPartType, Description, Category,
                    Length, Diameter, InnerDiameter, OuterDiameter, Thickness, Width, Height,
                    MaterialType, MaterialGrade, ProcessTemplateId, ProcessTemplateName,
                    EstimatedCost, EstimatedLeadTimeDays, EstimatedWeight,
                    IsActive, Status, IsDefault, ApprovedBy, ApprovalDate,
                    Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @TemplateName, @ChildPartType, @Description, @Category,
                    @Length, @Diameter, @InnerDiameter, @OuterDiameter, @Thickness, @Width, @Height,
                    @MaterialType, @MaterialGrade, @ProcessTemplateId, @ProcessTemplateName,
                    @EstimatedCost, @EstimatedLeadTimeDays, @EstimatedWeight,
                    @IsActive, @Status, @IsDefault, @ApprovedBy, @ApprovalDate,
                    @Remarks, @CreatedAt, @CreatedBy
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

        public async Task<bool> UpdateAsync(ChildPartTemplate template)
        {
            const string query = @"
                UPDATE Masters_ChildPartTemplates SET
                    TemplateName = @TemplateName, ChildPartType = @ChildPartType,
                    Description = @Description, Category = @Category,
                    Length = @Length, Diameter = @Diameter, InnerDiameter = @InnerDiameter,
                    OuterDiameter = @OuterDiameter, Thickness = @Thickness, Width = @Width, Height = @Height,
                    MaterialType = @MaterialType, MaterialGrade = @MaterialGrade,
                    ProcessTemplateId = @ProcessTemplateId, ProcessTemplateName = @ProcessTemplateName,
                    EstimatedCost = @EstimatedCost, EstimatedLeadTimeDays = @EstimatedLeadTimeDays,
                    EstimatedWeight = @EstimatedWeight,
                    IsActive = @IsActive, Status = @Status, IsDefault = @IsDefault,
                    ApprovedBy = @ApprovedBy, ApprovalDate = @ApprovalDate,
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
            const string query = "DELETE FROM Masters_ChildPartTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetByChildPartTypeAsync(string childPartType)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE ChildPartType = @ChildPartType ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartType", childPartType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetByCategoryAsync(string category)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE Category = @Category ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE ProcessTemplateId = @ProcessTemplateId ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessTemplateId", processTemplateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetDefaultTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE IsDefault = 1 ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToChildPartTemplate(reader));

            return templates;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_ChildPartTemplates WHERE TemplateName = @TemplateName";

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
                UPDATE Masters_ChildPartTemplates
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

        private static ChildPartTemplate MapToChildPartTemplate(SqlDataReader reader)
        {
            return new ChildPartTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                ChildPartType = reader.GetString(reader.GetOrdinal("ChildPartType")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Diameter = reader.IsDBNull(reader.GetOrdinal("Diameter")) ? null : reader.GetDecimal(reader.GetOrdinal("Diameter")),
                InnerDiameter = reader.IsDBNull(reader.GetOrdinal("InnerDiameter")) ? null : reader.GetDecimal(reader.GetOrdinal("InnerDiameter")),
                OuterDiameter = reader.IsDBNull(reader.GetOrdinal("OuterDiameter")) ? null : reader.GetDecimal(reader.GetOrdinal("OuterDiameter")),
                Thickness = reader.IsDBNull(reader.GetOrdinal("Thickness")) ? null : reader.GetDecimal(reader.GetOrdinal("Thickness")),
                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? null : reader.GetDecimal(reader.GetOrdinal("Width")),
                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? null : reader.GetDecimal(reader.GetOrdinal("Height")),
                MaterialType = reader.IsDBNull(reader.GetOrdinal("MaterialType")) ? null : reader.GetString(reader.GetOrdinal("MaterialType")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade")) ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                ProcessTemplateId = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                ProcessTemplateName = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateName")) ? null : reader.GetString(reader.GetOrdinal("ProcessTemplateName")),
                EstimatedCost = reader.IsDBNull(reader.GetOrdinal("EstimatedCost")) ? null : reader.GetDecimal(reader.GetOrdinal("EstimatedCost")),
                EstimatedLeadTimeDays = reader.IsDBNull(reader.GetOrdinal("EstimatedLeadTimeDays")) ? null : reader.GetInt32(reader.GetOrdinal("EstimatedLeadTimeDays")),
                EstimatedWeight = reader.IsDBNull(reader.GetOrdinal("EstimatedWeight")) ? null : reader.GetDecimal(reader.GetOrdinal("EstimatedWeight")),
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

        private static void AddTemplateParameters(SqlCommand command, ChildPartTemplate template)
        {
            if (template.Id > 0)
            {
                command.Parameters.AddWithValue("@Id", template.Id);
            }
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@ChildPartType", template.ChildPartType);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)template.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)template.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)template.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@InnerDiameter", (object?)template.InnerDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@OuterDiameter", (object?)template.OuterDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Thickness", (object?)template.Thickness ?? DBNull.Value);
            command.Parameters.AddWithValue("@Width", (object?)template.Width ?? DBNull.Value);
            command.Parameters.AddWithValue("@Height", (object?)template.Height ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialType", (object?)template.MaterialType ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)template.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)template.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateName", (object?)template.ProcessTemplateName ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedCost", (object?)template.EstimatedCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedLeadTimeDays", (object?)template.EstimatedLeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedWeight", (object?)template.EstimatedWeight ?? DBNull.Value);
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
    }
}
