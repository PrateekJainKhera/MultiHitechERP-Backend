using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Material?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToMaterial(reader) : null;
        }

        public async Task<Material?> GetByMaterialCodeAsync(string materialCode)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE MaterialCode = @MaterialCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialCode", materialCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToMaterial(reader) : null;
        }

        public async Task<IEnumerable<Material>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Materials ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<IEnumerable<Material>> GetActiveMaterialsAsync()
        {
            const string query = "SELECT * FROM Masters_Materials WHERE IsActive = 1 ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<Guid> InsertAsync(Material material)
        {
            const string query = @"
                INSERT INTO Masters_Materials (
                    Id, MaterialCode, MaterialName, Category, SubCategory, MaterialType,
                    Grade, Specification, Description, HSNCode,
                    StandardLength, Diameter, Thickness, Width,
                    PrimaryUOM, SecondaryUOM, ConversionFactor,
                    WeightPerMeter, WeightPerPiece, Density,
                    StandardCost, LastPurchasePrice, LastPurchaseDate,
                    MinStockLevel, MaxStockLevel, ReorderLevel, ReorderQuantity, LeadTimeDays,
                    PreferredSupplierId, PreferredSupplierName, StorageLocation, StorageConditions,
                    IsActive, Status, Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @Id, @MaterialCode, @MaterialName, @Category, @SubCategory, @MaterialType,
                    @Grade, @Specification, @Description, @HSNCode,
                    @StandardLength, @Diameter, @Thickness, @Width,
                    @PrimaryUOM, @SecondaryUOM, @ConversionFactor,
                    @WeightPerMeter, @WeightPerPiece, @Density,
                    @StandardCost, @LastPurchasePrice, @LastPurchaseDate,
                    @MinStockLevel, @MaxStockLevel, @ReorderLevel, @ReorderQuantity, @LeadTimeDays,
                    @PreferredSupplierId, @PreferredSupplierName, @StorageLocation, @StorageConditions,
                    @IsActive, @Status, @Remarks, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            material.Id = Guid.NewGuid();
            material.CreatedAt = DateTime.UtcNow;
            AddMaterialParameters(command, material);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return material.Id;
        }

        public async Task<bool> UpdateAsync(Material material)
        {
            const string query = @"
                UPDATE Masters_Materials SET
                    MaterialName = @MaterialName, Category = @Category, SubCategory = @SubCategory,
                    MaterialType = @MaterialType, Grade = @Grade, Specification = @Specification,
                    Description = @Description, HSNCode = @HSNCode,
                    StandardLength = @StandardLength, Diameter = @Diameter, Thickness = @Thickness, Width = @Width,
                    PrimaryUOM = @PrimaryUOM, SecondaryUOM = @SecondaryUOM, ConversionFactor = @ConversionFactor,
                    WeightPerMeter = @WeightPerMeter, WeightPerPiece = @WeightPerPiece, Density = @Density,
                    StandardCost = @StandardCost, LastPurchasePrice = @LastPurchasePrice, LastPurchaseDate = @LastPurchaseDate,
                    MinStockLevel = @MinStockLevel, MaxStockLevel = @MaxStockLevel,
                    ReorderLevel = @ReorderLevel, ReorderQuantity = @ReorderQuantity, LeadTimeDays = @LeadTimeDays,
                    PreferredSupplierId = @PreferredSupplierId, PreferredSupplierName = @PreferredSupplierName,
                    StorageLocation = @StorageLocation, StorageConditions = @StorageConditions,
                    IsActive = @IsActive, Status = @Status, Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            material.UpdatedAt = DateTime.UtcNow;
            AddMaterialParameters(command, material);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Materials WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            const string query = "UPDATE Masters_Materials SET IsActive = 1, Status = 'Active', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            const string query = "UPDATE Masters_Materials SET IsActive = 0, Status = 'Inactive', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<Material>> SearchByNameAsync(string name)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE MaterialName LIKE @Name ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", $"%{name}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<IEnumerable<Material>> GetByCategoryAsync(string category)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE Category = @Category ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<IEnumerable<Material>> GetByGradeAsync(string grade)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE Grade = @Grade ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Grade", grade);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<IEnumerable<Material>> GetByMaterialTypeAsync(string materialType)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE MaterialType = @MaterialType ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialType", materialType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<IEnumerable<Material>> GetLowStockMaterialsAsync()
        {
            // Note: This requires inventory data - placeholder for now
            const string query = "SELECT * FROM Masters_Materials WHERE IsActive = 1 ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<bool> ExistsAsync(string materialCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Materials WHERE MaterialCode = @MaterialCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialCode", materialCode);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        private static Material MapToMaterial(SqlDataReader reader)
        {
            return new Material
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                MaterialCode = reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialName = reader.GetString(reader.GetOrdinal("MaterialName")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                SubCategory = reader.IsDBNull(reader.GetOrdinal("SubCategory")) ? null : reader.GetString(reader.GetOrdinal("SubCategory")),
                MaterialType = reader.IsDBNull(reader.GetOrdinal("MaterialType")) ? null : reader.GetString(reader.GetOrdinal("MaterialType")),
                Grade = reader.IsDBNull(reader.GetOrdinal("Grade")) ? null : reader.GetString(reader.GetOrdinal("Grade")),
                Specification = reader.IsDBNull(reader.GetOrdinal("Specification")) ? null : reader.GetString(reader.GetOrdinal("Specification")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                HSNCode = reader.IsDBNull(reader.GetOrdinal("HSNCode")) ? null : reader.GetString(reader.GetOrdinal("HSNCode")),
                StandardLength = reader.IsDBNull(reader.GetOrdinal("StandardLength")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardLength")),
                Diameter = reader.IsDBNull(reader.GetOrdinal("Diameter")) ? null : reader.GetDecimal(reader.GetOrdinal("Diameter")),
                Thickness = reader.IsDBNull(reader.GetOrdinal("Thickness")) ? null : reader.GetDecimal(reader.GetOrdinal("Thickness")),
                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? null : reader.GetDecimal(reader.GetOrdinal("Width")),
                PrimaryUOM = reader.IsDBNull(reader.GetOrdinal("PrimaryUOM")) ? null : reader.GetString(reader.GetOrdinal("PrimaryUOM")),
                SecondaryUOM = reader.IsDBNull(reader.GetOrdinal("SecondaryUOM")) ? null : reader.GetString(reader.GetOrdinal("SecondaryUOM")),
                ConversionFactor = reader.IsDBNull(reader.GetOrdinal("ConversionFactor")) ? null : reader.GetDecimal(reader.GetOrdinal("ConversionFactor")),
                WeightPerMeter = reader.IsDBNull(reader.GetOrdinal("WeightPerMeter")) ? null : reader.GetDecimal(reader.GetOrdinal("WeightPerMeter")),
                WeightPerPiece = reader.IsDBNull(reader.GetOrdinal("WeightPerPiece")) ? null : reader.GetDecimal(reader.GetOrdinal("WeightPerPiece")),
                Density = reader.IsDBNull(reader.GetOrdinal("Density")) ? null : reader.GetDecimal(reader.GetOrdinal("Density")),
                StandardCost = reader.IsDBNull(reader.GetOrdinal("StandardCost")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardCost")),
                LastPurchasePrice = reader.IsDBNull(reader.GetOrdinal("LastPurchasePrice")) ? null : reader.GetDecimal(reader.GetOrdinal("LastPurchasePrice")),
                LastPurchaseDate = reader.IsDBNull(reader.GetOrdinal("LastPurchaseDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastPurchaseDate")),
                MinStockLevel = reader.IsDBNull(reader.GetOrdinal("MinStockLevel")) ? null : reader.GetDecimal(reader.GetOrdinal("MinStockLevel")),
                MaxStockLevel = reader.IsDBNull(reader.GetOrdinal("MaxStockLevel")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxStockLevel")),
                ReorderLevel = reader.IsDBNull(reader.GetOrdinal("ReorderLevel")) ? null : reader.GetDecimal(reader.GetOrdinal("ReorderLevel")),
                ReorderQuantity = reader.IsDBNull(reader.GetOrdinal("ReorderQuantity")) ? null : reader.GetDecimal(reader.GetOrdinal("ReorderQuantity")),
                LeadTimeDays = reader.IsDBNull(reader.GetOrdinal("LeadTimeDays")) ? null : reader.GetInt32(reader.GetOrdinal("LeadTimeDays")),
                PreferredSupplierId = reader.IsDBNull(reader.GetOrdinal("PreferredSupplierId")) ? null : reader.GetGuid(reader.GetOrdinal("PreferredSupplierId")),
                PreferredSupplierName = reader.IsDBNull(reader.GetOrdinal("PreferredSupplierName")) ? null : reader.GetString(reader.GetOrdinal("PreferredSupplierName")),
                StorageLocation = reader.IsDBNull(reader.GetOrdinal("StorageLocation")) ? null : reader.GetString(reader.GetOrdinal("StorageLocation")),
                StorageConditions = reader.IsDBNull(reader.GetOrdinal("StorageConditions")) ? null : reader.GetString(reader.GetOrdinal("StorageConditions")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddMaterialParameters(SqlCommand command, Material material)
        {
            command.Parameters.AddWithValue("@Id", material.Id);
            command.Parameters.AddWithValue("@MaterialCode", material.MaterialCode);
            command.Parameters.AddWithValue("@MaterialName", material.MaterialName);
            command.Parameters.AddWithValue("@Category", (object?)material.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubCategory", (object?)material.SubCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialType", (object?)material.MaterialType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Grade", (object?)material.Grade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specification", (object?)material.Specification ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)material.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@HSNCode", (object?)material.HSNCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardLength", (object?)material.StandardLength ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)material.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Thickness", (object?)material.Thickness ?? DBNull.Value);
            command.Parameters.AddWithValue("@Width", (object?)material.Width ?? DBNull.Value);
            command.Parameters.AddWithValue("@PrimaryUOM", (object?)material.PrimaryUOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@SecondaryUOM", (object?)material.SecondaryUOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@ConversionFactor", (object?)material.ConversionFactor ?? DBNull.Value);
            command.Parameters.AddWithValue("@WeightPerMeter", (object?)material.WeightPerMeter ?? DBNull.Value);
            command.Parameters.AddWithValue("@WeightPerPiece", (object?)material.WeightPerPiece ?? DBNull.Value);
            command.Parameters.AddWithValue("@Density", (object?)material.Density ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCost", (object?)material.StandardCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastPurchasePrice", (object?)material.LastPurchasePrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastPurchaseDate", (object?)material.LastPurchaseDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinStockLevel", (object?)material.MinStockLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaxStockLevel", (object?)material.MaxStockLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderLevel", (object?)material.ReorderLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReorderQuantity", (object?)material.ReorderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@LeadTimeDays", (object?)material.LeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredSupplierId", (object?)material.PreferredSupplierId ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredSupplierName", (object?)material.PreferredSupplierName ?? DBNull.Value);
            command.Parameters.AddWithValue("@StorageLocation", (object?)material.StorageLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@StorageConditions", (object?)material.StorageConditions ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", material.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)material.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)material.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", material.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)material.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)material.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)material.UpdatedBy ?? DBNull.Value);
        }
    }
}
