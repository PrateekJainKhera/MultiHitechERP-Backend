using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MachineModelRepository : IMachineModelRepository
    {
        private readonly string _connectionString;

        public MachineModelRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<IEnumerable<MachineModel>> GetAllAsync()
        {
            var models = new List<MachineModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT Id, ModelName, CreatedAt, CreatedBy, UpdatedAt, IsActive
                    FROM Masters_MachineModels
                    ORDER BY ModelName", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        models.Add(MapToModel(reader));
                    }
                }
            }

            return models;
        }

        public async Task<MachineModel?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT Id, ModelName, CreatedAt, CreatedBy, UpdatedAt, IsActive
                    FROM Masters_MachineModels
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToModel(reader);
                    }
                }
            }

            return null;
        }

        public async Task<MachineModel?> GetByNameAsync(string modelName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT Id, ModelName, CreatedAt, CreatedBy, UpdatedAt, IsActive
                    FROM Masters_MachineModels
                    WHERE ModelName = @ModelName", connection);

                command.Parameters.AddWithValue("@ModelName", modelName);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapToModel(reader);
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateAsync(MachineModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    INSERT INTO Masters_MachineModels (ModelName, CreatedAt, CreatedBy, UpdatedAt, IsActive)
                    VALUES (@ModelName, @CreatedAt, @CreatedBy, @UpdatedAt, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

                command.Parameters.AddWithValue("@ModelName", model.ModelName);
                command.Parameters.AddWithValue("@CreatedAt", model.CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", model.UpdatedAt);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);

                var id = await command.ExecuteScalarAsync();
                return id != null ? Convert.ToInt32(id) : 0;
            }
        }

        public async Task<bool> UpdateAsync(MachineModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    UPDATE Masters_MachineModels
                    SET ModelName = @ModelName,
                        UpdatedAt = @UpdatedAt,
                        IsActive = @IsActive
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", model.Id);
                command.Parameters.AddWithValue("@ModelName", model.ModelName);
                command.Parameters.AddWithValue("@UpdatedAt", model.UpdatedAt);
                command.Parameters.AddWithValue("@IsActive", model.IsActive);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    DELETE FROM Masters_MachineModels
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT COUNT(1)
                    FROM Masters_MachineModels
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                var count = (int?)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<bool> ExistsByNameAsync(string modelName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT COUNT(1)
                    FROM Masters_MachineModels
                    WHERE ModelName = @ModelName", connection);

                command.Parameters.AddWithValue("@ModelName", modelName);

                var count = (int?)await command.ExecuteScalarAsync();
                return count > 0;
            }
        }

        public async Task<int> GetProductCountAsync(int modelId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM Masters_Products
                    WHERE ModelId = @ModelId", connection);

                command.Parameters.AddWithValue("@ModelId", modelId);

                var count = await command.ExecuteScalarAsync();
                return count != null ? Convert.ToInt32(count) : 0;
            }
        }

        private static MachineModel MapToModel(SqlDataReader reader)
        {
            return new MachineModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ModelName = reader.GetString(reader.GetOrdinal("ModelName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
