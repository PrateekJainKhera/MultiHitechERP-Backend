using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductDefaultComponentRepository : IProductDefaultComponentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductDefaultComponentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductDefaultComponent>> GetByProductIdAsync(int productId)
        {
            const string query = @"SELECT * FROM Product_DefaultComponents WHERE ProductId = @ProductId ORDER BY Id";
            var results = new List<ProductDefaultComponent>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) results.Add(Map(reader));
            return results;
        }

        public async Task SaveDefaultsAsync(int productId, IEnumerable<ProductDefaultComponent> defaults, string? updatedBy)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string deleteQuery = "DELETE FROM Product_DefaultComponents WHERE ProductId = @ProductId";
                using (var deleteCmd = new SqlCommand(deleteQuery, connection, transaction))
                {
                    deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                const string insertQuery = @"
                    INSERT INTO Product_DefaultComponents
                        (ProductId, ComponentId, ComponentName, PartNumber, NoOfPieces, UOM, Notes, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                    VALUES
                        (@ProductId, @ComponentId, @ComponentName, @PartNumber, @NoOfPieces, @UOM, @Notes, @Now, @By, @Now, @By)";

                var now = DateTime.UtcNow;
                foreach (var item in defaults)
                {
                    using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@ComponentId", item.ComponentId);
                    insertCmd.Parameters.AddWithValue("@ComponentName", (object?)item.ComponentName ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@PartNumber", (object?)item.PartNumber ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@NoOfPieces", item.NoOfPieces);
                    insertCmd.Parameters.AddWithValue("@UOM", (object?)item.UOM ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@Now", now);
                    insertCmd.Parameters.AddWithValue("@By", (object?)updatedBy ?? DBNull.Value);
                    await insertCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static ProductDefaultComponent Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            ProductId = r.GetInt32(r.GetOrdinal("ProductId")),
            ComponentId = r.GetInt32(r.GetOrdinal("ComponentId")),
            ComponentName = r.IsDBNull(r.GetOrdinal("ComponentName")) ? null : r.GetString(r.GetOrdinal("ComponentName")),
            PartNumber = r.IsDBNull(r.GetOrdinal("PartNumber")) ? null : r.GetString(r.GetOrdinal("PartNumber")),
            NoOfPieces = r.GetDecimal(r.GetOrdinal("NoOfPieces")),
            UOM = r.IsDBNull(r.GetOrdinal("UOM")) ? null : r.GetString(r.GetOrdinal("UOM")),
            Notes = r.IsDBNull(r.GetOrdinal("Notes")) ? null : r.GetString(r.GetOrdinal("Notes")),
            CreatedAt = r.GetDateTime(r.GetOrdinal("CreatedAt")),
            CreatedBy = r.IsDBNull(r.GetOrdinal("CreatedBy")) ? null : r.GetString(r.GetOrdinal("CreatedBy")),
            UpdatedAt = r.GetDateTime(r.GetOrdinal("UpdatedAt")),
            UpdatedBy = r.IsDBNull(r.GetOrdinal("UpdatedBy")) ? null : r.GetString(r.GetOrdinal("UpdatedBy")),
        };
    }
}
