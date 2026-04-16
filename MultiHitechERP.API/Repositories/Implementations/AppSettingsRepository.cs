using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class AppSettingsRepository : IAppSettingsRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AppSettingsRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string?> GetValueAsync(string key)
        {
            const string query = "SELECT [Value] FROM App_Settings WHERE [Key] = @Key";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Key", key);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value || result == null ? null : result.ToString();
        }

        public async Task SetValueAsync(string key, string value, string? updatedBy = null)
        {
            const string query = @"
                UPDATE App_Settings
                SET [Value] = @Value, UpdatedAt = GETUTCDATE(), UpdatedBy = @UpdatedBy
                WHERE [Key] = @Key";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Key", key);
            command.Parameters.AddWithValue("@Value", value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<AppSetting>> GetAllAsync()
        {
            const string query = "SELECT Id, [Key], [Value], Description, UpdatedAt, UpdatedBy FROM App_Settings ORDER BY [Key]";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var list = new List<AppSetting>();
            while (await reader.ReadAsync())
            {
                list.Add(new AppSetting
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Key = reader.GetString(reader.GetOrdinal("Key")),
                    Value = reader.GetString(reader.GetOrdinal("Value")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                    UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                });
            }
            return list;
        }
    }
}
