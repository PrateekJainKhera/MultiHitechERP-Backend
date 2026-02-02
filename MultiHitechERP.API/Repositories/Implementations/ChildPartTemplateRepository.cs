using System;
using System.Collections.Generic;
using System.Text.Json;
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

        #region Template CRUD Operations

        public async Task<ChildPartTemplate?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToChildPartTemplate(reader);
            reader.Close();

            // Material requirements and process steps are now handled via ProcessTemplateId reference

            return template;
        }

        public async Task<ChildPartTemplate?> GetByCodeAsync(string templateCode)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE TemplateCode = @TemplateCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateCode", templateCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToChildPartTemplate(reader);
            reader.Close();

            // Material requirements and process steps removed - now using ProcessTemplateId reference

            return template;
        }

        public async Task<ChildPartTemplate?> GetByNameAsync(string templateName)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var template = MapToChildPartTemplate(reader);
            reader.Close();

            // Material requirements and process steps removed - now using ProcessTemplateId reference

            return template;
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
            {
                var template = MapToChildPartTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load related data for each template
            foreach (var template in templates)
            {
                // MaterialRequirements and ProcessSteps removed - now using ProcessTemplateId reference
            }

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
            {
                var template = MapToChildPartTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load related data for each template
            foreach (var template in templates)
            {
                // MaterialRequirements and ProcessSteps removed - now using ProcessTemplateId reference
            }

            return templates;
        }

        public async Task<int> InsertAsync(ChildPartTemplate template)
        {
            const string query = @"
                INSERT INTO Masters_ChildPartTemplates (
                    TemplateCode, TemplateName, ChildPartType, RollerType,
                    ProcessTemplateId, IsPurchased,
                    DrawingNumber, DrawingRevision, Length, Diameter,
                    InnerDiameter, OuterDiameter, Thickness, DimensionUnit,
                    Description, TechnicalNotes,
                    IsActive, CreatedAt, UpdatedAt, CreatedBy
                ) VALUES (
                    @TemplateCode, @TemplateName, @ChildPartType, @RollerType,
                    @ProcessTemplateId, @IsPurchased,
                    @DrawingNumber, @DrawingRevision, @Length, @Diameter,
                    @InnerDiameter, @OuterDiameter, @Thickness, @DimensionUnit,
                    @Description, @TechnicalNotes,
                    @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@ChildPartType", template.ChildPartType);
            command.Parameters.AddWithValue("@RollerType", template.RollerType);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)template.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsPurchased", template.IsPurchased);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)template.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingRevision", (object?)template.DrawingRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)template.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)template.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@InnerDiameter", (object?)template.InnerDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@OuterDiameter", (object?)template.OuterDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Thickness", (object?)template.Thickness ?? DBNull.Value);
            command.Parameters.AddWithValue("@DimensionUnit", template.DimensionUnit);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@TechnicalNotes", (object?)template.TechnicalNotes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", template.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", template.UpdatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)template.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var templateId = (int)await command.ExecuteScalarAsync();
            template.Id = templateId;

            return templateId;
        }

        public async Task<bool> UpdateAsync(ChildPartTemplate template)
        {
            const string query = @"
                UPDATE Masters_ChildPartTemplates SET
                    TemplateCode = @TemplateCode,
                    TemplateName = @TemplateName,
                    ChildPartType = @ChildPartType,
                    RollerType = @RollerType,
                    ProcessTemplateId = @ProcessTemplateId,
                    IsPurchased = @IsPurchased,
                    DrawingNumber = @DrawingNumber,
                    DrawingRevision = @DrawingRevision,
                    Length = @Length,
                    Diameter = @Diameter,
                    InnerDiameter = @InnerDiameter,
                    OuterDiameter = @OuterDiameter,
                    Thickness = @Thickness,
                    DimensionUnit = @DimensionUnit,
                    Description = @Description,
                    TechnicalNotes = @TechnicalNotes,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            template.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@Id", template.Id);
            command.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
            command.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            command.Parameters.AddWithValue("@ChildPartType", template.ChildPartType);
            command.Parameters.AddWithValue("@RollerType", template.RollerType);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)template.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsPurchased", template.IsPurchased);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)template.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingRevision", (object?)template.DrawingRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)template.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)template.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@InnerDiameter", (object?)template.InnerDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@OuterDiameter", (object?)template.OuterDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Thickness", (object?)template.Thickness ?? DBNull.Value);
            command.Parameters.AddWithValue("@DimensionUnit", template.DimensionUnit);
            command.Parameters.AddWithValue("@Description", (object?)template.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@TechnicalNotes", (object?)template.TechnicalNotes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", template.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", template.UpdatedAt);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Material requirements and process steps will be deleted automatically due to CASCADE DELETE
            const string query = "DELETE FROM Masters_ChildPartTemplates WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        #endregion

        #region Queries

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
            {
                var template = MapToChildPartTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load related data for each template
            foreach (var template in templates)
            {
                // MaterialRequirements and ProcessSteps removed - now using ProcessTemplateId reference
            }

            return templates;
        }

        public async Task<IEnumerable<ChildPartTemplate>> GetByRollerTypeAsync(string rollerType)
        {
            const string query = "SELECT * FROM Masters_ChildPartTemplates WHERE RollerType = @RollerType ORDER BY TemplateName";

            var templates = new List<ChildPartTemplate>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollerType", rollerType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var template = MapToChildPartTemplate(reader);
                templates.Add(template);
            }

            reader.Close();

            // Load related data for each template
            foreach (var template in templates)
            {
                // MaterialRequirements and ProcessSteps removed - now using ProcessTemplateId reference
            }

            return templates;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_ChildPartTemplates WHERE TemplateName = @TemplateName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TemplateName", templateName);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync() > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync()
        {
            const string query = "SELECT ISNULL(MAX(Id), 0) + 1 FROM Masters_ChildPartTemplates";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        #endregion

        #region Material Requirements Operations

        public async Task<IEnumerable<ChildPartTemplateMaterialRequirement>> GetMaterialRequirementsByTemplateIdAsync(int templateId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildPartTemplateMaterialRequirements
                WHERE ChildPartTemplateId = @ChildPartTemplateId
                ORDER BY Id";

            var requirements = new List<ChildPartTemplateMaterialRequirement>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                requirements.Add(MapToMaterialRequirement(reader));
            }

            return requirements;
        }

        public async Task<bool> DeleteMaterialRequirementsByTemplateIdAsync(int templateId)
        {
            const string query = "DELETE FROM Masters_ChildPartTemplateMaterialRequirements WHERE ChildPartTemplateId = @ChildPartTemplateId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> InsertMaterialRequirementsAsync(int templateId, IEnumerable<ChildPartTemplateMaterialRequirement> requirements)
        {
            if (requirements == null) return true;

            const string query = @"
                INSERT INTO Masters_ChildPartTemplateMaterialRequirements (
                    ChildPartTemplateId, RawMaterialId, RawMaterialName,
                    MaterialGrade, QuantityRequired, Unit, WastagePercent
                ) VALUES (
                    @ChildPartTemplateId, @RawMaterialId, @RawMaterialName,
                    @MaterialGrade, @QuantityRequired, @Unit, @WastagePercent
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            foreach (var requirement in requirements)
            {
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);
                command.Parameters.AddWithValue("@RawMaterialId", (object?)requirement.RawMaterialId ?? DBNull.Value);
                command.Parameters.AddWithValue("@RawMaterialName", requirement.RawMaterialName);
                command.Parameters.AddWithValue("@MaterialGrade", requirement.MaterialGrade);
                command.Parameters.AddWithValue("@QuantityRequired", requirement.QuantityRequired);
                command.Parameters.AddWithValue("@Unit", requirement.Unit);
                command.Parameters.AddWithValue("@WastagePercent", requirement.WastagePercent);

                await command.ExecuteNonQueryAsync();
            }

            return true;
        }

        #endregion

        #region Process Steps Operations

        public async Task<IEnumerable<ChildPartTemplateProcessStep>> GetProcessStepsByTemplateIdAsync(int templateId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildPartTemplateProcessSteps
                WHERE ChildPartTemplateId = @ChildPartTemplateId
                ORDER BY StepNumber";

            var steps = new List<ChildPartTemplateProcessStep>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                steps.Add(MapToProcessStep(reader));
            }

            return steps;
        }

        public async Task<bool> DeleteProcessStepsByTemplateIdAsync(int templateId)
        {
            const string query = "DELETE FROM Masters_ChildPartTemplateProcessSteps WHERE ChildPartTemplateId = @ChildPartTemplateId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<bool> InsertProcessStepsAsync(int templateId, IEnumerable<ChildPartTemplateProcessStep> steps)
        {
            if (steps == null) return true;

            const string query = @"
                INSERT INTO Masters_ChildPartTemplateProcessSteps (
                    ChildPartTemplateId, ProcessId, ProcessName, StepNumber,
                    MachineName, StandardTimeHours, RestTimeHours, Description
                ) VALUES (
                    @ChildPartTemplateId, @ProcessId, @ProcessName, @StepNumber,
                    @MachineName, @StandardTimeHours, @RestTimeHours, @Description
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            foreach (var step in steps)
            {
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ChildPartTemplateId", templateId);
                command.Parameters.AddWithValue("@ProcessId", (object?)step.ProcessId ?? DBNull.Value);
                command.Parameters.AddWithValue("@ProcessName", step.ProcessName);
                command.Parameters.AddWithValue("@StepNumber", step.StepNumber);
                command.Parameters.AddWithValue("@MachineName", (object?)step.MachineName ?? DBNull.Value);
                command.Parameters.AddWithValue("@StandardTimeHours", step.StandardTimeHours);
                command.Parameters.AddWithValue("@RestTimeHours", (object?)step.RestTimeHours ?? DBNull.Value);
                command.Parameters.AddWithValue("@Description", (object?)step.Description ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }

            return true;
        }

        #endregion

        #region Mapping Methods

        private ChildPartTemplate MapToChildPartTemplate(SqlDataReader reader)
        {
            return new ChildPartTemplate
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TemplateCode = reader.GetString(reader.GetOrdinal("TemplateCode")),
                TemplateName = reader.GetString(reader.GetOrdinal("TemplateName")),
                ChildPartType = reader.GetString(reader.GetOrdinal("ChildPartType")),
                RollerType = reader.IsDBNull(reader.GetOrdinal("RollerType")) ? string.Empty : reader.GetString(reader.GetOrdinal("RollerType")),
                ProcessTemplateId = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                IsPurchased = reader.GetBoolean(reader.GetOrdinal("IsPurchased")),
                DrawingNumber = reader.IsDBNull(reader.GetOrdinal("DrawingNumber")) ? null : reader.GetString(reader.GetOrdinal("DrawingNumber")),
                DrawingRevision = reader.IsDBNull(reader.GetOrdinal("DrawingRevision")) ? null : reader.GetString(reader.GetOrdinal("DrawingRevision")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Diameter = reader.IsDBNull(reader.GetOrdinal("Diameter")) ? null : reader.GetDecimal(reader.GetOrdinal("Diameter")),
                InnerDiameter = reader.IsDBNull(reader.GetOrdinal("InnerDiameter")) ? null : reader.GetDecimal(reader.GetOrdinal("InnerDiameter")),
                OuterDiameter = reader.IsDBNull(reader.GetOrdinal("OuterDiameter")) ? null : reader.GetDecimal(reader.GetOrdinal("OuterDiameter")),
                Thickness = reader.IsDBNull(reader.GetOrdinal("Thickness")) ? null : reader.GetDecimal(reader.GetOrdinal("Thickness")),
                DimensionUnit = reader.GetString(reader.GetOrdinal("DimensionUnit")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                TechnicalNotes = reader.IsDBNull(reader.GetOrdinal("TechnicalNotes")) ? null : reader.GetString(reader.GetOrdinal("TechnicalNotes")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }

        private ChildPartTemplateMaterialRequirement MapToMaterialRequirement(SqlDataReader reader)
        {
            return new ChildPartTemplateMaterialRequirement
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ChildPartTemplateId = reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId")),
                RawMaterialId = reader.IsDBNull(reader.GetOrdinal("RawMaterialId")) ? null : reader.GetInt32(reader.GetOrdinal("RawMaterialId")),
                RawMaterialName = reader.GetString(reader.GetOrdinal("RawMaterialName")),
                MaterialGrade = reader.GetString(reader.GetOrdinal("MaterialGrade")),
                QuantityRequired = reader.GetDecimal(reader.GetOrdinal("QuantityRequired")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                WastagePercent = reader.GetDecimal(reader.GetOrdinal("WastagePercent"))
            };
        }

        private ChildPartTemplateProcessStep MapToProcessStep(SqlDataReader reader)
        {
            return new ChildPartTemplateProcessStep
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ChildPartTemplateId = reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId")),
                ProcessId = reader.IsDBNull(reader.GetOrdinal("ProcessId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessName = reader.GetString(reader.GetOrdinal("ProcessName")),
                StepNumber = reader.GetInt32(reader.GetOrdinal("StepNumber")),
                MachineName = reader.IsDBNull(reader.GetOrdinal("MachineName")) ? null : reader.GetString(reader.GetOrdinal("MachineName")),
                StandardTimeHours = reader.GetDecimal(reader.GetOrdinal("StandardTimeHours")),
                RestTimeHours = reader.IsDBNull(reader.GetOrdinal("RestTimeHours")) ? null : reader.GetDecimal(reader.GetOrdinal("RestTimeHours")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
            };
        }

        #endregion
    }
}
