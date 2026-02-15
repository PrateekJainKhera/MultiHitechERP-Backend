using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductChildPartDrawingRepository : IProductChildPartDrawingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductChildPartDrawingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(ProductChildPartDrawing record)
        {
            const string query = @"
                INSERT INTO Product_ChildPartDrawings
                (ProductId, ChildPartTemplateId, DrawingId, CreatedAt, CreatedBy)
                OUTPUT INSERTED.Id
                VALUES (@ProductId, @ChildPartTemplateId, @DrawingId, @CreatedAt, @CreatedBy)";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            record.CreatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@ProductId", record.ProductId);
            command.Parameters.AddWithValue("@ChildPartTemplateId", record.ChildPartTemplateId);
            command.Parameters.AddWithValue("@DrawingId", record.DrawingId);
            command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)record.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var id = (int)await command.ExecuteScalarAsync();
            record.Id = id;

            return id;
        }

        public async Task<ProductChildPartDrawing?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Product_ChildPartDrawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToModel(reader) : null;
        }

        public async Task<IEnumerable<ProductChildPartDrawing>> GetByProductIdAsync(int productId)
        {
            const string query = "SELECT * FROM Product_ChildPartDrawings WHERE ProductId = @ProductId ORDER BY Id";

            var records = new List<ProductChildPartDrawing>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                records.Add(MapToModel(reader));
            }

            return records;
        }

        public async Task<ProductChildPartDrawing?> GetByProductAndChildPartAsync(int productId, int childPartTemplateId)
        {
            const string query = @"
                SELECT * FROM Product_ChildPartDrawings
                WHERE ProductId = @ProductId AND ChildPartTemplateId = @ChildPartTemplateId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            command.Parameters.AddWithValue("@ChildPartTemplateId", childPartTemplateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToModel(reader) : null;
        }

        public async Task<bool> UpdateAsync(ProductChildPartDrawing record)
        {
            const string query = @"
                UPDATE Product_ChildPartDrawings SET
                    DrawingId = @DrawingId,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            record.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@Id", record.Id);
            command.Parameters.AddWithValue("@DrawingId", record.DrawingId);
            command.Parameters.AddWithValue("@UpdatedAt", record.UpdatedAt);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)record.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Product_ChildPartDrawings WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteByProductIdAsync(int productId)
        {
            const string query = "DELETE FROM Product_ChildPartDrawings WHERE ProductId = @ProductId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() >= 0; // Returns true even if 0 rows deleted
        }

        private ProductChildPartDrawing MapToModel(SqlDataReader reader)
        {
            return new ProductChildPartDrawing
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ChildPartTemplateId = reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId")),
                DrawingId = reader.GetInt32(reader.GetOrdinal("DrawingId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
