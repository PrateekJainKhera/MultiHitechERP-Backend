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
    /// Repository implementation for ChildPart operations
    /// </summary>
    public class ChildPartRepository : IChildPartRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ChildPartRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ChildPart?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToChildPart(reader);

            return null;
        }

        public async Task<ChildPart?> GetByCodeAsync(string childPartCode)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE ChildPartCode = @ChildPartCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildPartCode", childPartCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToChildPart(reader);

            return null;
        }

        public async Task<IEnumerable<ChildPart>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<int> InsertAsync(ChildPart childPart)
        {
            const string query = @"
                INSERT INTO Masters_ChildParts (
                    Id, ChildPartCode, ChildPartName,
                    ProductId, ProductCode, ProductName,
                    PartType, Category, Description, Specification,
                    DrawingId, DrawingNumber, ProcessTemplateId,
                    MaterialId, MaterialCode, MaterialGrade,
                    Length, Diameter, Weight, UOM,
                    QuantityPerProduct, MakeOrBuy, PreferredSupplierId,
                    IsActive, Status, Remarks,
                    CreatedAt, CreatedBy
                )
                VALUES (
                    @Id, @ChildPartCode, @ChildPartName,
                    @ProductId, @ProductCode, @ProductName,
                    @PartType, @Category, @Description, @Specification,
                    @DrawingId, @DrawingNumber, @ProcessTemplateId,
                    @MaterialId, @MaterialCode, @MaterialGrade,
                    @Length, @Diameter, @Weight, @UOM,
                    @QuantityPerProduct, @MakeOrBuy, @PreferredSupplierId,
                    @IsActive, @Status, @Remarks,
                    @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = 0;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ChildPartCode", childPart.ChildPartCode);
            command.Parameters.AddWithValue("@ChildPartName", childPart.ChildPartName);
            command.Parameters.AddWithValue("@ProductId", (object?)childPart.ProductId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCode", (object?)childPart.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)childPart.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@PartType", (object?)childPart.PartType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)childPart.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)childPart.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specification", (object?)childPart.Specification ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingId", (object?)childPart.DrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)childPart.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)childPart.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialId", (object?)childPart.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)childPart.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)childPart.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)childPart.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)childPart.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", (object?)childPart.Weight ?? DBNull.Value);
            command.Parameters.AddWithValue("@UOM", (object?)childPart.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityPerProduct", (object?)childPart.QuantityPerProduct ?? DBNull.Value);
            command.Parameters.AddWithValue("@MakeOrBuy", (object?)childPart.MakeOrBuy ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredSupplierId", (object?)childPart.PreferredSupplierId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", childPart.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)childPart.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)childPart.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)childPart.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<bool> UpdateAsync(ChildPart childPart)
        {
            const string query = @"
                UPDATE Masters_ChildParts
                SET ChildPartCode = @ChildPartCode,
                    ChildPartName = @ChildPartName,
                    ProductId = @ProductId,
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    PartType = @PartType,
                    Category = @Category,
                    Description = @Description,
                    Specification = @Specification,
                    DrawingId = @DrawingId,
                    DrawingNumber = @DrawingNumber,
                    ProcessTemplateId = @ProcessTemplateId,
                    MaterialId = @MaterialId,
                    MaterialCode = @MaterialCode,
                    MaterialGrade = @MaterialGrade,
                    Length = @Length,
                    Diameter = @Diameter,
                    Weight = @Weight,
                    UOM = @UOM,
                    QuantityPerProduct = @QuantityPerProduct,
                    MakeOrBuy = @MakeOrBuy,
                    PreferredSupplierId = @PreferredSupplierId,
                    IsActive = @IsActive,
                    Status = @Status,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", childPart.Id);
            command.Parameters.AddWithValue("@ChildPartCode", childPart.ChildPartCode);
            command.Parameters.AddWithValue("@ChildPartName", childPart.ChildPartName);
            command.Parameters.AddWithValue("@ProductId", (object?)childPart.ProductId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCode", (object?)childPart.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)childPart.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@PartType", (object?)childPart.PartType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)childPart.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)childPart.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specification", (object?)childPart.Specification ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingId", (object?)childPart.DrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingNumber", (object?)childPart.DrawingNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessTemplateId", (object?)childPart.ProcessTemplateId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialId", (object?)childPart.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)childPart.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)childPart.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@Length", (object?)childPart.Length ?? DBNull.Value);
            command.Parameters.AddWithValue("@Diameter", (object?)childPart.Diameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", (object?)childPart.Weight ?? DBNull.Value);
            command.Parameters.AddWithValue("@UOM", (object?)childPart.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityPerProduct", (object?)childPart.QuantityPerProduct ?? DBNull.Value);
            command.Parameters.AddWithValue("@MakeOrBuy", (object?)childPart.MakeOrBuy ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredSupplierId", (object?)childPart.PreferredSupplierId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", childPart.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)childPart.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)childPart.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)childPart.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_ChildParts WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ChildPart>> GetByProductIdAsync(int productId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE ProductId = @ProductId
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByProductCodeAsync(string productCode)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE ProductCode = @ProductCode
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByMaterialIdAsync(int materialId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE MaterialId = @MaterialId
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByPartTypeAsync(string partType)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE PartType = @PartType
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PartType", partType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByCategoryAsync(string category)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE Category = @Category
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetActiveAsync()
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE IsActive = 1
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE Status = @Status
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByMakeOrBuyAsync(string makeOrBuy)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE MakeOrBuy = @MakeOrBuy
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MakeOrBuy", makeOrBuy);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByDrawingIdAsync(int drawingId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE DrawingId = @DrawingId
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrawingId", drawingId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<IEnumerable<ChildPart>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            const string query = @"
                SELECT * FROM Masters_ChildParts
                WHERE ProcessTemplateId = @ProcessTemplateId
                ORDER BY ChildPartCode";

            var childParts = new List<ChildPart>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessTemplateId", processTemplateId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                childParts.Add(MapToChildPart(reader));
            }

            return childParts;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Masters_ChildParts
                SET Status = @Status
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // Mapping Method
        private static ChildPart MapToChildPart(IDataReader reader)
        {
            return new ChildPart
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ChildPartCode = reader.GetString(reader.GetOrdinal("ChildPartCode")),
                ChildPartName = reader.GetString(reader.GetOrdinal("ChildPartName")),
                ProductId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? null : reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                PartType = reader.IsDBNull(reader.GetOrdinal("PartType")) ? null : reader.GetString(reader.GetOrdinal("PartType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Specification = reader.IsDBNull(reader.GetOrdinal("Specification")) ? null : reader.GetString(reader.GetOrdinal("Specification")),
                DrawingId = reader.IsDBNull(reader.GetOrdinal("DrawingId")) ? null : reader.GetInt32(reader.GetOrdinal("DrawingId")),
                DrawingNumber = reader.IsDBNull(reader.GetOrdinal("DrawingNumber")) ? null : reader.GetString(reader.GetOrdinal("DrawingNumber")),
                ProcessTemplateId = reader.IsDBNull(reader.GetOrdinal("ProcessTemplateId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessTemplateId")),
                MaterialId = reader.IsDBNull(reader.GetOrdinal("MaterialId")) ? null : reader.GetInt32(reader.GetOrdinal("MaterialId")),
                MaterialCode = reader.IsDBNull(reader.GetOrdinal("MaterialCode")) ? null : reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade")) ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? null : reader.GetDecimal(reader.GetOrdinal("Length")),
                Diameter = reader.IsDBNull(reader.GetOrdinal("Diameter")) ? null : reader.GetDecimal(reader.GetOrdinal("Diameter")),
                Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? null : reader.GetDecimal(reader.GetOrdinal("Weight")),
                UOM = reader.IsDBNull(reader.GetOrdinal("UOM")) ? null : reader.GetString(reader.GetOrdinal("UOM")),
                QuantityPerProduct = reader.IsDBNull(reader.GetOrdinal("QuantityPerProduct")) ? null : reader.GetInt32(reader.GetOrdinal("QuantityPerProduct")),
                MakeOrBuy = reader.IsDBNull(reader.GetOrdinal("MakeOrBuy")) ? null : reader.GetString(reader.GetOrdinal("MakeOrBuy")),
                PreferredSupplierId = reader.IsDBNull(reader.GetOrdinal("PreferredSupplierId")) ? null : reader.GetInt32(reader.GetOrdinal("PreferredSupplierId")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
