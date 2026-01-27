using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Products WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProduct(reader) : null;
        }

        public async Task<Product?> GetByPartCodeAsync(string partCode)
        {
            const string query = "SELECT * FROM Masters_Products WHERE PartCode = @PartCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartCode", partCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProduct(reader) : null;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Products ORDER BY ModelName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<int> InsertAsync(Product product)
        {
            const string query = @"
                INSERT INTO Masters_Products (
                    PartCode, CustomerName, ModelName, RollerType,
                    Diameter, Length, MaterialGrade, SurfaceFinish, Hardness,
                    DrawingNo, RevisionNo, RevisionDate, NumberOfTeeth,
                    ProcessTemplateId, CreatedAt, CreatedBy, UpdatedAt
                ) VALUES (
                    @PartCode, @CustomerName, @ModelName, @RollerType,
                    @Diameter, @Length, @MaterialGrade, @SurfaceFinish, @Hardness,
                    @DrawingNo, @RevisionNo, @RevisionDate, @NumberOfTeeth,
                    @ProcessTemplateId, @CreatedAt, @CreatedBy, @UpdatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@PartCode", product.PartCode);
            command.Parameters.AddWithValue("@CustomerName", (object?)product.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelName", product.ModelName);
            command.Parameters.AddWithValue("@RollerType", product.RollerType);
            command.Parameters.AddWithValue("@Diameter", product.Diameter);
            command.Parameters.AddWithValue("@Length", product.Length);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)product.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@SurfaceFinish", (object?)product.SurfaceFinish ?? DBNull.Value);
            command.Parameters.AddWithValue("@Hardness", (object?)product.Hardness ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNo", (object?)product.DrawingNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionNo", (object?)product.RevisionNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDate", (object?)product.RevisionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfTeeth", (object?)product.NumberOfTeeth ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", product.ProcessTemplateId);
            command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", product.CreatedBy);
            command.Parameters.AddWithValue("@UpdatedAt", product.UpdatedAt);

            await connection.OpenAsync();
            var productId = (int)await command.ExecuteScalarAsync();
            product.Id = productId;

            return productId;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            const string query = @"
                UPDATE Masters_Products SET
                    PartCode = @PartCode,
                    CustomerName = @CustomerName,
                    ModelName = @ModelName,
                    RollerType = @RollerType,
                    Diameter = @Diameter,
                    Length = @Length,
                    MaterialGrade = @MaterialGrade,
                    SurfaceFinish = @SurfaceFinish,
                    Hardness = @Hardness,
                    DrawingNo = @DrawingNo,
                    RevisionNo = @RevisionNo,
                    RevisionDate = @RevisionDate,
                    NumberOfTeeth = @NumberOfTeeth,
                    ProcessTemplateId = @ProcessTemplateId,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            product.UpdatedAt = DateTime.UtcNow;

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@PartCode", product.PartCode);
            command.Parameters.AddWithValue("@CustomerName", (object?)product.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelName", product.ModelName);
            command.Parameters.AddWithValue("@RollerType", product.RollerType);
            command.Parameters.AddWithValue("@Diameter", product.Diameter);
            command.Parameters.AddWithValue("@Length", product.Length);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)product.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@SurfaceFinish", (object?)product.SurfaceFinish ?? DBNull.Value);
            command.Parameters.AddWithValue("@Hardness", (object?)product.Hardness ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNo", (object?)product.DrawingNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionNo", (object?)product.RevisionNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDate", (object?)product.RevisionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfTeeth", (object?)product.NumberOfTeeth ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", product.ProcessTemplateId);
            command.Parameters.AddWithValue("@UpdatedAt", product.UpdatedAt);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Products WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            const string query = @"
                SELECT * FROM Masters_Products
                WHERE ModelName LIKE '%' + @Name + '%'
                   OR PartCode LIKE '%' + @Name + '%'
                   OR CustomerName LIKE '%' + @Name + '%'
                ORDER BY ModelName";

            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByRollerTypeAsync(string rollerType)
        {
            const string query = "SELECT * FROM Masters_Products WHERE RollerType = @RollerType ORDER BY ModelName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RollerType", rollerType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<bool> ExistsAsync(string partCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Products WHERE PartCode = @PartCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartCode", partCode);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync() > 0;
        }

        private Product MapToProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                PartCode = reader.GetString(reader.GetOrdinal("PartCode")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                ModelName = reader.GetString(reader.GetOrdinal("ModelName")),
                RollerType = reader.GetString(reader.GetOrdinal("RollerType")),
                Diameter = reader.GetDecimal(reader.GetOrdinal("Diameter")),
                Length = reader.GetDecimal(reader.GetOrdinal("Length")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade")) ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                SurfaceFinish = reader.IsDBNull(reader.GetOrdinal("SurfaceFinish")) ? null : reader.GetString(reader.GetOrdinal("SurfaceFinish")),
                Hardness = reader.IsDBNull(reader.GetOrdinal("Hardness")) ? null : reader.GetString(reader.GetOrdinal("Hardness")),
                DrawingNo = reader.IsDBNull(reader.GetOrdinal("DrawingNo")) ? null : reader.GetString(reader.GetOrdinal("DrawingNo")),
                RevisionNo = reader.IsDBNull(reader.GetOrdinal("RevisionNo")) ? null : reader.GetString(reader.GetOrdinal("RevisionNo")),
                RevisionDate = reader.IsDBNull(reader.GetOrdinal("RevisionDate")) ? null : reader.GetString(reader.GetOrdinal("RevisionDate")),
                NumberOfTeeth = reader.IsDBNull(reader.GetOrdinal("NumberOfTeeth")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfTeeth")),
                ProcessTemplateId = reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }
    }
}
