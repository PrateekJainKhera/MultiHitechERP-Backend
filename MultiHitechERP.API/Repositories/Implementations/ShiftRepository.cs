using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Scheduling;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ShiftRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ShiftMaster>> GetAllAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<ShiftMaster>(
                "SELECT * FROM Scheduling_ShiftMaster ORDER BY ShiftName");
        }

        public async Task<IEnumerable<ShiftMaster>> GetActiveAsync()
        {
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryAsync<ShiftMaster>(
                "SELECT * FROM Scheduling_ShiftMaster WHERE IsActive = 1 ORDER BY ShiftName");
        }

        public async Task<ShiftMaster?> GetByIdAsync(int id)
        {
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<ShiftMaster>(
                "SELECT * FROM Scheduling_ShiftMaster WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> InsertAsync(ShiftMaster shift)
        {
            using var conn = _connectionFactory.CreateConnection();
            return await conn.QuerySingleAsync<int>(@"
                INSERT INTO Scheduling_ShiftMaster
                    (ShiftName, StartTime, EndTime, RegularHours, MaxOvertimeHours, IsActive, CreatedAt, CreatedBy)
                VALUES
                    (@ShiftName, @StartTime, @EndTime, @RegularHours, @MaxOvertimeHours, @IsActive, GETUTCDATE(), @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", shift);
        }

        public async Task<bool> UpdateAsync(ShiftMaster shift)
        {
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(@"
                UPDATE Scheduling_ShiftMaster SET
                    ShiftName = @ShiftName,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    RegularHours = @RegularHours,
                    MaxOvertimeHours = @MaxOvertimeHours,
                    IsActive = @IsActive,
                    UpdatedAt = GETUTCDATE(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id", shift);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _connectionFactory.CreateConnection();
            var rows = await conn.ExecuteAsync(
                "DELETE FROM Scheduling_ShiftMaster WHERE Id = @Id", new { Id = id });
            return rows > 0;
        }
    }
}
