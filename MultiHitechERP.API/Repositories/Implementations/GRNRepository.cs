using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class GRNRepository : IGRNRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public GRNRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<int> CreateAsync(GRN grn)
        {
            var sql = @"
                INSERT INTO Stores_GRN (
                    GRNNo, GRNDate, SupplierId, SupplierName, SupplierBatchNo,
                    PONo, PODate, InvoiceNo, InvoiceDate,
                    TotalPieces, TotalWeight, TotalValue, Status,
                    QualityCheckStatus, QualityCheckedBy, QualityCheckedAt, QualityRemarks,
                    Remarks, CreatedAt, CreatedBy
                )
                VALUES (
                    @GRNNo, @GRNDate, @SupplierId, @SupplierName, @SupplierBatchNo,
                    @PONo, @PODate, @InvoiceNo, @InvoiceDate,
                    @TotalPieces, @TotalWeight, @TotalValue, @Status,
                    @QualityCheckStatus, @QualityCheckedBy, @QualityCheckedAt, @QualityRemarks,
                    @Remarks, GETUTCDATE(), @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await GetConnection().ExecuteScalarAsync<int>(sql, grn);
        }

        public async Task<GRN?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Stores_GRN WHERE Id = @Id";
            return await GetConnection().QueryFirstOrDefaultAsync<GRN>(sql, new { Id = id });
        }

        public async Task<GRN?> GetByGRNNoAsync(string grnNo)
        {
            var sql = "SELECT * FROM Stores_GRN WHERE GRNNo = @GRNNo";
            return await GetConnection().QueryFirstOrDefaultAsync<GRN>(sql, new { GRNNo = grnNo });
        }

        public async Task<IEnumerable<GRN>> GetAllAsync()
        {
            var sql = "SELECT * FROM Stores_GRN ORDER BY GRNDate DESC, CreatedAt DESC";
            return await GetConnection().QueryAsync<GRN>(sql);
        }

        public async Task<IEnumerable<GRN>> GetBySupplierId(int supplierId)
        {
            var sql = "SELECT * FROM Stores_GRN WHERE SupplierId = @SupplierId ORDER BY GRNDate DESC";
            return await GetConnection().QueryAsync<GRN>(sql, new { SupplierId = supplierId });
        }

        public async Task<bool> UpdateAsync(GRN grn)
        {
            var sql = @"
                UPDATE Stores_GRN SET
                    GRNDate = @GRNDate,
                    SupplierId = @SupplierId,
                    SupplierName = @SupplierName,
                    SupplierBatchNo = @SupplierBatchNo,
                    PONo = @PONo,
                    PODate = @PODate,
                    InvoiceNo = @InvoiceNo,
                    InvoiceDate = @InvoiceDate,
                    TotalPieces = @TotalPieces,
                    TotalWeight = @TotalWeight,
                    TotalValue = @TotalValue,
                    Status = @Status,
                    QualityCheckStatus = @QualityCheckStatus,
                    QualityCheckedBy = @QualityCheckedBy,
                    QualityCheckedAt = @QualityCheckedAt,
                    QualityRemarks = @QualityRemarks,
                    Remarks = @Remarks,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, grn);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Stores_GRN WHERE Id = @Id";
            var result = await GetConnection().ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        // GRN Lines
        public async Task<int> CreateLineAsync(GRNLine line)
        {
            var sql = @"
                INSERT INTO Stores_GRNLines (
                    GRNId, SequenceNo, MaterialId, MaterialName, Grade,
                    MaterialType, Diameter, OuterDiameter, InnerDiameter, Width, Thickness,
                    MaterialDensity, TotalWeightKG, CalculatedLengthMM, WeightPerMeterKG,
                    NumberOfPieces, LengthPerPieceMM, UnitPrice, LineTotal, Remarks
                )
                VALUES (
                    @GRNId, @SequenceNo, @MaterialId, @MaterialName, @Grade,
                    @MaterialType, @Diameter, @OuterDiameter, @InnerDiameter, @Width, @Thickness,
                    @MaterialDensity, @TotalWeightKG, @CalculatedLengthMM, @WeightPerMeterKG,
                    @NumberOfPieces, @LengthPerPieceMM, @UnitPrice, @LineTotal, @Remarks
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await GetConnection().ExecuteScalarAsync<int>(sql, line);
        }

        public async Task<IEnumerable<GRNLine>> GetLinesByGRNIdAsync(int grnId)
        {
            var sql = "SELECT * FROM Stores_GRNLines WHERE GRNId = @GRNId ORDER BY SequenceNo";
            return await GetConnection().QueryAsync<GRNLine>(sql, new { GRNId = grnId });
        }

        public async Task<bool> UpdateLineAsync(GRNLine line)
        {
            var sql = @"
                UPDATE Stores_GRNLines SET
                    MaterialId = @MaterialId,
                    MaterialName = @MaterialName,
                    Grade = @Grade,
                    MaterialType = @MaterialType,
                    Diameter = @Diameter,
                    OuterDiameter = @OuterDiameter,
                    InnerDiameter = @InnerDiameter,
                    Width = @Width,
                    Thickness = @Thickness,
                    MaterialDensity = @MaterialDensity,
                    TotalWeightKG = @TotalWeightKG,
                    CalculatedLengthMM = @CalculatedLengthMM,
                    WeightPerMeterKG = @WeightPerMeterKG,
                    NumberOfPieces = @NumberOfPieces,
                    LengthPerPieceMM = @LengthPerPieceMM,
                    UnitPrice = @UnitPrice,
                    LineTotal = @LineTotal,
                    Remarks = @Remarks
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, line);
            return result > 0;
        }

        public async Task<bool> DeleteLineAsync(int lineId)
        {
            var sql = "DELETE FROM Stores_GRNLines WHERE Id = @Id";
            var result = await GetConnection().ExecuteAsync(sql, new { Id = lineId });
            return result > 0;
        }
    }
}
