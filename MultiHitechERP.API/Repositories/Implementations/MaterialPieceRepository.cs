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
    public class MaterialPieceRepository : IMaterialPieceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialPieceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<int> CreateAsync(MaterialPiece piece)
        {
            var sql = @"
                INSERT INTO Stores_MaterialPieces (
                    PieceNo, MaterialId, MaterialCode, MaterialName, Grade, Diameter,
                    OriginalLengthMM, CurrentLengthMM, OriginalWeightKG, CurrentWeightKG,
                    Status, AllocatedToRequisitionId, IssuedToJobCardId,
                    WarehouseId, WarehouseName, StorageLocation, BinNumber, RackNumber,
                    GRNId, GRNNo, ReceivedDate, SupplierBatchNo, SupplierId, UnitCost,
                    IsWastage, WastageReason, ScrapValue,
                    CreatedAt, CreatedBy
                )
                VALUES (
                    @PieceNo, @MaterialId, @MaterialCode, @MaterialName, @Grade, @Diameter,
                    @OriginalLengthMM, @CurrentLengthMM, @OriginalWeightKG, @CurrentWeightKG,
                    @Status, @AllocatedToRequisitionId, @IssuedToJobCardId,
                    @WarehouseId, @WarehouseName, @StorageLocation, @BinNumber, @RackNumber,
                    @GRNId, @GRNNo, @ReceivedDate, @SupplierBatchNo, @SupplierId, @UnitCost,
                    @IsWastage, @WastageReason, @ScrapValue,
                    GETUTCDATE(), @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await GetConnection().ExecuteScalarAsync<int>(sql, piece);
        }

        public async Task<MaterialPiece?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE Id = @Id";
            return await GetConnection().QueryFirstOrDefaultAsync<MaterialPiece>(sql, new { Id = id });
        }

        public async Task<MaterialPiece?> GetByPieceNoAsync(string pieceNo)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE PieceNo = @PieceNo";
            return await GetConnection().QueryFirstOrDefaultAsync<MaterialPiece>(sql, new { PieceNo = pieceNo });
        }

        public async Task<IEnumerable<MaterialPiece>> GetAllAsync()
        {
            var sql = "SELECT * FROM Stores_MaterialPieces ORDER BY CreatedAt DESC";
            return await GetConnection().QueryAsync<MaterialPiece>(sql);
        }

        public async Task<IEnumerable<MaterialPiece>> GetByMaterialIdAsync(int materialId)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE MaterialId = @MaterialId ORDER BY ReceivedDate DESC";
            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { MaterialId = materialId });
        }

        public async Task<IEnumerable<MaterialPiece>> GetByStatusAsync(string status)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE Status = @Status ORDER BY ReceivedDate DESC";
            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { Status = status });
        }

        public async Task<IEnumerable<MaterialPiece>> GetByGRNIdAsync(int grnId)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE GRNId = @GRNId ORDER BY PieceNo";
            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { GRNId = grnId });
        }

        public async Task<IEnumerable<MaterialPiece>> GetAvailablePiecesAsync()
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE Status = 'Available' ORDER BY ReceivedDate";
            return await GetConnection().QueryAsync<MaterialPiece>(sql);
        }

        public async Task<IEnumerable<MaterialPiece>> GetWastagePiecesAsync()
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE IsWastage = 1 ORDER BY ReceivedDate DESC";
            return await GetConnection().QueryAsync<MaterialPiece>(sql);
        }

        public async Task<bool> UpdateAsync(MaterialPiece piece)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces SET
                    MaterialCode = @MaterialCode,
                    MaterialName = @MaterialName,
                    Grade = @Grade,
                    Diameter = @Diameter,
                    CurrentLengthMM = @CurrentLengthMM,
                    CurrentWeightKG = @CurrentWeightKG,
                    Status = @Status,
                    AllocatedToRequisitionId = @AllocatedToRequisitionId,
                    IssuedToJobCardId = @IssuedToJobCardId,
                    WarehouseId = @WarehouseId,
                    WarehouseName = @WarehouseName,
                    StorageLocation = @StorageLocation,
                    BinNumber = @BinNumber,
                    RackNumber = @RackNumber,
                    IsWastage = @IsWastage,
                    WastageReason = @WastageReason,
                    ScrapValue = @ScrapValue,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, piece);
            return result > 0;
        }

        public async Task<bool> UpdateLengthAsync(int pieceId, decimal newLengthMM)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET CurrentLengthMM = @NewLength,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, new { Id = pieceId, NewLength = newLengthMM });
            return result > 0;
        }

        public async Task<bool> AdjustLengthAsync(int pieceId, decimal newLengthMM, decimal newWeightKG, string updatedBy)
        {
            // Update both Original and Current so the CHK_MaterialPieces_Length constraint
            // (CurrentLengthMM <= OriginalLengthMM) is never violated — this is a full correction.
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET OriginalLengthMM = @NewLengthMM,
                    CurrentLengthMM  = @NewLengthMM,
                    OriginalWeightKG = @NewWeightKG,
                    CurrentWeightKG  = @NewWeightKG,
                    UpdatedAt        = GETUTCDATE(),
                    UpdatedBy        = @UpdatedBy
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, new
            {
                Id           = pieceId,
                NewLengthMM  = newLengthMM,
                NewWeightKG  = newWeightKG,
                UpdatedBy    = updatedBy
            });
            return result > 0;
        }

        public async Task<bool> MarkAsWastageAsync(int pieceId, string reason, decimal? scrapValue)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET IsWastage = 1,
                    WastageReason = @Reason,
                    ScrapValue = @ScrapValue,
                    Status = 'Scrap',
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, new { Id = pieceId, Reason = reason, ScrapValue = scrapValue });
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Stores_MaterialPieces WHERE Id = @Id";
            var result = await GetConnection().ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }

        // Stock queries
        public async Task<decimal> GetTotalStockByMaterialIdAsync(int materialId)
        {
            var sql = @"
                SELECT ISNULL(SUM(CurrentLengthMM), 0)
                FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId AND Status != 'Consumed'";

            return await GetConnection().ExecuteScalarAsync<decimal>(sql, new { MaterialId = materialId });
        }

        public async Task<decimal> GetAvailableStockByMaterialIdAsync(int materialId)
        {
            var sql = @"
                SELECT ISNULL(SUM(CurrentLengthMM), 0)
                FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId AND Status = 'Available'";

            return await GetConnection().ExecuteScalarAsync<decimal>(sql, new { MaterialId = materialId });
        }

        // Requisition/Issue methods
        public async Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByFIFOAsync(int materialId, decimal requiredQuantityMM)
        {
            var sql = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId AND Status = 'Available'
                ORDER BY ReceivedDate, Id";

            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { MaterialId = materialId });
        }

        public async Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByMaterialAsync(int materialId, string? grade, decimal? diameterMM)
        {
            var sql = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId
                  AND Status = 'Available'
                  AND (@Grade IS NULL OR Grade = @Grade)
                  AND (@DiameterMM IS NULL OR Diameter = @DiameterMM)
                ORDER BY CurrentLengthMM";

            return await GetConnection().QueryAsync<MaterialPiece>(sql, new
            {
                MaterialId = materialId,
                Grade = grade,
                DiameterMM = diameterMM
            });
        }

        public async Task<IEnumerable<MaterialPiece>> GetAllocatedPiecesAsync(int requisitionId)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE AllocatedToRequisitionId = @RequisitionId";
            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { RequisitionId = requisitionId });
        }

        public async Task<bool> AllocatePieceAsync(int pieceId, int requisitionId)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Allocated',
                    AllocatedToRequisitionId = @RequisitionId,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id AND Status = 'Available'";

            var result = await GetConnection().ExecuteAsync(sql, new { Id = pieceId, RequisitionId = requisitionId });
            return result > 0;
        }

        public async Task<bool> ReturnPieceAsync(int pieceId)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Available',
                    AllocatedToRequisitionId = NULL,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, new { Id = pieceId });
            return result > 0;
        }

        public async Task<bool> IssuePieceAsync(int pieceId, int jobCardId, DateTime issuedDate, string issuedBy)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Issued',
                    IssuedToJobCardId = @JobCardId,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @IssuedBy
                WHERE Id = @Id";

            var result = await GetConnection().ExecuteAsync(sql, new { Id = pieceId, JobCardId = jobCardId, IssuedBy = issuedBy });
            return result > 0;
        }

        public async Task<int> ConsumePiecesByJobCardAsync(int jobCardId)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Consumed',
                    UpdatedAt = GETUTCDATE()
                WHERE IssuedToJobCardId = @JobCardId
                  AND Status = 'Issued'";

            return await GetConnection().ExecuteAsync(sql, new { JobCardId = jobCardId });
        }

        public async Task<IEnumerable<MaterialPiece>> GetByWarehouseIdAsync(int warehouseId)
        {
            var sql = "SELECT * FROM Stores_MaterialPieces WHERE WarehouseId = @WarehouseId ORDER BY Status, ReceivedDate DESC";
            return await GetConnection().QueryAsync<MaterialPiece>(sql, new { WarehouseId = warehouseId });
        }

        /// <summary>
        /// Relocate one or more pieces to a new warehouse.
        /// Updates WarehouseId and WarehouseName on each piece.
        /// </summary>
        public async Task<int> RelocatePiecesAsync(IEnumerable<int> pieceIds, int newWarehouseId, string warehouseName, string relocatedBy)
        {
            var sql = @"
                UPDATE Stores_MaterialPieces
                SET WarehouseId   = @WarehouseId,
                    WarehouseName = @WarehouseName,
                    UpdatedAt     = GETUTCDATE(),
                    UpdatedBy     = @UpdatedBy
                WHERE Id IN @Ids";

            return await GetConnection().ExecuteAsync(sql, new
            {
                WarehouseId   = newWarehouseId,
                WarehouseName = warehouseName,
                UpdatedBy     = relocatedBy,
                Ids           = pieceIds
            });
        }

        public async Task<bool> CutPieceAsync(
            int pieceId,
            decimal lengthToCutMM,
            int jobCardId,
            string cutByOperator,
            string? orderNo = null,
            string? childPartName = null,
            int minimumUsableLengthMM = 300)
        {
            var connection = GetConnection();

            // 1. Get current piece info
            var piece = await GetByIdAsync(pieceId);
            if (piece == null) return false;

            // 2. Calculate new length
            var newLengthMM = piece.CurrentLengthMM - lengthToCutMM;
            if (newLengthMM < 0) return false; // Can't cut more than available

            // 3. Calculate new weight (proportional to length)
            var newWeightKG = piece.OriginalLengthMM > 0
                ? piece.OriginalWeightKG * (newLengthMM / piece.OriginalLengthMM)
                : piece.CurrentWeightKG - (piece.CurrentWeightKG * (lengthToCutMM / piece.CurrentLengthMM));

            // 4. Check if piece becomes wastage
            var becomesWastage = newLengthMM < minimumUsableLengthMM;
            var newStatus = becomesWastage ? "Scrap" : "Available";

            // 5. Update the piece
            var updateSql = @"
                UPDATE Stores_MaterialPieces
                SET CurrentLengthMM = @NewLengthMM,
                    CurrentWeightKG = @NewWeightKG,
                    Status = @NewStatus,
                    IsWastage = @IsWastage,
                    WastageReason = @WastageReason,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @PieceId";

            var wastageReason = becomesWastage
                ? $"Remaining length ({newLengthMM:F0}mm) below minimum usable ({minimumUsableLengthMM}mm)"
                : null;

            var updateResult = await connection.ExecuteAsync(updateSql, new
            {
                PieceId = pieceId,
                NewLengthMM = newLengthMM,
                NewWeightKG = newWeightKG,
                NewStatus = newStatus,
                IsWastage = becomesWastage,
                WastageReason = wastageReason,
                UpdatedBy = cutByOperator
            });

            if (updateResult == 0) return false;

            // 6. Create usage history record
            var usageHistorySql = @"
                INSERT INTO Stores_MaterialUsageHistory (
                    MaterialPieceId, PieceNo, OrderNo, ChildPartName,
                    JobCardId, LengthUsedMM, LengthRemainingMM,
                    CuttingDate, CutByOperator, CreatedAt, CreatedBy
                ) VALUES (
                    @MaterialPieceId, @PieceNo, @OrderNo, @ChildPartName,
                    @JobCardId, @LengthUsedMM, @LengthRemainingMM,
                    GETUTCDATE(), @CutByOperator, GETUTCDATE(), @CreatedBy
                )";

            await connection.ExecuteAsync(usageHistorySql, new
            {
                MaterialPieceId = pieceId,
                PieceNo = piece.PieceNo,
                OrderNo = orderNo,
                ChildPartName = childPartName,
                JobCardId = jobCardId,
                LengthUsedMM = lengthToCutMM,
                LengthRemainingMM = newLengthMM,
                CutByOperator = cutByOperator,
                CreatedBy = cutByOperator
            });

            return true;
        }
    }
}
