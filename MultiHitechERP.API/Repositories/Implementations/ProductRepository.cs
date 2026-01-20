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
    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Masters_Products WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProduct(reader) : null;
        }

        public async Task<Product?> GetByProductCodeAsync(string productCode)
        {
            const string query = "SELECT * FROM Masters_Products WHERE ProductCode = @ProductCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProduct(reader) : null;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Products ORDER BY ProductName";
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

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            const string query = "SELECT * FROM Masters_Products WHERE IsActive = 1 ORDER BY ProductName";
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

        public async Task<Guid> InsertAsync(Product product)
        {
            const string query = @"
                INSERT INTO Masters_Products
                (Id, ProductCode, ProductName, Category, SubCategory, ProductType, Specification, Description, HSNCode,
                 Length, Width, Height, Diameter, Weight, UOM, DrawingId, DrawingNumber, BOMId, ProcessRouteId,
                 StandardCost, SellingPrice, MaterialGrade, MaterialSpecification, StandardBatchSize, MinOrderQuantity,
                 LeadTimeDays, IsActive, Status, Remarks, CreatedAt, CreatedBy)
                VALUES
                (@Id, @ProductCode, @ProductName, @Category, @SubCategory, @ProductType, @Specification, @Description, @HSNCode,
                 @Length, @Width, @Height, @Diameter, @Weight, @UOM, @DrawingId, @DrawingNumber, @BOMId, @ProcessRouteId,
                 @StandardCost, @SellingPrice, @MaterialGrade, @MaterialSpecification, @StandardBatchSize, @MinOrderQuantity,
                 @LeadTimeDays, @IsActive, @Status, @Remarks, @CreatedAt, @CreatedBy)";

            var productId = Guid.NewGuid();
            product.Id = productId;
            product.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddProductParameters(command, product);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return productId;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            const string query = @"
                UPDATE Masters_Products SET
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    Category = @Category,
                    SubCategory = @SubCategory,
                    ProductType = @ProductType,
                    Specification = @Specification,
                    Description = @Description,
                    HSNCode = @HSNCode,
                    Length = @Length,
                    Width = @Width,
                    Height = @Height,
                    Diameter = @Diameter,
                    Weight = @Weight,
                    UOM = @UOM,
                    DrawingId = @DrawingId,
                    DrawingNumber = @DrawingNumber,
                    BOMId = @BOMId,
                    ProcessRouteId = @ProcessRouteId,
                    StandardCost = @StandardCost,
                    SellingPrice = @SellingPrice,
                    MaterialGrade = @MaterialGrade,
                    MaterialSpecification = @MaterialSpecification,
                    StandardBatchSize = @StandardBatchSize,
                    MinOrderQuantity = @MinOrderQuantity,
                    LeadTimeDays = @LeadTimeDays,
                    IsActive = @IsActive,
                    Status = @Status,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            product.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddProductParameters(command, product);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Products WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ActivateAsync(Guid id)
        {
            const string query = "UPDATE Masters_Products SET IsActive = 1, Status = 'Active', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            const string query = "UPDATE Masters_Products SET IsActive = 0, Status = 'Inactive', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
        {
            const string query = "SELECT * FROM Masters_Products WHERE ProductName LIKE @Name ORDER BY ProductName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", $"%{name}%");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            const string query = "SELECT * FROM Masters_Products WHERE Category = @Category ORDER BY ProductName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByProductTypeAsync(string productType)
        {
            const string query = "SELECT * FROM Masters_Products WHERE ProductType = @ProductType ORDER BY ProductName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductType", productType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<IEnumerable<Product>> GetByDrawingIdAsync(Guid drawingId)
        {
            const string query = "SELECT * FROM Masters_Products WHERE DrawingId = @DrawingId ORDER BY ProductName";
            var products = new List<Product>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingId", drawingId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<bool> ExistsAsync(string productCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Products WHERE ProductCode = @ProductCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        private static Product MapToProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                SubCategory = reader.IsDBNull(reader.GetOrdinal("SubCategory")) ? null : reader.GetString(reader.GetOrdinal("SubCategory")),
                ProductType = reader.IsDBNull(reader.GetOrdinal("ProductType")) ? null : reader.GetString(reader.GetOrdinal("ProductType")),
                Specification = reader.IsDBNull(reader.GetOrdinal("Specification")) ? null : reader.GetString(reader.GetOrdinal("Specification")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                HSNCode = reader.IsDBNull(reader.GetOrdinal("HSNCode")) ? null : reader.GetString(reader.GetOrdinal("HSNCode")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? null : reader.GetDecimal(reader.GetOrdinal("Width")),
                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? null : reader.GetDecimal(reader.GetOrdinal("Height")),
                Diameter = reader.IsDBNull(reader.GetOrdinal("Diameter")) ? null : reader.GetDecimal(reader.GetOrdinal("Diameter")),
                Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? null : reader.GetDecimal(reader.GetOrdinal("Weight")),
                UOM = reader.IsDBNull(reader.GetOrdinal("UOM")) ? null : reader.GetString(reader.GetOrdinal("UOM")),
                DrawingId = reader.IsDBNull(reader.GetOrdinal("DrawingId")) ? null : reader.GetGuid(reader.GetOrdinal("DrawingId")),
                DrawingNumber = reader.IsDBNull(reader.GetOrdinal("DrawingNumber")) ? null : reader.GetString(reader.GetOrdinal("DrawingNumber")),
                BOMId = reader.IsDBNull(reader.GetOrdinal("BOMId")) ? null : reader.GetGuid(reader.GetOrdinal("BOMId")),
                ProcessRouteId = reader.IsDBNull(reader.GetOrdinal("ProcessRouteId")) ? null : reader.GetGuid(reader.GetOrdinal("ProcessRouteId")),
                StandardCost = reader.IsDBNull(reader.GetOrdinal("StandardCost")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardCost")),
                SellingPrice = reader.IsDBNull(reader.GetOrdinal("SellingPrice")) ? null : reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade")) ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                MaterialSpecification = reader.IsDBNull(reader.GetOrdinal("MaterialSpecification")) ? null : reader.GetString(reader.GetOrdinal("MaterialSpecification")),
                StandardBatchSize = reader.IsDBNull(reader.GetOrdinal("StandardBatchSize")) ? null : reader.GetInt32(reader.GetOrdinal("StandardBatchSize")),
                MinOrderQuantity = reader.IsDBNull(reader.GetOrdinal("MinOrderQuantity")) ? null : reader.GetInt32(reader.GetOrdinal("MinOrderQuantity")),
                LeadTimeDays = reader.IsDBNull(reader.GetOrdinal("LeadTimeDays")) ? null : reader.GetInt32(reader.GetOrdinal("LeadTimeDays")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddProductParameters(SqlCommand command, Product product)
        {
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
            command.Parameters.AddWithValue("@ProductName", product.ProductName);
            command.Parameters.AddWithValue("@Category", (object?)product.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubCategory", (object?)product.SubCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductType", (object?)product.ProductType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specification", (object?)product.Specification ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@HSNCode", (object?)product.HSNCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)product.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Width", (object?)product.Width ?? DBNull.Value);
            command.Parameters.AddWithValue("@Height", (object?)product.Height ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)product.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", (object?)product.Weight ?? DBNull.Value);
            command.Parameters.AddWithValue("@UOM", (object?)product.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingId", (object?)product.DrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)product.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@BOMId", (object?)product.BOMId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessRouteId", (object?)product.ProcessRouteId ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCost", (object?)product.StandardCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@SellingPrice", (object?)product.SellingPrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)product.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialSpecification", (object?)product.MaterialSpecification ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardBatchSize", (object?)product.StandardBatchSize ?? DBNull.Value);
            command.Parameters.AddWithValue("@MinOrderQuantity", (object?)product.MinOrderQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@LeadTimeDays", (object?)product.LeadTimeDays ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", product.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)product.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)product.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)product.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)product.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)product.UpdatedBy ?? DBNull.Value);
        }
    }
}
