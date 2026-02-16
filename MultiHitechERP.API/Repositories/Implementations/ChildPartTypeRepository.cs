using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ChildPartTypeRepository : IChildPartTypeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ChildPartTypeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ChildPartType>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_ChildPartTypes ORDER BY TypeName";
            var result = new List<ChildPartType>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.Add(Map(reader));

            return result;
        }

        public async Task<IEnumerable<ChildPartType>> GetActiveAsync()
        {
            const string query = "SELECT * FROM Masters_ChildPartTypes WHERE IsActive = 1 ORDER BY TypeName";
            var result = new List<ChildPartType>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.Add(Map(reader));

            return result;
        }

        public async Task<ChildPartType?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_ChildPartTypes WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<int> CreateAsync(ChildPartType childPartType)
        {
            const string query = @"
                INSERT INTO Masters_ChildPartTypes (TypeName, IsActive, CreatedAt, CreatedBy)
                VALUES (@TypeName, 1, @CreatedAt, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TypeName", childPartType.TypeName);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)childPartType.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_ChildPartTypes WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static ChildPartType Map(SqlDataReader reader) => new()
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            TypeName = reader.GetString(reader.GetOrdinal("TypeName")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
        };
    }
}
