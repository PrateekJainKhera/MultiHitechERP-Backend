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

        #region Template CRUD Operations

        public async Task<ProductTemplate?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToProductTemplate(reader);
            reader.Close();

            // Load child parts
            template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(id);

            return template;
        }

        public async Task<ProductTemplate?> GetByCodeAsync(string templateCode)
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.TemplateCode = @TemplateCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateCode", templateCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToProductTemplate(reader);
            reader.Close();

            // Load child parts
            template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);

            return template;
        }

        public async Task<ProductTemplate?> GetByNameAsync(string templateName)
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToProductTemplate(reader);
            reader.Close();

            // Load child parts
            template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);

            return template;
        }

        public async Task<IEnumerable<ProductTemplate>> GetAllAsync()
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                ORDER BY pt.TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToProductTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load child parts for each template
            foreach (var template in templates)
            {
                template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);
            }

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetActiveTemplatesAsync()
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.IsActive = 1
                ORDER BY pt.TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToProductTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load child parts for each template
            foreach (var template in templates)
            {
                template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);
            }

            return templates;
        }

        public async Task<int> InsertAsync(ProductTemplate template)
        {
            const string query = @"
                INSERT INTO Masters_ProductTemplates (
                    TemplateCode, TemplateName, Description, RollerType,
                    ProcessTemplateId, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                ) VALUES (
                    @TemplateCode, @TemplateName, @Description, @RollerType,
                    @ProcessTemplateId, @IsActive, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@RollerType", template.RollerType);
            command.Parameters.AddWithValue("@ProcessTemplateId", template.ProcessTemplateId);
            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", template.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)template.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", template.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)template.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var templateId = (int)await command.ExecuteScalarAsync();
            template.Id = templateId;

            return templateId;
        }

        public async Task<bool> UpdateAsync(ProductTemplate template)
        {
            const string query = @"
                UPDATE Masters_ProductTemplates SET
                    TemplateCode = @TemplateCode,
                    TemplateName = @TemplateName,
                    Description = @Description,
                    RollerType = @RollerType,
                    ProcessTemplateId = @ProcessTemplateId,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@Id", template.Id);
            command.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@RollerType", template.RollerType);
            command.Parameters.AddWithValue("@ProcessTemplateId", template.ProcessTemplateId);
            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", template.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)template.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Child parts will be deleted automatically due to CASCADE DELETE
            const string query = "DELETE FROM Masters_ProductTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        #endregion

        #region Queries

        public async Task<IEnumerable<ProductTemplate>> GetByRollerTypeAsync(string rollerType)
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.RollerType = @RollerType
                ORDER BY pt.TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollerType", rollerType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToProductTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load child parts for each template
            foreach (var template in templates)
            {
                template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);
            }

            return templates;
        }

        public async Task<IEnumerable<ProductTemplate>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            const string query = @"
                SELECT pt.*, prt.TemplateName as ProcessTemplateName
                FROM Masters_ProductTemplates pt
                LEFT JOIN Masters_ProcessTemplates prt ON pt.ProcessTemplateId = prt.Id
                WHERE pt.ProcessTemplateId = @ProcessTemplateId
                ORDER BY pt.TemplateName";

            var templates = new List<ProductTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessTemplateId", processTemplateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToProductTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load child parts for each template
            foreach (var template in templates)
            {
                template.ChildParts = (List<ProductTemplateChildPart>)await GetChildPartsByTemplateIdAsync(template.Id);
            }

            return templates;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_ProductTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync() > 0;
        }

        #endregion

        #region Child Parts Operations

        public async Task<IEnumerable<ProductTemplateChildPart>> GetChildPartsByTemplateIdAsync(int templateId)
        {
            const string query = @"
                SELECT * FROM Masters_ProductTemplateChildParts
                WHERE ProductTemplateId = @ProductTemplateId
                ORDER BY SequenceNo";

            var childParts = new List<ProductTemplateChildPart>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductTemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToProductTemplateChildPart(reader));
            }

            return childParts;
        }

        public async Task<bool> DeleteChildPartsByTemplateIdAsync(int templateId)
        {
            const string query = "DELETE FROM Masters_ProductTemplateChildParts WHERE ProductTemplateId = @ProductTemplateId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductTemplateId", templateId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> InsertChildPartsAsync(int templateId, IEnumerable<ProductTemplateChildPart> childParts)
        {
            if (childParts == null) return true;

            const string query = @"
                INSERT INTO Masters_ProductTemplateChildParts (
                    ProductTemplateId, ChildPartName, ChildPartCode, Quantity,
                    Unit, Notes, SequenceNo, ChildPartTemplateId
                ) VALUES (
                    @ProductTemplateId, @ChildPartName, @ChildPartCode, @Quantity,
                    @Unit, @Notes, @SequenceNo, @ChildPartTemplateId
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            foreach (var childPart in childParts)
            {
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductTemplateId", templateId);
                command.Parameters.AddWithValue("@ChildPartName", childPart.ChildPartName);
                command.Parameters.AddWithValue("@ChildPartCode", (object?)childPart.ChildPartCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", childPart.Quantity);
                command.Parameters.AddWithValue("@Unit", childPart.Unit);
                command.Parameters.AddWithValue("@Notes", (object?)childPart.Notes ?? DBNull.Value);
                command.Parameters.AddWithValue("@SequenceNo", childPart.SequenceNo);
                command.Parameters.AddWithValue("@ChildPartTemplateId", (object?)childPart.ChildPartTemplateId ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }

            return true;
        }

        public async Task<int> GetNextSequenceNumberAsync(string rollerType)
        {
            // TemplateCode format: ROLLERTYPE-SEQ
            // Example: MAG-0001, PRT-0001
            string prefix = rollerType.Length >= 3 ? rollerType.Substring(0, 3).ToUpper() : rollerType.ToUpper();

            const string query = @"
                SELECT ISNULL(MAX(CAST(RIGHT(TemplateCode, 4) AS INT)), 0) + 1
                FROM Masters_ProductTemplates
                WHERE TemplateCode LIKE @Prefix + '%'";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Prefix", prefix);

            await connection.OpenAsync();
            var nextSequence = (int)(await command.ExecuteScalarAsync() ?? 1);

            return nextSequence;
        }

        #endregion

        #region Mapping Methods

        private ProductTemplate MapToProductTemplate(SqlDataReader reader)
        {
            return new ProductTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateCode = reader.GetString(reader.GetOrdinal("TemplateCode")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                RollerType = reader.GetString(reader.GetOrdinal("RollerType")),
                ProcessTemplateId = reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                ProcessTemplateName = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ProcessTemplateName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                ChildParts = new List<ProductTemplateChildPart>()
            };
        }

        private ProductTemplateChildPart MapToProductTemplateChildPart(SqlDataReader reader)
        {
            return new ProductTemplateChildPart
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductTemplateId = reader.GetInt32(reader.GetOrdinal("ProductTemplateId")),
                ChildPartName = reader.GetString(reader.GetOrdinal("ChildPartName")),
                ChildPartCode = reader.IsDBNull(reader.GetOrdinal("ChildPartCode")) ? null : reader.GetString(reader.GetOrdinal("ChildPartCode")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                SequenceNo = reader.GetInt32(reader.GetOrdinal("SequenceNo")),
                ChildPartTemplateId = reader.IsDBNull(reader.GetOrdinal("ChildPartTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId"))
            };
        }

        #endregion
    }
}
