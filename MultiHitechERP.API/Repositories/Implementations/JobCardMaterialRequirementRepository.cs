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
    /// JobCardMaterialRequirement repository implementation using ADO.NET
    /// </summary>
    public class JobCardMaterialRequirementRepository : IJobCardMaterialRequirementRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public JobCardMaterialRequirementRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<JobCardMaterialRequirement?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Planning_JobCardMaterialRequirements WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToMaterialRequirement(reader) : null;
        }

        public async Task<IEnumerable<JobCardMaterialRequirement>> GetAllAsync()
        {
            const string query = "SELECT * FROM Planning_JobCardMaterialRequirements ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requirements = new List<JobCardMaterialRequirement>();
            while (await reader.ReadAsync())
            {
                requirements.Add(MapToMaterialRequirement(reader));
            }

            return requirements;
        }

        public async Task<IEnumerable<JobCardMaterialRequirement>> GetByJobCardIdAsync(int jobCardId)
        {
            const string query = "SELECT * FROM Planning_JobCardMaterialRequirements WHERE JobCardId = @JobCardId ORDER BY Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requirements = new List<JobCardMaterialRequirement>();
            while (await reader.ReadAsync())
            {
                requirements.Add(MapToMaterialRequirement(reader));
            }

            return requirements;
        }

        public async Task<int> InsertAsync(JobCardMaterialRequirement requirement)
        {
            const string query = @"
                INSERT INTO Planning_JobCardMaterialRequirements (
                    JobCardId, JobCardNo, RawMaterialId, RawMaterialName, MaterialGrade,
                    RequiredQuantity, Unit, WastagePercent, TotalQuantityWithWastage,
                    Source, ConfirmedBy, ConfirmedAt, CreatedAt, CreatedBy
                )
                VALUES (
                    @JobCardId, @JobCardNo, @RawMaterialId, @RawMaterialName, @MaterialGrade,
                    @RequiredQuantity, @Unit, @WastagePercent, @TotalQuantityWithWastage,
                    @Source, @ConfirmedBy, @ConfirmedAt, GETUTCDATE(), @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@JobCardId", requirement.JobCardId);
            command.Parameters.AddWithValue("@JobCardNo", (object?)requirement.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@RawMaterialId", (object?)requirement.RawMaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@RawMaterialName", requirement.RawMaterialName);
            command.Parameters.AddWithValue("@MaterialGrade", requirement.MaterialGrade);
            command.Parameters.AddWithValue("@RequiredQuantity", requirement.RequiredQuantity);
            command.Parameters.AddWithValue("@Unit", requirement.Unit);
            command.Parameters.AddWithValue("@WastagePercent", requirement.WastagePercent);
            command.Parameters.AddWithValue("@TotalQuantityWithWastage", requirement.TotalQuantityWithWastage);
            command.Parameters.AddWithValue("@Source", requirement.Source);
            command.Parameters.AddWithValue("@ConfirmedBy", requirement.ConfirmedBy);
            command.Parameters.AddWithValue("@ConfirmedAt", requirement.ConfirmedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)requirement.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return (int)(await command.ExecuteScalarAsync() ?? 0);
        }

        public async Task<bool> InsertBatchAsync(IEnumerable<JobCardMaterialRequirement> requirements)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var requirement in requirements)
                {
                    const string query = @"
                        INSERT INTO Planning_JobCardMaterialRequirements (
                            JobCardId, JobCardNo, RawMaterialId, RawMaterialName, MaterialGrade,
                            RequiredQuantity, Unit, WastagePercent, TotalQuantityWithWastage,
                            Source, ConfirmedBy, ConfirmedAt, CreatedAt, CreatedBy
                        )
                        VALUES (
                            @JobCardId, @JobCardNo, @RawMaterialId, @RawMaterialName, @MaterialGrade,
                            @RequiredQuantity, @Unit, @WastagePercent, @TotalQuantityWithWastage,
                            @Source, @ConfirmedBy, @ConfirmedAt, GETUTCDATE(), @CreatedBy
                        )";

                    using var command = new SqlCommand(query, connection, transaction);

                    command.Parameters.AddWithValue("@JobCardId", requirement.JobCardId);
                    command.Parameters.AddWithValue("@JobCardNo", (object?)requirement.JobCardNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RawMaterialId", (object?)requirement.RawMaterialId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RawMaterialName", requirement.RawMaterialName);
                    command.Parameters.AddWithValue("@MaterialGrade", requirement.MaterialGrade);
                    command.Parameters.AddWithValue("@RequiredQuantity", requirement.RequiredQuantity);
                    command.Parameters.AddWithValue("@Unit", requirement.Unit);
                    command.Parameters.AddWithValue("@WastagePercent", requirement.WastagePercent);
                    command.Parameters.AddWithValue("@TotalQuantityWithWastage", requirement.TotalQuantityWithWastage);
                    command.Parameters.AddWithValue("@Source", requirement.Source);
                    command.Parameters.AddWithValue("@ConfirmedBy", requirement.ConfirmedBy);
                    command.Parameters.AddWithValue("@ConfirmedAt", requirement.ConfirmedAt);
                    command.Parameters.AddWithValue("@CreatedBy", (object?)requirement.CreatedBy ?? DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(JobCardMaterialRequirement requirement)
        {
            const string query = @"
                UPDATE Planning_JobCardMaterialRequirements SET
                    RawMaterialId = @RawMaterialId,
                    RawMaterialName = @RawMaterialName,
                    MaterialGrade = @MaterialGrade,
                    RequiredQuantity = @RequiredQuantity,
                    Unit = @Unit,
                    WastagePercent = @WastagePercent,
                    TotalQuantityWithWastage = @TotalQuantityWithWastage,
                    Source = @Source,
                    ConfirmedBy = @ConfirmedBy,
                    ConfirmedAt = @ConfirmedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", requirement.Id);
            command.Parameters.AddWithValue("@RawMaterialId", (object?)requirement.RawMaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@RawMaterialName", requirement.RawMaterialName);
            command.Parameters.AddWithValue("@MaterialGrade", requirement.MaterialGrade);
            command.Parameters.AddWithValue("@RequiredQuantity", requirement.RequiredQuantity);
            command.Parameters.AddWithValue("@Unit", requirement.Unit);
            command.Parameters.AddWithValue("@WastagePercent", requirement.WastagePercent);
            command.Parameters.AddWithValue("@TotalQuantityWithWastage", requirement.TotalQuantityWithWastage);
            command.Parameters.AddWithValue("@Source", requirement.Source);
            command.Parameters.AddWithValue("@ConfirmedBy", requirement.ConfirmedBy);
            command.Parameters.AddWithValue("@ConfirmedAt", requirement.ConfirmedAt);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Planning_JobCardMaterialRequirements WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteByJobCardIdAsync(int jobCardId)
        {
            const string query = "DELETE FROM Planning_JobCardMaterialRequirements WHERE JobCardId = @JobCardId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        private static JobCardMaterialRequirement MapToMaterialRequirement(SqlDataReader reader)
        {
            return new JobCardMaterialRequirement
            {
                Id = reader.GetInt32("Id"),
                JobCardId = reader.GetInt32("JobCardId"),
                JobCardNo = reader.IsDBNull("JobCardNo") ? null : reader.GetString("JobCardNo"),
                RawMaterialId = reader.IsDBNull("RawMaterialId") ? null : reader.GetInt32("RawMaterialId"),
                RawMaterialName = reader.GetString("RawMaterialName"),
                MaterialGrade = reader.GetString("MaterialGrade"),
                RequiredQuantity = reader.GetDecimal("RequiredQuantity"),
                Unit = reader.GetString("Unit"),
                WastagePercent = reader.GetDecimal("WastagePercent"),
                TotalQuantityWithWastage = reader.GetDecimal("TotalQuantityWithWastage"),
                Source = reader.GetString("Source"),
                ConfirmedBy = reader.GetString("ConfirmedBy"),
                ConfirmedAt = reader.GetDateTime("ConfirmedAt"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                CreatedBy = reader.IsDBNull("CreatedBy") ? null : reader.GetString("CreatedBy")
            };
        }
    }
}
