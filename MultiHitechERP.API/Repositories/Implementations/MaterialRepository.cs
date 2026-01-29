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
                    MaterialCode, MaterialName, Grade, Shape, Diameter, LengthInMM,
                    Density, WeightKG, IsActive, CreatedAt, CreatedBy, UpdatedAt
                ) VALUES (
                    @MaterialCode, @MaterialName, @Grade, @Shape, @Diameter, @LengthInMM,
                    @Density, @WeightKG, @IsActive, @CreatedAt, @CreatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = DateTime.UtcNow;
            material.IsActive = true;

            command.Parameters.AddWithValue("@MaterialCode", material.MaterialCode);
            command.Parameters.AddWithValue("@MaterialName", material.MaterialName);
            command.Parameters.AddWithValue("@Grade", material.Grade);
            command.Parameters.AddWithValue("@Shape", material.Shape);
            command.Parameters.AddWithValue("@Diameter", material.Diameter);
            command.Parameters.AddWithValue("@LengthInMM", material.LengthInMM);
            command.Parameters.AddWithValue("@Density", material.Density);
            command.Parameters.AddWithValue("@WeightKG", material.WeightKG);
            command.Parameters.AddWithValue("@IsActive", material.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", material.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)material.CreatedBy ?? DBNull.Value);
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

        public async Task<int> GetNextSequenceNumberAsync(string grade, string shape, decimal diameter)
        {
            // MaterialCode format: GRADE-SHAPE-DIAMETER-SEQ
            // Example: EN8-ROD-050-001, EN8-ROD-050-002
            string prefix = $"{grade.Replace(" ", "")}-{shape.ToUpper().Substring(0, 3)}-{((int)diameter):D3}";

            const string query = @"
                SELECT ISNULL(MAX(CAST(RIGHT(MaterialCode, 3) AS INT)), 0) + 1
                FROM Masters_Materials
                WHERE MaterialCode LIKE @Prefix + '%'";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Prefix", prefix);

            await connection.OpenAsync();
            var nextSequence = (int)await command.ExecuteScalarAsync();

            return nextSequence;
        }

        private Material MapToMaterial(SqlDataReader reader)
        {
            return new Material
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MaterialCode = reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialName = reader.GetString(reader.GetOrdinal("MaterialName")),
                Grade = reader.GetString(reader.GetOrdinal("Grade")),
                Shape = reader.GetString(reader.GetOrdinal("Shape")),
                Diameter = reader.GetDecimal(reader.GetOrdinal("Diameter")),
                LengthInMM = reader.GetDecimal(reader.GetOrdinal("LengthInMM")),
                Density = reader.GetDecimal(reader.GetOrdinal("Density")),
                WeightKG = reader.GetDecimal(reader.GetOrdinal("WeightKG")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
