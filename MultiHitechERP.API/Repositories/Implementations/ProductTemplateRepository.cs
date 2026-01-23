using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductTemplateRepository : IProductTemplateRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductTemplateRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ProductTemplate?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProductTemplate(reader) : null;
        }

        public async Task<ProductTemplate?> GetByNameAsync(string templateName)
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProductTemplate(reader) : null;
        }

        public async Task<IEnumerable<ProductTemplate>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_ProductTemplates ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetActiveTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE IsActive = 1 ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<int> InsertAsync(ProductTemplate template)
        {
            const string query = @"
                INSERT INTO Masters_ProductTemplates (
                    TemplateName, ProductType, Category, RollerType, Description,
                    ProcessTemplateId, ProcessTemplateName, EstimatedLeadTimeDays, StandardCost,
                    IsActive, Status, IsDefault, ApprovedBy, ApprovalDate,
                    Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @TemplateName, @ProductType, @Category, @RollerType, @Description,
                    @ProcessTemplateId, @ProcessTemplateName, @EstimatedLeadTimeDays, @StandardCost,
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

        public async Task<bool> UpdateAsync(ProductTemplate template)
        {
            const string query = @"
                UPDATE Masters_ProductTemplates SET
                    TemplateName = @TemplateName, ProductType = @ProductType, Category = @Category,
                    RollerType = @RollerType, Description = @Description,
                    ProcessTemplateId = @ProcessTemplateId, ProcessTemplateName = @ProcessTemplateName,
                    EstimatedLeadTimeDays = @EstimatedLeadTimeDays, StandardCost = @StandardCost,
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
            const string query = "DELETE FROM Masters_ProductTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<ProductTemplate>> GetByProductTypeAsync(string productType)
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE ProductType = @ProductType ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductType", productType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetByCategoryAsync(string category)
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE Category = @Category ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE ProcessTemplateId = @ProcessTemplateId ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessTemplateId", processTemplateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetDefaultTemplatesAsync()
        {
            const string query = "SELECT * FROM Masters_ProductTemplates WHERE IsDefault = 1 ORDER BY TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                templates.Add(MapToProductTemplate(reader));

            return templates;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_ProductTemplates WHERE TemplateName = @TemplateName";

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
                UPDATE Masters_ProductTemplates
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

        private static ProductTemplate MapToProductTemplate(SqlDataReader reader)
        {
            return new ProductTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                ProductType = reader.IsDBNull(reader.GetOrdinal("ProductType")) ? null : reader.GetString(reader.GetOrdinal("ProductType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                RollerType = reader.IsDBNull(reader.GetOrdinal("RollerType")) ? null : reader.GetString(reader.GetOrdinal("RollerType")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                ProcessTemplateId = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                ProcessTemplateName = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateName")) ? null : reader.GetString(reader.GetOrdinal("ProcessTemplateName")),
                EstimatedLeadTimeDays = reader.IsDBNull(reader.GetOrdinal("EstimatedLeadTimeDays")) ? null : reader.GetInt32(reader.GetOrdinal("EstimatedLeadTimeDays")),
                StandardCost = reader.IsDBNull(reader.GetOrdinal("StandardCost")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardCost")),
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

        private static void AddTemplateParameters(SqlCommand command, ProductTemplate template)
        {
            if (template.Id > 0)
            {
                command.Parameters.AddWithValue("@Id", template.Id);
            }
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@ProductType", (object?)template.ProductType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)template.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@RollerType", (object?)template.RollerType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)template.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateName", (object?)template.ProcessTemplateName ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedLeadTimeDays", (object?)template.EstimatedLeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCost", (object?)template.StandardCost ?? DBNull.Value);
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
