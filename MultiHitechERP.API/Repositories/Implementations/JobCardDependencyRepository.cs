using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Planning;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// JobCardDependency repository implementation using ADO.NET
    /// </summary>
    public class JobCardDependencyRepository : IJobCardDependencyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JobCardDependencyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JobCardDependency?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Planning_JobCardDependencies WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToDependency(reader) : null;
        }

        public async Task<IEnumerable<JobCardDependency>> GetAllAsync()
        {
            const string query = "SELECT * FROM Planning_JobCardDependencies ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var dependencies = new List<JobCardDependency>();
            while (await reader.ReadAsync())
            {
                dependencies.Add(MapToDependency(reader));
            }

            return dependencies;
        }

        public async Task<IEnumerable<JobCardDependency>> GetDependenciesForJobCardAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCardDependencies
                WHERE DependentJobCardId = @JobCardId
                ORDER BY CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var dependencies = new List<JobCardDependency>();
            while (await reader.ReadAsync())
            {
                dependencies.Add(MapToDependency(reader));
            }

            return dependencies;
        }

        public async Task<IEnumerable<JobCardDependency>> GetPrerequisitesForJobCardAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCardDependencies
                WHERE DependentJobCardId = @JobCardId
                ORDER BY CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var dependencies = new List<JobCardDependency>();
            while (await reader.ReadAsync())
            {
                dependencies.Add(MapToDependency(reader));
            }

            return dependencies;
        }

        public async Task<IEnumerable<JobCardDependency>> GetUnresolvedDependenciesAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Planning_JobCardDependencies
                WHERE DependentJobCardId = @JobCardId AND IsResolved = 0
                ORDER BY CreatedAt";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var dependencies = new List<JobCardDependency>();
            while (await reader.ReadAsync())
            {
                dependencies.Add(MapToDependency(reader));
            }

            return dependencies;
        }

        public async Task<bool> HasUnresolvedDependenciesAsync(int jobCardId)
        {
            const string query = @"
                SELECT COUNT(1) FROM Planning_JobCardDependencies
                WHERE DependentJobCardId = @JobCardId AND IsResolved = 0";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<int> InsertAsync(JobCardDependency dependency)
        {
            const string query = @"
                INSERT INTO Planning_JobCardDependencies
                (Id, DependentJobCardId, DependentJobCardNo, PrerequisiteJobCardId, PrerequisiteJobCardNo,
                 DependencyType, IsResolved, ResolvedAt, LagTimeMinutes, CreatedAt)
                VALUES
                (@Id, @DependentJobCardId, @DependentJobCardNo, @PrerequisiteJobCardId, @PrerequisiteJobCardNo,
                 @DependencyType, @IsResolved, @ResolvedAt, @LagTimeMinutes, @CreatedAt)";

            var dependencyId = 0;
            dependency.Id = dependencyId;
            dependency.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddDependencyParameters(command, dependency);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return dependencyId;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Planning_JobCardDependencies WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> MarkAsResolvedAsync(int dependencyId)
        {
            const string query = @"
                UPDATE Planning_JobCardDependencies
                SET IsResolved = 1, ResolvedAt = @ResolvedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", dependencyId);
            command.Parameters.AddWithValue("@ResolvedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> MarkAllResolvedForPrerequisiteAsync(int prerequisiteJobCardId)
        {
            const string query = @"
                UPDATE Planning_JobCardDependencies
                SET IsResolved = 1, ResolvedAt = @ResolvedAt
                WHERE PrerequisiteJobCardId = @PrerequisiteJobCardId AND IsResolved = 0";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PrerequisiteJobCardId", prerequisiteJobCardId);
            command.Parameters.AddWithValue("@ResolvedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> WouldCreateCircularDependencyAsync(int dependentJobCardId, int prerequisiteJobCardId)
        {
            // Use recursive CTE to check if prerequisiteJobCardId depends on dependentJobCardId
            const string query = @"
                WITH DependencyChain AS (
                    -- Base case: direct dependencies
                    SELECT PrerequisiteJobCardId, DependentJobCardId, 1 AS Level
                    FROM Planning_JobCardDependencies
                    WHERE DependentJobCardId = @PrerequisiteJobCardId

                    UNION ALL

                    -- Recursive case: follow the chain
                    SELECT d.PrerequisiteJobCardId, d.DependentJobCardId, dc.Level + 1
                    FROM Planning_JobCardDependencies d
                    INNER JOIN DependencyChain dc ON d.DependentJobCardId = dc.PrerequisiteJobCardId
                    WHERE dc.Level < 10  -- Prevent infinite loops
                )
                SELECT COUNT(1)
                FROM DependencyChain
                WHERE PrerequisiteJobCardId = @DependentJobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DependentJobCardId", dependentJobCardId);
            command.Parameters.AddWithValue("@PrerequisiteJobCardId", prerequisiteJobCardId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // Helper Methods
        private static JobCardDependency MapToDependency(SqlDataReader reader)
        {
            return new JobCardDependency
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                DependentJobCardId = reader.GetInt32(reader.GetOrdinal("DependentJobCardId")),
                DependentJobCardNo = reader.IsDBNull(reader.GetOrdinal("DependentJobCardNo")) ? null : reader.GetString(reader.GetOrdinal("DependentJobCardNo")),
                PrerequisiteJobCardId = reader.GetInt32(reader.GetOrdinal("PrerequisiteJobCardId")),
                PrerequisiteJobCardNo = reader.IsDBNull(reader.GetOrdinal("PrerequisiteJobCardNo")) ? null : reader.GetString(reader.GetOrdinal("PrerequisiteJobCardNo")),
                DependencyType = reader.GetString(reader.GetOrdinal("DependencyType")),
                IsResolved = reader.GetBoolean(reader.GetOrdinal("IsResolved")),
                ResolvedAt = reader.IsDBNull(reader.GetOrdinal("ResolvedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ResolvedAt")),
                LagTimeMinutes = reader.IsDBNull(reader.GetOrdinal("LagTimeMinutes")) ? null : reader.GetInt32(reader.GetOrdinal("LagTimeMinutes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        private static void AddDependencyParameters(SqlCommand command, JobCardDependency dependency)
        {
            command.Parameters.AddWithValue("@Id", dependency.Id);
            command.Parameters.AddWithValue("@DependentJobCardId", dependency.DependentJobCardId);
            command.Parameters.AddWithValue("@DependentJobCardNo", (object?)dependency.DependentJobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@PrerequisiteJobCardId", dependency.PrerequisiteJobCardId);
            command.Parameters.AddWithValue("@PrerequisiteJobCardNo", (object?)dependency.PrerequisiteJobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@DependencyType", dependency.DependencyType);
            command.Parameters.AddWithValue("@IsResolved", dependency.IsResolved);
            command.Parameters.AddWithValue("@ResolvedAt", (object?)dependency.ResolvedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@LagTimeMinutes", (object?)dependency.LagTimeMinutes ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", dependency.CreatedAt);
        }
    }
}
