using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MaterialTypeRepository : IMaterialTypeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialTypeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MaterialTypeModel>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Masters_MaterialTypes WHERE IsActive = 1 ORDER BY Name";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<MaterialTypeModel>(sql);
        }

        public async Task<MaterialTypeModel?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Masters_MaterialTypes WHERE Id = @Id";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<MaterialTypeModel>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(MaterialTypeModel materialType)
        {
            const string sql = @"
                INSERT INTO Masters_MaterialTypes (Name, IsActive, CreatedAt, CreatedBy)
                VALUES (@Name, 1, GETUTCDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var conn = _connectionFactory.CreateConnection();
            return await conn.ExecuteScalarAsync<int>(sql, materialType);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "UPDATE Masters_MaterialTypes SET IsActive = 0 WHERE Id = @Id";
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
