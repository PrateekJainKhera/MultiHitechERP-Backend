using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ScrapSaleRepository : IScrapSaleRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ScrapSaleRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<int> CreateAsync(ScrapSale sale)
        {
            var sql = @"
                INSERT INTO Stores_ScrapSales
                    (MaterialId, MaterialCode, MaterialName, WeightKG, RatePerKG, TotalAmount,
                     BuyerName, SaleDate, Remarks, CreatedAt, CreatedBy)
                VALUES
                    (@MaterialId, @MaterialCode, @MaterialName, @WeightKG, @RatePerKG, @TotalAmount,
                     @BuyerName, @SaleDate, @Remarks, GETUTCDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            return await GetConnection().ExecuteScalarAsync<int>(sql, sale);
        }

        public async Task<IEnumerable<ScrapSaleResponse>> GetAllAsync()
        {
            var sql = @"SELECT Id, MaterialId, MaterialCode, MaterialName, WeightKG, RatePerKG, TotalAmount,
                               BuyerName, SaleDate, Remarks, CreatedAt, CreatedBy
                        FROM Stores_ScrapSales
                        ORDER BY SaleDate DESC, Id DESC";
            return await GetConnection().QueryAsync<ScrapSaleResponse>(sql);
        }

        public async Task<IEnumerable<ScrapMaterialSummaryResponse>> GetWastageByMaterialAsync()
        {
            var sql = @"SELECT MaterialId, MAX(MaterialCode) AS MaterialCode, MAX(MaterialName) AS MaterialName,
                               COUNT(*) AS WastagePieces, ISNULL(SUM(CurrentWeightKG), 0) AS WastageWeightKG
                        FROM Stores_MaterialPieces
                        WHERE IsWastage = 1
                        GROUP BY MaterialId";
            return await GetConnection().QueryAsync<ScrapMaterialSummaryResponse>(sql);
        }

        public async Task<IEnumerable<ScrapMaterialSummaryResponse>> GetSoldByMaterialAsync()
        {
            var sql = @"SELECT MaterialId, MAX(MaterialCode) AS MaterialCode, MAX(MaterialName) AS MaterialName,
                               ISNULL(SUM(WeightKG), 0) AS SoldWeightKG
                        FROM Stores_ScrapSales
                        GROUP BY MaterialId";
            return await GetConnection().QueryAsync<ScrapMaterialSummaryResponse>(sql);
        }
    }
}
