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

        public async Task<Material?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

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

        public async Task<int> InsertAsync(Material material)
        {
            const string query = @"
                INSERT INTO Masters_Materials (
                    MaterialName, Grade, Shape, Diameter, LengthInMM,
                    Density, WeightKG, CreatedAt, UpdatedAt
                ) VALUES (
                    @MaterialName, @Grade, @Shape, @Diameter, @LengthInMM,
                    @Density, @WeightKG, @CreatedAt, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@MaterialName", material.MaterialName);
            command.Parameters.AddWithValue("@Grade", material.Grade);
            command.Parameters.AddWithValue("@Shape", material.Shape);
            command.Parameters.AddWithValue("@Diameter", material.Diameter);
            command.Parameters.AddWithValue("@LengthInMM", material.LengthInMM);
            command.Parameters.AddWithValue("@Density", material.Density);
            command.Parameters.AddWithValue("@WeightKG", material.WeightKG);
            command.Parameters.AddWithValue("@CreatedAt", material.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", material.UpdatedAt);

            await connection.OpenAsync();
            var materialId = (int)await command.ExecuteScalarAsync();
            material.Id = materialId;

            return materialId;
        }

        public async Task<bool> UpdateAsync(Material material)
        {
            const string query = @"
                UPDATE Masters_Materials SET
                    MaterialName = @MaterialName,
                    Grade = @Grade,
                    Shape = @Shape,
                    Diameter = @Diameter,
                    LengthInMM = @LengthInMM,
                    Density = @Density,
                    WeightKG = @WeightKG,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            material.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@Id", material.Id);
            command.Parameters.AddWithValue("@MaterialName", material.MaterialName);
            command.Parameters.AddWithValue("@Grade", material.Grade);
            command.Parameters.AddWithValue("@Shape", material.Shape);
            command.Parameters.AddWithValue("@Diameter", material.Diameter);
            command.Parameters.AddWithValue("@LengthInMM", material.LengthInMM);
            command.Parameters.AddWithValue("@Density", material.Density);
            command.Parameters.AddWithValue("@WeightKG", material.WeightKG);
            command.Parameters.AddWithValue("@UpdatedAt", material.UpdatedAt);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Materials WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<Material>> SearchByNameAsync(string name)
        {
            const string query = @"
                SELECT * FROM Masters_Materials
                WHERE MaterialName LIKE '%' + @Name + '%'
                   OR Grade LIKE '%' + @Name + '%'
                ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

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

        public async Task<IEnumerable<Material>> GetByShapeAsync(string shape)
        {
            const string query = "SELECT * FROM Masters_Materials WHERE Shape = @Shape ORDER BY MaterialName";

            var materials = new List<Material>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Shape", shape);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                materials.Add(MapToMaterial(reader));

            return materials;
        }

        public async Task<bool> ExistsByNameAsync(string materialName)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Materials WHERE MaterialName = @MaterialName";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialName", materialName);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private Material MapToMaterial(SqlDataReader reader)
        {
            return new Material
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MaterialName = reader.GetString(reader.GetOrdinal("MaterialName")),
                Grade = reader.GetString(reader.GetOrdinal("Grade")),
                Shape = reader.GetString(reader.GetOrdinal("Shape")),
                Diameter = reader.GetDecimal(reader.GetOrdinal("Diameter")),
                LengthInMM = reader.GetDecimal(reader.GetOrdinal("LengthInMM")),
                Density = reader.GetDecimal(reader.GetOrdinal("Density")),
                WeightKG = reader.GetDecimal(reader.GetOrdinal("WeightKG")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
