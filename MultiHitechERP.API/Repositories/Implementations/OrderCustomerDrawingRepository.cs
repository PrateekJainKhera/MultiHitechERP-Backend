using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OrderCustomerDrawingRepository : IOrderCustomerDrawingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderCustomerDrawingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<OrderCustomerDrawing>> GetByOrderIdAsync(int orderId)
        {
            const string query = @"
                SELECT Id, OrderId, OriginalFileName, StoredFileName, FilePath, FileSize,
                       MimeType, DrawingType, Notes, UploadedAt, UploadedBy
                FROM Order_CustomerDrawings
                WHERE OrderId = @OrderId
                ORDER BY UploadedAt";

            var result = new List<OrderCustomerDrawing>();
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(Map(reader));
            return result;
        }

        public async Task<OrderCustomerDrawing?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, OrderId, OriginalFileName, StoredFileName, FilePath, FileSize,
                       MimeType, DrawingType, Notes, UploadedAt, UploadedBy
                FROM Order_CustomerDrawings WHERE Id = @Id";

            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<int> InsertAsync(OrderCustomerDrawing drawing)
        {
            const string query = @"
                INSERT INTO Order_CustomerDrawings
                    (OrderId, OriginalFileName, StoredFileName, FilePath, FileSize, MimeType, DrawingType, Notes, UploadedAt, UploadedBy)
                VALUES
                    (@OrderId, @OriginalFileName, @StoredFileName, @FilePath, @FileSize, @MimeType, @DrawingType, @Notes, @UploadedAt, @UploadedBy);
                SELECT SCOPE_IDENTITY();";

            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", drawing.OrderId);
            cmd.Parameters.AddWithValue("@OriginalFileName", drawing.OriginalFileName);
            cmd.Parameters.AddWithValue("@StoredFileName", drawing.StoredFileName);
            cmd.Parameters.AddWithValue("@FilePath", drawing.FilePath);
            cmd.Parameters.AddWithValue("@FileSize", (object?)drawing.FileSize ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MimeType", (object?)drawing.MimeType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DrawingType", drawing.DrawingType);
            cmd.Parameters.AddWithValue("@Notes", (object?)drawing.Notes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UploadedAt", drawing.UploadedAt);
            cmd.Parameters.AddWithValue("@UploadedBy", (object?)drawing.UploadedBy ?? DBNull.Value);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task DeleteAsync(int id)
        {
            const string query = "DELETE FROM Order_CustomerDrawings WHERE Id = @Id";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        private static OrderCustomerDrawing Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            OrderId = r.GetInt32(r.GetOrdinal("OrderId")),
            OriginalFileName = r.GetString(r.GetOrdinal("OriginalFileName")),
            StoredFileName = r.GetString(r.GetOrdinal("StoredFileName")),
            FilePath = r.GetString(r.GetOrdinal("FilePath")),
            FileSize = r.IsDBNull(r.GetOrdinal("FileSize")) ? null : r.GetInt64(r.GetOrdinal("FileSize")),
            MimeType = r.IsDBNull(r.GetOrdinal("MimeType")) ? null : r.GetString(r.GetOrdinal("MimeType")),
            DrawingType = r.GetString(r.GetOrdinal("DrawingType")),
            Notes = r.IsDBNull(r.GetOrdinal("Notes")) ? null : r.GetString(r.GetOrdinal("Notes")),
            UploadedAt = r.GetDateTime(r.GetOrdinal("UploadedAt")),
            UploadedBy = r.IsDBNull(r.GetOrdinal("UploadedBy")) ? null : r.GetString(r.GetOrdinal("UploadedBy")),
        };
    }
}
