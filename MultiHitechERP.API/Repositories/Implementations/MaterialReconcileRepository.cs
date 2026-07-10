using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MaterialReconcileRepository : IMaterialReconcileRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialReconcileRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ReconcileLogResponse>> GetHistoryAsync(int? materialId)
        {
            var sql = @"SELECT TOP 500 Id, MaterialId, MaterialCode, MaterialName, PieceNo, ActionType,
                            LengthBeforeMM, LengthAfterMM, LengthRemovedMM, WeightRemovedKG, Reason, Remarks, PerformedBy, CreatedAt
                        FROM Stores_MaterialReconcileLog"
                        + (materialId.HasValue ? " WHERE MaterialId=@MaterialId" : "")
                        + " ORDER BY CreatedAt DESC, Id DESC";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            if (materialId.HasValue) cmd.Parameters.AddWithValue("@MaterialId", materialId.Value);
            using var r = await cmd.ExecuteReaderAsync();
            var list = new List<ReconcileLogResponse>();
            decimal? Dec(int i) => r.IsDBNull(i) ? (decimal?)null : r.GetDecimal(i);
            string? Str(int i) => r.IsDBNull(i) ? null : r.GetString(i);
            while (await r.ReadAsync())
            {
                list.Add(new ReconcileLogResponse
                {
                    Id = r.GetInt32(0),
                    MaterialId = r.GetInt32(1),
                    MaterialCode = Str(2),
                    MaterialName = Str(3),
                    PieceNo = Str(4),
                    ActionType = Str(5),
                    LengthBeforeMM = Dec(6),
                    LengthAfterMM = Dec(7),
                    LengthRemovedMM = Dec(8),
                    WeightRemovedKG = Dec(9),
                    Reason = Str(10),
                    Remarks = Str(11),
                    PerformedBy = Str(12),
                    CreatedAt = r.IsDBNull(13) ? null : r.GetDateTime(13).ToString("yyyy-MM-dd HH:mm"),
                });
            }
            return list;
        }

        public async Task InsertLogAsync(int materialId, string? materialCode, string? materialName,
            int? pieceId, string? pieceNo, string actionType,
            decimal? lengthBefore, decimal? lengthAfter, decimal? lengthRemoved, decimal? weightRemoved,
            string? reason, string? remarks, string? performedBy)
        {
            const string sql = @"
                INSERT INTO Stores_MaterialReconcileLog
                    (MaterialId, MaterialCode, MaterialName, PieceId, PieceNo, ActionType,
                     LengthBeforeMM, LengthAfterMM, LengthRemovedMM, WeightRemovedKG, Reason, Remarks, PerformedBy, CreatedAt)
                VALUES
                    (@MaterialId, @MaterialCode, @MaterialName, @PieceId, @PieceNo, @ActionType,
                     @LengthBefore, @LengthAfter, @LengthRemoved, @WeightRemoved, @Reason, @Remarks, @PerformedBy, GETUTCDATE())";

            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaterialId", materialId);
            cmd.Parameters.AddWithValue("@MaterialCode", (object?)materialCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MaterialName", (object?)materialName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PieceId", (object?)pieceId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PieceNo", (object?)pieceNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ActionType", actionType);
            cmd.Parameters.AddWithValue("@LengthBefore", (object?)lengthBefore ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LengthAfter", (object?)lengthAfter ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LengthRemoved", (object?)lengthRemoved ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WeightRemoved", (object?)weightRemoved ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Reason", (object?)reason ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PerformedBy", (object?)performedBy ?? DBNull.Value);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task SetMaterialAggregateAsync(int materialId, decimal totalLengthMM, string uom)
        {
            const string sql = @"
                IF EXISTS (SELECT 1 FROM Inventory_Stock WHERE ItemType='RawMaterial' AND ItemId=@MaterialId)
                    UPDATE Inventory_Stock SET CurrentStock=@Total, LastUpdated=GETUTCDATE() WHERE ItemType='RawMaterial' AND ItemId=@MaterialId;
                ELSE
                    INSERT INTO Inventory_Stock (ItemType, ItemId, ItemCode, ItemName, CurrentStock, ReservedStock, UOM, LastUpdated)
                    SELECT 'RawMaterial', @MaterialId, m.MaterialCode, m.MaterialName, @Total, 0, @UOM, GETUTCDATE()
                    FROM Masters_Materials m WHERE m.Id=@MaterialId;";
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@MaterialId", materialId);
            cmd.Parameters.AddWithValue("@Total", totalLengthMM);
            cmd.Parameters.AddWithValue("@UOM", string.IsNullOrWhiteSpace(uom) ? "mm" : uom);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
