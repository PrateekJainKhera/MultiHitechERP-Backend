using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProcessCategoryRepository : IProcessCategoryRepository
    {
        private readonly string _connectionString;

        public ProcessCategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<ProcessCategory>> GetAllAsync()
        {
            var categories = new List<ProcessCategory>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT Id, CategoryCode, CategoryName, Description, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_ProcessCategories
                ORDER BY CategoryName", connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                categories.Add(MapToProcessCategory(reader));
            }

            return categories;
        }

        public async Task<ProcessCategory?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT Id, CategoryCode, CategoryName, Description, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_ProcessCategories
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToProcessCategory(reader);
            }

            return null;
        }

        public async Task<ProcessCategory?> GetByCategoryCodeAsync(string categoryCode)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT Id, CategoryCode, CategoryName, Description, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Masters_ProcessCategories
                WHERE CategoryCode = @CategoryCode", connection);

            command.Parameters.AddWithValue("@CategoryCode", categoryCode);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToProcessCategory(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(ProcessCategory processCategory)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                INSERT INTO Masters_ProcessCategories
                (CategoryCode, CategoryName, Description, IsActive, CreatedAt, CreatedBy)
                VALUES
                (@CategoryCode, @CategoryName, @Description, @IsActive, @CreatedAt, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

            command.Parameters.AddWithValue("@CategoryCode", processCategory.CategoryCode);
            command.Parameters.AddWithValue("@CategoryName", processCategory.CategoryName);
            command.Parameters.AddWithValue("@Description", (object?)processCategory.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", processCategory.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)processCategory.CreatedBy ?? DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> UpdateAsync(ProcessCategory processCategory)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Masters_ProcessCategories SET
                    CategoryCode = @CategoryCode,
                    CategoryName = @CategoryName,
                    Description = @Description,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", processCategory.Id);
            command.Parameters.AddWithValue("@CategoryCode", processCategory.CategoryCode);
            command.Parameters.AddWithValue("@CategoryName", processCategory.CategoryName);
            command.Parameters.AddWithValue("@Description", (object?)processCategory.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", processCategory.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)processCategory.UpdatedBy ?? DBNull.Value);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM Masters_ProcessCategories WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> CategoryCodeExistsAsync(string categoryCode, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(1) FROM Masters_ProcessCategories WHERE CategoryCode = @CategoryCode";
            if (excludeId.HasValue)
            {
                query += " AND Id != @ExcludeId";
            }

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CategoryCode", categoryCode);
            if (excludeId.HasValue)
            {
                command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
            }

            var count = (int)await command.ExecuteScalarAsync();
            return count > 0;
        }

        private static ProcessCategory MapToProcessCategory(SqlDataReader reader)
        {
            return new ProcessCategory
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                CategoryCode = reader.GetString(reader.GetOrdinal("CategoryCode")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
