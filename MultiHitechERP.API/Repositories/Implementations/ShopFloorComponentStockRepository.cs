using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ShopFloorComponentStockRepository : IShopFloorComponentStockRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ShopFloorComponentStockRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static ShopFloorComponentStock Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(r.GetOrdinal("Id")),
            ComponentId = r.GetInt32(r.GetOrdinal("ComponentId")),
            ComponentName = r.IsDBNull(r.GetOrdinal("ComponentName")) ? null : r.GetString(r.GetOrdinal("ComponentName")),
            PartNumber = r.IsDBNull(r.GetOrdinal("PartNumber")) ? null : r.GetString(r.GetOrdinal("PartNumber")),
            UOM = r.IsDBNull(r.GetOrdinal("UOM")) ? null : r.GetString(r.GetOrdinal("UOM")),
            Quantity = r.GetDecimal(r.GetOrdinal("Quantity")),
            ReservedQty = r.GetDecimal(r.GetOrdinal("ReservedQty")),
            LastUpdated = r.GetDateTime(r.GetOrdinal("LastUpdated")),
            UpdatedBy = r.IsDBNull(r.GetOrdinal("UpdatedBy")) ? null : r.GetString(r.GetOrdinal("UpdatedBy")),
        };

        public async Task<IEnumerable<ShopFloorComponentStock>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Stores_ShopFloorComponentStock ORDER BY ComponentName";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            using var r = await cmd.ExecuteReaderAsync();
            var list = new List<ShopFloorComponentStock>();
            while (await r.ReadAsync()) list.Add(Map((SqlDataReader)r));
            return list;
        }

        public async Task<ShopFloorComponentStock?> GetByComponentIdAsync(int componentId)
        {
            const string sql = "SELECT * FROM Stores_ShopFloorComponentStock WHERE ComponentId = @ComponentId";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync()) return Map((SqlDataReader)r);
            return null;
        }

        public async Task AddToFloorAsync(int componentId, string? componentName, string? partNumber, string? uom, decimal qty, string? by)
        {
            // Upsert: bump quantity if the component already has a floor row, otherwise create it.
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Stores_ShopFloorComponentStock WHERE ComponentId = @ComponentId)
                    UPDATE Stores_ShopFloorComponentStock
                       SET Quantity = Quantity + @Qty,
                           ComponentName = COALESCE(@ComponentName, ComponentName),
                           PartNumber = COALESCE(@PartNumber, PartNumber),
                           UOM = COALESCE(@UOM, UOM),
                           LastUpdated = GETUTCDATE(),
                           UpdatedBy = @By
                     WHERE ComponentId = @ComponentId;
                ELSE
                    INSERT INTO Stores_ShopFloorComponentStock (ComponentId, ComponentName, PartNumber, UOM, Quantity, ReservedQty, LastUpdated, UpdatedBy)
                    VALUES (@ComponentId, @ComponentName, @PartNumber, @UOM, @Qty, 0, GETUTCDATE(), @By);";

            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            cmd.Parameters.AddWithValue("@ComponentName", (object?)componentName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PartNumber", (object?)partNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UOM", (object?)uom ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Qty", qty);
            cmd.Parameters.AddWithValue("@By", (object?)by ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task AdjustReservedAsync(int componentId, decimal delta)
        {
            const string sql = @"
                UPDATE Stores_ShopFloorComponentStock
                   SET ReservedQty = CASE WHEN ReservedQty + @Delta < 0 THEN 0 ELSE ReservedQty + @Delta END,
                       LastUpdated = GETUTCDATE()
                 WHERE ComponentId = @ComponentId;
                IF @@ROWCOUNT = 0 AND @Delta > 0
                    INSERT INTO Stores_ShopFloorComponentStock (ComponentId, Quantity, ReservedQty, LastUpdated)
                    VALUES (@ComponentId, 0, @Delta, GETUTCDATE());";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            cmd.Parameters.AddWithValue("@Delta", delta);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ConsumeAsync(int componentId, decimal qty)
        {
            // Block if the floor doesn't physically have enough. Also release up to qty of reservation.
            const string sql = @"
                UPDATE Stores_ShopFloorComponentStock
                   SET Quantity = Quantity - @Qty,
                       ReservedQty = CASE WHEN ReservedQty - @Qty < 0 THEN 0 ELSE ReservedQty - @Qty END,
                       LastUpdated = GETUTCDATE()
                 WHERE ComponentId = @ComponentId AND Quantity >= @Qty;
                SELECT @@ROWCOUNT;";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            cmd.Parameters.AddWithValue("@Qty", qty);
            var affected = (int)(await cmd.ExecuteScalarAsync() ?? 0);
            return affected > 0;
        }
    }
}
