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
    public class MaterialUsageHistoryRepository : IMaterialUsageHistoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialUsageHistoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<int> CreateAsync(MaterialUsageHistory usage)
        {
            var sql = @"
                INSERT INTO Stores_MaterialUsageHistory (
                    MaterialPieceId, PieceNo,
                    OrderId, OrderNo, ChildPartId, ChildPartName, ProductName,
                    JobCardId, JobCardNo,
                    LengthUsedMM, LengthRemainingMM, WastageGeneratedMM,
                    CuttingDate, CutByOperator, CutByOperatorId, MachineUsed, MachineId,
                    Notes, CreatedAt, CreatedBy
                )
                VALUES (
                    @MaterialPieceId, @PieceNo,
                    @OrderId, @OrderNo, @ChildPartId, @ChildPartName, @ProductName,
                    @JobCardId, @JobCardNo,
                    @LengthUsedMM, @LengthRemainingMM, @WastageGeneratedMM,
                    @CuttingDate, @CutByOperator, @CutByOperatorId, @MachineUsed, @MachineId,
                    @Notes, GETUTCDATE(), @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await GetConnection().ExecuteScalarAsync<int>(sql, usage);
        }

        public async Task<MaterialUsageHistory?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Stores_MaterialUsageHistory WHERE Id = @Id";
            return await GetConnection().QueryFirstOrDefaultAsync<MaterialUsageHistory>(sql, new { Id = id });
        }

        public async Task<IEnumerable<MaterialUsageHistory>> GetAllAsync()
        {
            var sql = "SELECT * FROM Stores_MaterialUsageHistory ORDER BY CuttingDate DESC";
            return await GetConnection().QueryAsync<MaterialUsageHistory>(sql);
        }

        public async Task<IEnumerable<MaterialUsageHistory>> GetByPieceIdAsync(int pieceId)
        {
            var sql = "SELECT * FROM Stores_MaterialUsageHistory WHERE MaterialPieceId = @PieceId ORDER BY CuttingDate";
            return await GetConnection().QueryAsync<MaterialUsageHistory>(sql, new { PieceId = pieceId });
        }

        public async Task<IEnumerable<MaterialUsageHistory>> GetByOrderIdAsync(int orderId)
        {
            var sql = "SELECT * FROM Stores_MaterialUsageHistory WHERE OrderId = @OrderId ORDER BY CuttingDate DESC";
            return await GetConnection().QueryAsync<MaterialUsageHistory>(sql, new { OrderId = orderId });
        }

        public async Task<IEnumerable<MaterialUsageHistory>> GetByJobCardIdAsync(int jobCardId)
        {
            var sql = "SELECT * FROM Stores_MaterialUsageHistory WHERE JobCardId = @JobCardId ORDER BY CuttingDate";
            return await GetConnection().QueryAsync<MaterialUsageHistory>(sql, new { JobCardId = jobCardId });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Stores_MaterialUsageHistory WHERE Id = @Id";
            var result = await GetConnection().ExecuteAsync(sql, new { Id = id });
            return result > 0;
        }
    }
}
