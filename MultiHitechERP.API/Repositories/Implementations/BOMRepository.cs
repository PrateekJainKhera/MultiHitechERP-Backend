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
    /// Repository implementation for BOM (Bill of Materials) operations
    /// </summary>
    public class BOMRepository : IBOMRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BOMRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<BOM?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToBOM(reader);

            return null;
        }

        public async Task<BOM?> GetByBOMNoAsync(string bomNo)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE BOMNo = @BOMNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMNo", bomNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToBOM(reader);

            return null;
        }

        public async Task<IEnumerable<BOM>> GetAllAsync()
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                ORDER BY CreatedAt DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        public async Task<int> InsertAsync(BOM bom)
        {
            const string query = @"
                INSERT INTO Masters_BOM (
                    Id, BOMNo, ProductId, ProductCode, ProductName,
                    RevisionNumber, RevisionDate, IsLatestRevision,
                    BOMType, BaseQuantity, BaseUOM,
                    IsActive, Status, ApprovedBy, ApprovalDate,
                    Remarks, CreatedAt, CreatedBy
                )
                VALUES (
                    @Id, @BOMNo, @ProductId, @ProductCode, @ProductName,
                    @RevisionNumber, @RevisionDate, @IsLatestRevision,
                    @BOMType, @BaseQuantity, @BaseUOM,
                    @IsActive, @Status, @ApprovedBy, @ApprovalDate,
                    @Remarks, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = 0;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@BOMNo", bom.BOMNo);
            command.Parameters.AddWithValue("@ProductId", bom.ProductId);
            command.Parameters.AddWithValue("@ProductCode", (object?)bom.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)bom.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionNumber", (object?)bom.RevisionNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDate", (object?)bom.RevisionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLatestRevision", bom.IsLatestRevision);
            command.Parameters.AddWithValue("@BOMType", (object?)bom.BOMType ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseQuantity", (object?)bom.BaseQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseUOM", (object?)bom.BaseUOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", bom.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)bom.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)bom.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalDate", (object?)bom.ApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)bom.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@CreatedBy", (object?)bom.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<bool> UpdateAsync(BOM bom)
        {
            const string query = @"
                UPDATE Masters_BOM
                SET BOMNo = @BOMNo,
                    ProductId = @ProductId,
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    RevisionNumber = @RevisionNumber,
                    RevisionDate = @RevisionDate,
                    IsLatestRevision = @IsLatestRevision,
                    BOMType = @BOMType,
                    BaseQuantity = @BaseQuantity,
                    BaseUOM = @BaseUOM,
                    IsActive = @IsActive,
                    Status = @Status,
                    ApprovedBy = @ApprovedBy,
                    ApprovalDate = @ApprovalDate,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", bom.Id);
            command.Parameters.AddWithValue("@BOMNo", bom.BOMNo);
            command.Parameters.AddWithValue("@ProductId", bom.ProductId);
            command.Parameters.AddWithValue("@ProductCode", (object?)bom.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductName", (object?)bom.ProductName ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionNumber", (object?)bom.RevisionNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@RevisionDate", (object?)bom.RevisionDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsLatestRevision", bom.IsLatestRevision);
            command.Parameters.AddWithValue("@BOMType", (object?)bom.BOMType ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseQuantity", (object?)bom.BaseQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseUOM", (object?)bom.BaseUOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", bom.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)bom.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)bom.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalDate", (object?)bom.ApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)bom.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)bom.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_BOM WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<BOM>> GetByProductIdAsync(int productId)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE ProductId = @ProductId
                ORDER BY RevisionDate DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        public async Task<BOM?> GetLatestRevisionAsync(int productId)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE ProductId = @ProductId
                  AND IsLatestRevision = 1
                ORDER BY RevisionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToBOM(reader);

            return null;
        }

        public async Task<IEnumerable<BOM>> GetByProductCodeAsync(string productCode)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE ProductCode = @ProductCode
                ORDER BY RevisionDate DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductCode", productCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        public async Task<IEnumerable<BOM>> GetActiveAsync()
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE IsActive = 1
                ORDER BY CreatedAt DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        public async Task<IEnumerable<BOM>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE Status = @Status
                ORDER BY CreatedAt DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        public async Task<IEnumerable<BOM>> GetByBOMTypeAsync(string bomType)
        {
            const string query = @"
                SELECT * FROM Masters_BOM
                WHERE BOMType = @BOMType
                ORDER BY CreatedAt DESC";

            var boms = new List<BOM>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMType", bomType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                boms.Add(MapToBOM(reader));
            }

            return boms;
        }

        // BOM Item Operations
        public async Task<IEnumerable<BOMItem>> GetBOMItemsAsync(int bomId)
        {
            const string query = @"
                SELECT * FROM Masters_BOMItems
                WHERE BOMId = @BOMId
                ORDER BY LineNo";

            var items = new List<BOMItem>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMId", bomId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(MapToBOMItem(reader));
            }

            return items;
        }

        public async Task<int> InsertBOMItemAsync(BOMItem item)
        {
            const string query = @"
                INSERT INTO Masters_BOMItems (
                    Id, BOMId, LineNo, ItemType,
                    MaterialId, MaterialCode, MaterialName,
                    ChildPartId, ChildPartCode, ChildPartName,
                    QuantityRequired, UOM,
                    LengthRequiredMM, ScrapPercentage, ScrapQuantity,
                    WastageMM, NetQuantityRequired,
                    ReferenceDesignator, Notes, CreatedAt
                )
                VALUES (
                    @Id, @BOMId, @LineNo, @ItemType,
                    @MaterialId, @MaterialCode, @MaterialName,
                    @ChildPartId, @ChildPartCode, @ChildPartName,
                    @QuantityRequired, @UOM,
                    @LengthRequiredMM, @ScrapPercentage, @ScrapQuantity,
                    @WastageMM, @NetQuantityRequired,
                    @ReferenceDesignator, @Notes, @CreatedAt
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var id = 0;
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@BOMId", item.BOMId);
            command.Parameters.AddWithValue("@LineNo", item.LineNo);
            command.Parameters.AddWithValue("@ItemType", item.ItemType);
            command.Parameters.AddWithValue("@MaterialId", (object?)item.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)item.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)item.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartId", (object?)item.ChildPartId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartCode", (object?)item.ChildPartCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartName", (object?)item.ChildPartName ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityRequired", item.QuantityRequired);
            command.Parameters.AddWithValue("@UOM", (object?)item.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@LengthRequiredMM", (object?)item.LengthRequiredMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScrapPercentage", (object?)item.ScrapPercentage ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScrapQuantity", (object?)item.ScrapQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@WastageMM", (object?)item.WastageMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@NetQuantityRequired", (object?)item.NetQuantityRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReferenceDesignator", (object?)item.ReferenceDesignator ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        public async Task<bool> UpdateBOMItemAsync(BOMItem item)
        {
            const string query = @"
                UPDATE Masters_BOMItems
                SET LineNo = @LineNo,
                    ItemType = @ItemType,
                    MaterialId = @MaterialId,
                    MaterialCode = @MaterialCode,
                    MaterialName = @MaterialName,
                    ChildPartId = @ChildPartId,
                    ChildPartCode = @ChildPartCode,
                    ChildPartName = @ChildPartName,
                    QuantityRequired = @QuantityRequired,
                    UOM = @UOM,
                    LengthRequiredMM = @LengthRequiredMM,
                    ScrapPercentage = @ScrapPercentage,
                    ScrapQuantity = @ScrapQuantity,
                    WastageMM = @WastageMM,
                    NetQuantityRequired = @NetQuantityRequired,
                    ReferenceDesignator = @ReferenceDesignator,
                    Notes = @Notes
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@LineNo", item.LineNo);
            command.Parameters.AddWithValue("@ItemType", item.ItemType);
            command.Parameters.AddWithValue("@MaterialId", (object?)item.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)item.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)item.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartId", (object?)item.ChildPartId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartCode", (object?)item.ChildPartCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChildPartName", (object?)item.ChildPartName ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityRequired", item.QuantityRequired);
            command.Parameters.AddWithValue("@UOM", (object?)item.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@LengthRequiredMM", (object?)item.LengthRequiredMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScrapPercentage", (object?)item.ScrapPercentage ?? DBNull.Value);
            command.Parameters.AddWithValue("@ScrapQuantity", (object?)item.ScrapQuantity ?? DBNull.Value);
            command.Parameters.AddWithValue("@WastageMM", (object?)item.WastageMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@NetQuantityRequired", (object?)item.NetQuantityRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReferenceDesignator", (object?)item.ReferenceDesignator ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteBOMItemAsync(int itemId)
        {
            const string query = "DELETE FROM Masters_BOMItems WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", itemId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAllBOMItemsAsync(int bomId)
        {
            const string query = "DELETE FROM Masters_BOMItems WHERE BOMId = @BOMId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMId", bomId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<BOMItem>> GetMaterialItemsAsync(int bomId)
        {
            const string query = @"
                SELECT * FROM Masters_BOMItems
                WHERE BOMId = @BOMId
                  AND ItemType = 'Material'
                ORDER BY LineNo";

            var items = new List<BOMItem>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMId", bomId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(MapToBOMItem(reader));
            }

            return items;
        }

        public async Task<IEnumerable<BOMItem>> GetChildPartItemsAsync(int bomId)
        {
            const string query = @"
                SELECT * FROM Masters_BOMItems
                WHERE BOMId = @BOMId
                  AND ItemType = 'Child Part'
                ORDER BY LineNo";

            var items = new List<BOMItem>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMId", bomId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(MapToBOMItem(reader));
            }

            return items;
        }

        public async Task<IEnumerable<BOMItem>> GetItemsByTypeAsync(int bomId, string itemType)
        {
            const string query = @"
                SELECT * FROM Masters_BOMItems
                WHERE BOMId = @BOMId
                  AND ItemType = @ItemType
                ORDER BY LineNo";

            var items = new List<BOMItem>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BOMId", bomId);
            command.Parameters.AddWithValue("@ItemType", itemType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(MapToBOMItem(reader));
            }

            return items;
        }

        public async Task<bool> ApproveBOMAsync(int id, string approvedBy, DateTime approvalDate)
        {
            const string query = @"
                UPDATE Masters_BOM
                SET ApprovedBy = @ApprovedBy,
                    ApprovalDate = @ApprovalDate,
                    Status = 'Approved'
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ApprovedBy", approvedBy);
            command.Parameters.AddWithValue("@ApprovalDate", approvalDate);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Masters_BOM
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

        public async Task<bool> MarkAsNonLatestAsync(int id)
        {
            const string query = @"
                UPDATE Masters_BOM
                SET IsLatestRevision = 0
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<int> GetNextRevisionNumberAsync(int productId)
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(RevisionNumber AS INT)), 0) + 1
                FROM Masters_BOM
                WHERE ProductId = @ProductId
                  AND ISNUMERIC(RevisionNumber) = 1";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 1;
        }

        // Mapping Methods
        private static BOM MapToBOM(IDataReader reader)
        {
            return new BOM
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                BOMNo = reader.GetString(reader.GetOrdinal("BOMNo")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductCode = reader.IsDBNull(reader.GetOrdinal("ProductCode")) ? null : reader.GetString(reader.GetOrdinal("ProductCode")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                RevisionNumber = reader.IsDBNull(reader.GetOrdinal("RevisionNumber")) ? null : reader.GetString(reader.GetOrdinal("RevisionNumber")),
                RevisionDate = reader.IsDBNull(reader.GetOrdinal("RevisionDate")) ? null : reader.GetDateTime(reader.GetOrdinal("RevisionDate")),
                IsLatestRevision = reader.GetBoolean(reader.GetOrdinal("IsLatestRevision")),
                BOMType = reader.IsDBNull(reader.GetOrdinal("BOMType")) ? null : reader.GetString(reader.GetOrdinal("BOMType")),
                BaseQuantity = reader.IsDBNull(reader.GetOrdinal("BaseQuantity")) ? null : reader.GetDecimal(reader.GetOrdinal("BaseQuantity")),
                BaseUOM = reader.IsDBNull(reader.GetOrdinal("BaseUOM")) ? null : reader.GetString(reader.GetOrdinal("BaseUOM")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovalDate = reader.IsDBNull(reader.GetOrdinal("ApprovalDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovalDate")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static BOMItem MapToBOMItem(IDataReader reader)
        {
            return new BOMItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                BOMId = reader.GetInt32(reader.GetOrdinal("BOMId")),
                LineNo = reader.GetInt32(reader.GetOrdinal("LineNo")),
                ItemType = reader.GetString(reader.GetOrdinal("ItemType")),
                MaterialId = reader.IsDBNull(reader.GetOrdinal("MaterialId")) ? null : reader.GetInt32(reader.GetOrdinal("MaterialId")),
                MaterialCode = reader.IsDBNull(reader.GetOrdinal("MaterialCode")) ? null : reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialName = reader.IsDBNull(reader.GetOrdinal("MaterialName")) ? null : reader.GetString(reader.GetOrdinal("MaterialName")),
                ChildPartId = reader.IsDBNull(reader.GetOrdinal("ChildPartId")) ? null : reader.GetInt32(reader.GetOrdinal("ChildPartId")),
                ChildPartCode = reader.IsDBNull(reader.GetOrdinal("ChildPartCode")) ? null : reader.GetString(reader.GetOrdinal("ChildPartCode")),
                ChildPartName = reader.IsDBNull(reader.GetOrdinal("ChildPartName")) ? null : reader.GetString(reader.GetOrdinal("ChildPartName")),
                QuantityRequired = reader.GetDecimal(reader.GetOrdinal("QuantityRequired")),
                UOM = reader.IsDBNull(reader.GetOrdinal("UOM")) ? null : reader.GetString(reader.GetOrdinal("UOM")),
                LengthRequiredMM = reader.IsDBNull(reader.GetOrdinal("LengthRequiredMM")) ? null : reader.GetDecimal(reader.GetOrdinal("LengthRequiredMM")),
                ScrapPercentage = reader.IsDBNull(reader.GetOrdinal("ScrapPercentage")) ? null : reader.GetDecimal(reader.GetOrdinal("ScrapPercentage")),
                ScrapQuantity = reader.IsDBNull(reader.GetOrdinal("ScrapQuantity")) ? null : reader.GetDecimal(reader.GetOrdinal("ScrapQuantity")),
                WastageMM = reader.IsDBNull(reader.GetOrdinal("WastageMM")) ? null : reader.GetDecimal(reader.GetOrdinal("WastageMM")),
                NetQuantityRequired = reader.IsDBNull(reader.GetOrdinal("NetQuantityRequired")) ? null : reader.GetDecimal(reader.GetOrdinal("NetQuantityRequired")),
                ReferenceDesignator = reader.IsDBNull(reader.GetOrdinal("ReferenceDesignator")) ? null : reader.GetString(reader.GetOrdinal("ReferenceDesignator")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
