using Dapper;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OpeningStockRepository : IOpeningStockRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OpeningStockRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private SqlConnection GetConnection() =>
            (SqlConnection)_connectionFactory.CreateConnection();

        public async Task<string> GenerateEntryNoAsync()
        {
            var prefix = $"OS-{DateTime.Now:yyyyMM}-";
            var sql = "SELECT ISNULL(MAX(CAST(RIGHT(EntryNo, 3) AS INT)), 0) FROM Stores_OpeningStockEntries WHERE EntryNo LIKE @Prefix + '%'";
            using var conn = GetConnection();
            await conn.OpenAsync();
            var count = await conn.ExecuteScalarAsync<int>(sql, new { Prefix = prefix });
            return $"{prefix}{(count + 1):D3}";
        }

        public async Task<int> CreateAsync(OpeningStockEntry entry, IEnumerable<OpeningStockItem> items)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                var insertEntrySql = @"
                    INSERT INTO Stores_OpeningStockEntries
                        (EntryNo, EntryDate, Status, Remarks, CreatedAt, CreatedBy)
                    VALUES
                        (@EntryNo, @EntryDate, 'Draft', @Remarks, GETDATE(), @CreatedBy);
                    SELECT SCOPE_IDENTITY();";
                var entryId = await conn.ExecuteScalarAsync<int>(insertEntrySql, entry, tx);

                var insertItemSql = @"
                    INSERT INTO Stores_OpeningStockItems
                        (EntryId, SequenceNo, ItemType,
                         MaterialId, MaterialName, Grade, MaterialType,
                         Diameter, OuterDiameter, InnerDiameter, Width, Thickness,
                         MaterialDensity, TotalWeightKG, CalculatedLengthMM, WeightPerMeterKG,
                         NumberOfPieces, LengthPerPieceMM, WarehouseId,
                         ComponentId, ComponentName, PartNumber, Quantity, UOM,
                         UnitCost, LineTotal, Remarks, SortOrder)
                    VALUES
                        (@EntryId, @SequenceNo, @ItemType,
                         @MaterialId, @MaterialName, @Grade, @MaterialType,
                         @Diameter, @OuterDiameter, @InnerDiameter, @Width, @Thickness,
                         @MaterialDensity, @TotalWeightKG, @CalculatedLengthMM, @WeightPerMeterKG,
                         @NumberOfPieces, @LengthPerPieceMM, @WarehouseId,
                         @ComponentId, @ComponentName, @PartNumber, @Quantity, @UOM,
                         @UnitCost, @LineTotal, @Remarks, @SortOrder)";

                foreach (var item in items)
                {
                    item.EntryId = entryId;
                    await conn.ExecuteAsync(insertItemSql, item, tx);
                }

                tx.Commit();
                return entryId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<OpeningStockSummaryResponse>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    e.Id, e.EntryNo, e.EntryDate, e.Status, e.Remarks,
                    e.CreatedAt, e.CreatedBy, e.ConfirmedAt, e.ConfirmedBy,
                    ISNULL(e.TotalPieces, 0)     AS TotalPieces,
                    ISNULL(e.TotalComponents, 0) AS TotalComponents,
                    (SELECT COUNT(1) FROM Stores_OpeningStockItems WHERE EntryId = e.Id AND ItemType = 'RawMaterial')  AS RawMaterialLines,
                    (SELECT COUNT(1) FROM Stores_OpeningStockItems WHERE EntryId = e.Id AND ItemType = 'Component')    AS ComponentLines
                FROM Stores_OpeningStockEntries e
                ORDER BY e.CreatedAt DESC";
            using var conn = GetConnection();
            return await conn.QueryAsync<OpeningStockSummaryResponse>(sql);
        }

        public async Task<OpeningStockDetailResponse?> GetByIdAsync(int id)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();

            var entrySql = @"
                SELECT
                    e.Id, e.EntryNo, e.EntryDate, e.Status, e.Remarks,
                    e.CreatedAt, e.CreatedBy, e.ConfirmedAt, e.ConfirmedBy,
                    e.TotalPieces, e.TotalComponents
                FROM Stores_OpeningStockEntries e
                WHERE e.Id = @Id";
            var entry = await conn.QueryFirstOrDefaultAsync<OpeningStockDetailResponse>(entrySql, new { Id = id });
            if (entry == null) return null;

            var itemsSql = @"
                SELECT * FROM Stores_OpeningStockItems
                WHERE EntryId = @Id
                ORDER BY SortOrder, SequenceNo";
            entry.Items = (await conn.QueryAsync<OpeningStockItemResponse>(itemsSql, new { Id = id })).ToList();

            return entry;
        }

        public async Task<bool> UpdateItemsAsync(int entryId, IEnumerable<OpeningStockItem> items)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                await conn.ExecuteAsync("DELETE FROM Stores_OpeningStockItems WHERE EntryId = @EntryId", new { EntryId = entryId }, tx);

                var insertItemSql = @"
                    INSERT INTO Stores_OpeningStockItems
                        (EntryId, SequenceNo, ItemType,
                         MaterialId, MaterialName, Grade, MaterialType,
                         Diameter, OuterDiameter, InnerDiameter, Width, Thickness,
                         MaterialDensity, TotalWeightKG, CalculatedLengthMM, WeightPerMeterKG,
                         NumberOfPieces, LengthPerPieceMM, WarehouseId,
                         ComponentId, ComponentName, PartNumber, Quantity, UOM,
                         UnitCost, LineTotal, Remarks, SortOrder)
                    VALUES
                        (@EntryId, @SequenceNo, @ItemType,
                         @MaterialId, @MaterialName, @Grade, @MaterialType,
                         @Diameter, @OuterDiameter, @InnerDiameter, @Width, @Thickness,
                         @MaterialDensity, @TotalWeightKG, @CalculatedLengthMM, @WeightPerMeterKG,
                         @NumberOfPieces, @LengthPerPieceMM, @WarehouseId,
                         @ComponentId, @ComponentName, @PartNumber, @Quantity, @UOM,
                         @UnitCost, @LineTotal, @Remarks, @SortOrder)";

                foreach (var item in items)
                {
                    item.EntryId = entryId;
                    await conn.ExecuteAsync(insertItemSql, item, tx);
                }

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<bool> ConfirmAsync(int id, string? confirmedBy, int totalPieces, int totalComponents)
        {
            var sql = @"
                UPDATE Stores_OpeningStockEntries
                SET Status = 'Confirmed', ConfirmedAt = GETDATE(), ConfirmedBy = @ConfirmedBy,
                    TotalPieces = @TotalPieces, TotalComponents = @TotalComponents
                WHERE Id = @Id AND Status = 'Draft'";
            using var conn = GetConnection();
            var rows = await conn.ExecuteAsync(sql, new { Id = id, ConfirmedBy = confirmedBy, TotalPieces = totalPieces, TotalComponents = totalComponents });
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Stores_OpeningStockEntries WHERE Id = @Id AND Status = 'Draft'";
            using var conn = GetConnection();
            var rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
