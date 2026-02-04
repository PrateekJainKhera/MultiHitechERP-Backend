using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class DrawingRepository : IDrawingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DrawingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Drawing?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Drawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToDrawing(reader) : null;
        }

        public async Task<IEnumerable<Drawing>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Drawings ORDER BY DrawingName";

            var drawings = new List<Drawing>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                drawings.Add(MapToDrawing(reader));

            return drawings;
        }

        public async Task<int> InsertAsync(Drawing drawing)
        {
            const string query = @"
                INSERT INTO Masters_Drawings (
                    DrawingNumber, DrawingName, DrawingType, RevisionNumber,
                    Status, Description, Remarks, IsActive, CreatedAt, CreatedBy
                ) VALUES (
                    @DrawingNumber, @DrawingName, @DrawingType, @RevisionNumber,
                    @Status, @Description, @Remarks, @IsActive, @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            drawing.CreatedAt = DateTime.UtcNow;
            command.Parameters.AddWithValue("@DrawingNumber", drawing.DrawingNumber);
            command.Parameters.AddWithValue("@DrawingName", drawing.DrawingName);
            command.Parameters.AddWithValue("@DrawingType", drawing.DrawingType);
            command.Parameters.AddWithValue("@RevisionNumber", (object?)drawing.RevisionNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", drawing.Status);
            command.Parameters.AddWithValue("@Description", (object?)drawing.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)drawing.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", drawing.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", drawing.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)drawing.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync()!;
        }

        public async Task<bool> UpdateAsync(Drawing drawing)
        {
            const string query = @"
                UPDATE Masters_Drawings SET
                    DrawingName = @DrawingName,
                    DrawingType = @DrawingType,
                    RevisionNumber = @RevisionNumber,
                    Status = @Status,
                    Description = @Description,
                    Remarks = @Remarks,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            drawing.UpdatedAt = DateTime.UtcNow;
            command.Parameters.AddWithValue("@Id", drawing.Id);
            command.Parameters.AddWithValue("@DrawingName", drawing.DrawingName);
            command.Parameters.AddWithValue("@DrawingType", drawing.DrawingType);
            command.Parameters.AddWithValue("@RevisionNumber", (object?)drawing.RevisionNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", drawing.Status);
            command.Parameters.AddWithValue("@Description", (object?)drawing.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)drawing.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", drawing.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", drawing.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)drawing.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Drawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<string> GetNextDrawingNumberAsync()
        {
            const string query = @"
                SELECT COALESCE(MAX(CAST(RIGHT(DrawingNumber, LEN(DrawingNumber) - CHARINDEX('-', DrawingNumber)) AS INT)), 0)
                FROM Masters_Drawings";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            var maxNum = (int)await command.ExecuteScalarAsync()!;
            return $"DWG-{maxNum + 1:D3}";
        }

        public async Task<bool> ExistsAsync(string drawingNumber)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Drawings WHERE DrawingNumber = @DrawingNumber";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingNumber", drawingNumber);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync()! > 0;
        }

        private Drawing MapToDrawing(SqlDataReader reader)
        {
            return new Drawing
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                DrawingNumber = reader.GetString(reader.GetOrdinal("DrawingNumber")),
                DrawingName = reader.GetString(reader.GetOrdinal("DrawingName")),
                DrawingType = reader.GetString(reader.GetOrdinal("DrawingType")),
                RevisionNumber = reader.IsDBNull(reader.GetOrdinal("RevisionNumber")) ? null : reader.GetString(reader.GetOrdinal("RevisionNumber")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
