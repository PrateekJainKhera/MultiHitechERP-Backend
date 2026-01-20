using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Drawing repository implementation using ADO.NET
    /// </summary>
    public class DrawingRepository : IDrawingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DrawingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Drawing?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Masters_Drawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToDrawing(reader) : null;
        }

        public async Task<Drawing?> GetByDrawingNumberAsync(string drawingNumber)
        {
            const string query = "SELECT * FROM Masters_Drawings WHERE DrawingNumber = @DrawingNumber AND IsLatestRevision = 1";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingNumber", drawingNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToDrawing(reader) : null;
        }

        public async Task<IEnumerable<Drawing>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Drawings ORDER BY DrawingNumber, VersionNumber DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<IEnumerable<Drawing>> GetActiveDrawingsAsync()
        {
            const string query = "SELECT * FROM Masters_Drawings WHERE IsActive = 1 AND IsLatestRevision = 1 ORDER BY DrawingNumber";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<IEnumerable<Drawing>> GetRevisionHistoryAsync(string drawingNumber)
        {
            const string query = @"
                SELECT * FROM Masters_Drawings
                WHERE DrawingNumber = @DrawingNumber
                ORDER BY VersionNumber DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingNumber", drawingNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<Drawing?> GetLatestRevisionAsync(string drawingNumber)
        {
            const string query = @"
                SELECT TOP 1 * FROM Masters_Drawings
                WHERE DrawingNumber = @DrawingNumber AND IsLatestRevision = 1
                ORDER BY VersionNumber DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingNumber", drawingNumber);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToDrawing(reader) : null;
        }

        public async Task<IEnumerable<Drawing>> GetByProductIdAsync(Guid productId)
        {
            const string query = @"
                SELECT * FROM Masters_Drawings
                WHERE ProductId = @ProductId AND IsLatestRevision = 1 AND IsActive = 1
                ORDER BY DrawingNumber";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<IEnumerable<Drawing>> GetByDrawingTypeAsync(string drawingType)
        {
            const string query = @"
                SELECT * FROM Masters_Drawings
                WHERE DrawingType = @DrawingType AND IsLatestRevision = 1 AND IsActive = 1
                ORDER BY DrawingNumber";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingType", drawingType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<IEnumerable<Drawing>> GetPendingApprovalAsync()
        {
            const string query = @"
                SELECT * FROM Masters_Drawings
                WHERE ApprovedBy IS NULL AND IsActive = 1 AND IsLatestRevision = 1
                ORDER BY CreatedAt DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var drawings = new List<Drawing>();
            while (await reader.ReadAsync())
            {
                drawings.Add(MapToDrawing(reader));
            }

            return drawings;
        }

        public async Task<Guid> InsertAsync(Drawing drawing)
        {
            const string query = @"
                INSERT INTO Masters_Drawings
                (Id, DrawingNumber, DrawingTitle, ProductId, ProductCode, ProductName, RevisionNumber, RevisionDate,
                 RevisionDescription, DrawingType, Category, FilePath, FileName, FileFormat, FileSize,
                 PreparedBy, CheckedBy, ApprovedBy, ApprovalDate, MaterialSpecification, Finish, ToleranceGrade,
                 TreatmentRequired, OverallLength, OverallWidth, OverallHeight, Weight,
                 IsActive, Status, IsLatestRevision, PreviousRevisionId, VersionNumber,
                 Remarks, CreatedAt, CreatedBy)
                VALUES
                (@Id, @DrawingNumber, @DrawingTitle, @ProductId, @ProductCode, @ProductName, @RevisionNumber, @RevisionDate,
                 @RevisionDescription, @DrawingType, @Category, @FilePath, @FileName, @FileFormat, @FileSize,
                 @PreparedBy, @CheckedBy, @ApprovedBy, @ApprovalDate, @MaterialSpecification, @Finish, @ToleranceGrade,
                 @TreatmentRequired, @OverallLength, @OverallWidth, @OverallHeight, @Weight,
                 @IsActive, @Status, @IsLatestRevision, @PreviousRevisionId, @VersionNumber,
                 @Remarks, @CreatedAt, @CreatedBy)";

            var drawingId = Guid.NewGuid();
            drawing.Id = drawingId;
            drawing.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddDrawingParameters(command, drawing);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return drawingId;
        }

        public async Task<bool> UpdateAsync(Drawing drawing)
        {
            const string query = @"
                UPDATE Masters_Drawings SET
                    DrawingNumber = @DrawingNumber,
                    DrawingTitle = @DrawingTitle,
                    ProductId = @ProductId,
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    RevisionNumber = @RevisionNumber,
                    RevisionDate = @RevisionDate,
                    RevisionDescription = @RevisionDescription,
                    DrawingType = @DrawingType,
                    Category = @Category,
                    FilePath = @FilePath,
                    FileName = @FileName,
                    FileFormat = @FileFormat,
                    FileSize = @FileSize,
                    PreparedBy = @PreparedBy,
                    CheckedBy = @CheckedBy,
                    ApprovedBy = @ApprovedBy,
                    ApprovalDate = @ApprovalDate,
                    MaterialSpecification = @MaterialSpecification,
                    Finish = @Finish,
                    ToleranceGrade = @ToleranceGrade,
                    TreatmentRequired = @TreatmentRequired,
                    OverallLength = @OverallLength,
                    OverallWidth = @OverallWidth,
                    OverallHeight = @OverallHeight,
                    Weight = @Weight,
                    IsActive = @IsActive,
                    Status = @Status,
                    IsLatestRevision = @IsLatestRevision,
                    PreviousRevisionId = @PreviousRevisionId,
                    VersionNumber = @VersionNumber,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            drawing.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddDrawingParameters(command, drawing);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Drawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> MarkAsLatestRevisionAsync(Guid id)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Get the drawing
                var drawing = await GetByIdAsync(id);
                if (drawing == null)
                    return false;

                // Mark all revisions of this drawing as not latest
                const string updateOthersQuery = @"
                    UPDATE Masters_Drawings
                    SET IsLatestRevision = 0
                    WHERE DrawingNumber = @DrawingNumber";

                using (var cmd1 = new SqlCommand(updateOthersQuery, connection, transaction))
                {
                    cmd1.Parameters.AddWithValue("@DrawingNumber", drawing.DrawingNumber);
                    await cmd1.ExecuteNonQueryAsync();
                }

                // Mark this one as latest
                const string updateThisQuery = @"
                    UPDATE Masters_Drawings
                    SET IsLatestRevision = 1, UpdatedAt = @UpdatedAt
                    WHERE Id = @Id";

                using (var cmd2 = new SqlCommand(updateThisQuery, connection, transaction))
                {
                    cmd2.Parameters.AddWithValue("@Id", id);
                    cmd2.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    await cmd2.ExecuteNonQueryAsync();
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

        public async Task<bool> ExistsAsync(string drawingNumber, string revisionNumber)
        {
            const string query = @"
                SELECT COUNT(1) FROM Masters_Drawings
                WHERE DrawingNumber = @DrawingNumber AND RevisionNumber = @RevisionNumber";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingNumber", drawingNumber);
            command.Parameters.AddWithValue("@RevisionNumber", revisionNumber ?? string.Empty);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // Helper Methods
        private static Drawing MapToDrawing(SqlDataReader reader)
        {
            return new Drawing
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                DrawingNumber = reader.GetString(reader.GetOrdinal("DrawingNumber")),
                DrawingTitle = reader.GetString(reader.GetOrdinal("DrawingTitle")),
                ProductId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? null : reader.GetGuid(reader.GetOrdinal("ProductId")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                RevisionNumber = reader.IsDBNull(reader.GetOrdinal("RevisionNumber")) ? null : reader.GetString(reader.GetOrdinal("RevisionNumber")),
                RevisionDate = reader.IsDBNull(reader.GetOrdinal("RevisionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("RevisionDate")),
                RevisionDescription = reader.IsDBNull(reader.GetOrdinal("RevisionDescription")) ? null : reader.GetString(reader.GetOrdinal("RevisionDescription")),
                DrawingType = reader.IsDBNull(reader.GetOrdinal("DrawingType")) ? null : reader.GetString(reader.GetOrdinal("DrawingType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                FilePath = reader.IsDBNull(reader.GetOrdinal("FilePath")) ? null : reader.GetString(reader.GetOrdinal("FilePath")),
                FileName = reader.IsDBNull(reader.GetOrdinal("FileName")) ? null : reader.GetString(reader.GetOrdinal("FileName")),
                FileFormat = reader.IsDBNull(reader.GetOrdinal("FileFormat")) ? null : reader.GetString(reader.GetOrdinal("FileFormat")),
                FileSize = reader.IsDBNull(reader.GetOrdinal("FileSize")) ? null : reader.GetInt64(reader.GetOrdinal("FileSize")),
                PreparedBy = reader.IsDBNull(reader.GetOrdinal("PreparedBy")) ? null : reader.GetString(reader.GetOrdinal("PreparedBy")),
                CheckedBy = reader.IsDBNull(reader.GetOrdinal("CheckedBy")) ? null : reader.GetString(reader.GetOrdinal("CheckedBy")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovalDate = reader.IsDBNull(reader.GetOrdinal("ApprovalDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovalDate")),
                MaterialSpecification = reader.IsDBNull(reader.GetOrdinal("MaterialSpecification")) ? null : reader.GetString(reader.GetOrdinal("MaterialSpecification")),
                Finish = reader.IsDBNull(reader.GetOrdinal("Finish")) ? null : reader.GetString(reader.GetOrdinal("Finish")),
                ToleranceGrade = reader.IsDBNull(reader.GetOrdinal("ToleranceGrade")) ? null : reader.GetString(reader.GetOrdinal("ToleranceGrade")),
                TreatmentRequired = reader.IsDBNull(reader.GetOrdinal("TreatmentRequired")) ? null : reader.GetString(reader.GetOrdinal("TreatmentRequired")),
                OverallLength = reader.IsDBNull(reader.GetOrdinal("OverallLength")) ? null : reader.GetDecimal(reader.GetOrdinal("OverallLength")),
                OverallWidth = reader.IsDBNull(reader.GetOrdinal("OverallWidth")) ? null : reader.GetDecimal(reader.GetOrdinal("OverallWidth")),
                OverallHeight = reader.IsDBNull(reader.GetOrdinal("OverallHeight")) ? null : reader.GetDecimal(reader.GetOrdinal("OverallHeight")),
                Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? null : reader.GetDecimal(reader.GetOrdinal("Weight")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                IsLatestRevision = reader.GetBoolean(reader.GetOrdinal("IsLatestRevision")),
                PreviousRevisionId = reader.IsDBNull(reader.GetOrdinal("PreviousRevisionId")) ? null : reader.GetGuid(reader.GetOrdinal("PreviousRevisionId")),
                VersionNumber = reader.GetInt32(reader.GetOrdinal("VersionNumber")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddDrawingParameters(SqlCommand command, Drawing drawing)
        {
            command.Parameters.AddWithValue("@Id", drawing.Id);
            command.Parameters.AddWithValue("@DrawingNumber", drawing.DrawingNumber);
            command.Parameters.AddWithValue("@DrawingTitle", drawing.DrawingTitle);
            command.Parameters.AddWithValue("@ProductId", (object?)drawing.ProductId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCode", (object?)drawing.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)drawing.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionNumber", (object?)drawing.RevisionNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDate", (object?)drawing.RevisionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDescription", (object?)drawing.RevisionDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingType", (object?)drawing.DrawingType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)drawing.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@FilePath", (object?)drawing.FilePath ?? DBNull.Value);
            command.Parameters.AddWithValue("@FileName", (object?)drawing.FileName ?? DBNull.Value);
            command.Parameters.AddWithValue("@FileFormat", (object?)drawing.FileFormat ?? DBNull.Value);
            command.Parameters.AddWithValue("@FileSize", (object?)drawing.FileSize ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreparedBy", (object?)drawing.PreparedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CheckedBy", (object?)drawing.CheckedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)drawing.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalDate", (object?)drawing.ApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialSpecification", (object?)drawing.MaterialSpecification ?? DBNull.Value);
            command.Parameters.AddWithValue("@Finish", (object?)drawing.Finish ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToleranceGrade", (object?)drawing.ToleranceGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@TreatmentRequired", (object?)drawing.TreatmentRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@OverallLength", (object?)drawing.OverallLength ?? DBNull.Value);
            command.Parameters.AddWithValue("@OverallWidth", (object?)drawing.OverallWidth ?? DBNull.Value);
            command.Parameters.AddWithValue("@OverallHeight", (object?)drawing.OverallHeight ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", (object?)drawing.Weight ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", drawing.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)drawing.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLatestRevision", drawing.IsLatestRevision);
            command.Parameters.AddWithValue("@PreviousRevisionId", (object?)drawing.PreviousRevisionId ?? DBNull.Value);
            command.Parameters.AddWithValue("@VersionNumber", drawing.VersionNumber);
            command.Parameters.AddWithValue("@Remarks", (object?)drawing.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", drawing.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)drawing.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)drawing.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)drawing.UpdatedBy ?? DBNull.Value);
        }
    }
}
