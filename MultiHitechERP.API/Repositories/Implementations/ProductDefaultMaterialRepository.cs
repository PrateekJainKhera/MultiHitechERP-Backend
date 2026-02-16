using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductDefaultMaterialRepository : IProductDefaultMaterialRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductDefaultMaterialRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductDefaultMaterial>> GetByProductIdAsync(int productId)
        {
            const string query = @"
                SELECT *
                FROM Product_DefaultMaterials
                WHERE ProductId = @ProductId
                ORDER BY ChildPartTemplateId, Id";

            var results = new List<ProductDefaultMaterial>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(MapToModel(reader));
            }

            return results;
        }

        public async Task SaveDefaultsAsync(int productId, IEnumerable<ProductDefaultMaterial> defaults, string? updatedBy)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Delete all existing defaults for this product
                const string deleteQuery = "DELETE FROM Product_DefaultMaterials WHERE ProductId = @ProductId";
                using var deleteCmd = new SqlCommand(deleteQuery, connection, transaction);
                deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                await deleteCmd.ExecuteNonQueryAsync();

                // Insert new defaults
                const string insertQuery = @"
                    INSERT INTO Product_DefaultMaterials
                        (ProductId, ChildPartTemplateId, RawMaterialId, RawMaterialName, MaterialGrade,
                         RequiredQuantity, Unit, WastageMM, Notes, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                    VALUES
                        (@ProductId, @ChildPartTemplateId, @RawMaterialId, @RawMaterialName, @MaterialGrade,
                         @RequiredQuantity, @Unit, @WastageMM, @Notes, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

                var now = DateTime.UtcNow;
                foreach (var item in defaults)
                {
                    using var insertCmd = new SqlCommand(insertQuery, connection, transaction);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@ChildPartTemplateId", (object?)item.ChildPartTemplateId ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@RawMaterialId", (object?)item.RawMaterialId ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@RawMaterialName", item.RawMaterialName);
                    insertCmd.Parameters.AddWithValue("@MaterialGrade", item.MaterialGrade);
                    insertCmd.Parameters.AddWithValue("@RequiredQuantity", item.RequiredQuantity);
                    insertCmd.Parameters.AddWithValue("@Unit", item.Unit);
                    insertCmd.Parameters.AddWithValue("@WastageMM", item.WastageMM);
                    insertCmd.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@CreatedAt", now);
                    insertCmd.Parameters.AddWithValue("@CreatedBy", (object?)updatedBy ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@UpdatedAt", now);
                    insertCmd.Parameters.AddWithValue("@UpdatedBy", (object?)updatedBy ?? DBNull.Value);
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

        private static ProductDefaultMaterial MapToModel(SqlDataReader reader)
        {
            return new ProductDefaultMaterial
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ChildPartTemplateId = reader.IsDBNull(reader.GetOrdinal("ChildPartTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartTemplateId")),
                RawMaterialId = reader.IsDBNull(reader.GetOrdinal("RawMaterialId")) ? null : reader.GetInt32(reader.GetOrdinal("RawMaterialId")),
                RawMaterialName = reader.GetString(reader.GetOrdinal("RawMaterialName")),
                MaterialGrade = reader.GetString(reader.GetOrdinal("MaterialGrade")),
                RequiredQuantity = reader.GetDecimal(reader.GetOrdinal("RequiredQuantity")),
                Unit = reader.GetString(reader.GetOrdinal("Unit")),
                WastageMM = reader.GetDecimal(reader.GetOrdinal("WastageMM")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
            };
        }
    }
}
