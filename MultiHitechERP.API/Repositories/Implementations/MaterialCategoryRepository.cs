using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MaterialCategoryRepository : IMaterialCategoryRepository
    {
        private readonly string _connectionString;

        public MaterialCategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<MaterialCategory>> GetAllAsync()
        {
            var categories = new List<MaterialCategory>();
            const string query = "SELECT * FROM Masters_MaterialCategories ORDER BY CategoryName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(MapToMaterialCategory(reader));
            }

            return categories;
        }

        public async Task<MaterialCategory?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_MaterialCategories WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToMaterialCategory(reader);
            }

            return null;
        }

        public async Task<MaterialCategory?> GetByCategoryCodeAsync(string categoryCode)
        {
            const string query = "SELECT * FROM Masters_MaterialCategories WHERE CategoryCode = @CategoryCode";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryCode", categoryCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToMaterialCategory(reader);
            }

            return null;
        }

        public async Task<IEnumerable<MaterialCategory>> GetByMaterialTypeAsync(string materialType)
        {
            var categories = new List<MaterialCategory>();
            const string query = "SELECT * FROM Masters_MaterialCategories WHERE MaterialType = @MaterialType ORDER BY CategoryName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialType", materialType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(MapToMaterialCategory(reader));
            }

            return categories;
        }

        public async Task<IEnumerable<MaterialCategory>> GetActiveAsync()
        {
            var categories = new List<MaterialCategory>();
            const string query = "SELECT * FROM Masters_MaterialCategories WHERE IsActive = 1 ORDER BY CategoryName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(MapToMaterialCategory(reader));
            }

            return categories;
        }

        public async Task<IEnumerable<MaterialCategory>> SearchByNameAsync(string searchTerm)
        {
            var categories = new List<MaterialCategory>();
            const string query = @"
                SELECT * FROM Masters_MaterialCategories
                WHERE CategoryName LIKE @SearchTerm
                   OR CategoryCode LIKE @SearchTerm
                   OR Description LIKE @SearchTerm
                ORDER BY CategoryName";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(MapToMaterialCategory(reader));
            }

            return categories;
        }

        public async Task<int> CreateAsync(MaterialCategory category)
        {
            const string query = @"
                INSERT INTO Masters_MaterialCategories (
                    CategoryCode, CategoryName, Quality, Description, DefaultUOM,
                    MaterialType, IsActive, CreatedAt, UpdatedAt
                ) VALUES (
                    @CategoryCode, @CategoryName, @Quality, @Description, @DefaultUOM,
                    @MaterialType, @IsActive, @CreatedAt, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CategoryCode", category.CategoryCode);
            command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            command.Parameters.AddWithValue("@Quality", category.Quality);
            command.Parameters.AddWithValue("@Description", category.Description);
            command.Parameters.AddWithValue("@DefaultUOM", category.DefaultUOM);
            command.Parameters.AddWithValue("@MaterialType", category.MaterialType);
            command.Parameters.AddWithValue("@IsActive", category.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", category.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", category.UpdatedAt);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(int id, MaterialCategory category)
        {
            const string query = @"
                UPDATE Masters_MaterialCategories
                SET CategoryCode = @CategoryCode,
                    CategoryName = @CategoryName,
                    Quality = @Quality,
                    Description = @Description,
                    DefaultUOM = @DefaultUOM,
                    MaterialType = @MaterialType,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@CategoryCode", category.CategoryCode);
            command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            command.Parameters.AddWithValue("@Quality", category.Quality);
            command.Parameters.AddWithValue("@Description", category.Description);
            command.Parameters.AddWithValue("@DefaultUOM", category.DefaultUOM);
            command.Parameters.AddWithValue("@MaterialType", category.MaterialType);
            command.Parameters.AddWithValue("@IsActive", category.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", category.UpdatedAt);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_MaterialCategories WHERE Id = @Id";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> CategoryCodeExistsAsync(string categoryCode, int? excludeId = null)
        {
            var query = "SELECT COUNT(1) FROM Masters_MaterialCategories WHERE CategoryCode = @CategoryCode";
            if (excludeId.HasValue)
            {
                query += " AND Id != @ExcludeId";
            }

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryCode", categoryCode);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            await connection.OpenAsync();
            var count = (int)(await command.ExecuteScalarAsync() ?? 0);
            return count > 0;
        }

        private MaterialCategory MapToMaterialCategory(SqlDataReader reader)
        {
            return new MaterialCategory
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                CategoryCode = reader.GetString(reader.GetOrdinal("CategoryCode")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Quality = reader.GetString(reader.GetOrdinal("Quality")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DefaultUOM = reader.GetString(reader.GetOrdinal("DefaultUOM")),
                MaterialType = reader.GetString(reader.GetOrdinal("MaterialType")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
