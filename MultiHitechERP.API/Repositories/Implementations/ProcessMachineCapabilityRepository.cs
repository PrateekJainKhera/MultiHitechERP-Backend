using System.Data;
using Dapper;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProcessMachineCapabilityRepository : IProcessMachineCapabilityRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProcessMachineCapabilityRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<IEnumerable<ProcessMachineCapability>> GetAllAsync()
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                ORDER BY p.ProcessName, pmc.PreferenceLevel";

            return await connection.QueryAsync<ProcessMachineCapability>(sql);
        }

        public async Task<ProcessMachineCapability?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                WHERE pmc.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<ProcessMachineCapability>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ProcessMachineCapability>> GetByProcessIdAsync(int processId)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                WHERE pmc.ProcessId = @ProcessId
                ORDER BY pmc.PreferenceLevel";

            return await connection.QueryAsync<ProcessMachineCapability>(sql, new { ProcessId = processId });
        }

        public async Task<IEnumerable<ProcessMachineCapability>> GetByMachineIdAsync(int machineId)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                WHERE pmc.MachineId = @MachineId
                ORDER BY p.ProcessName";

            return await connection.QueryAsync<ProcessMachineCapability>(sql, new { MachineId = machineId });
        }

        public async Task<IEnumerable<ProcessMachineCapability>> GetCapableMachinesForProcessAsync(int processId)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                WHERE pmc.ProcessId = @ProcessId
                  AND pmc.IsActive = 1
                  AND m.IsActive = 1
                  AND m.IsAvailable = 1
                ORDER BY pmc.PreferenceLevel, pmc.EfficiencyRating DESC";

            return await connection.QueryAsync<ProcessMachineCapability>(sql, new { ProcessId = processId });
        }

        public async Task<ProcessMachineCapability?> GetByProcessAndMachineAsync(int processId, int machineId)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT
                    pmc.*,
                    p.ProcessName,
                    m.MachineName,
                    m.MachineCode
                FROM Masters_ProcessMachineCapability pmc
                LEFT JOIN Masters_Processes p ON pmc.ProcessId = p.Id
                LEFT JOIN Masters_Machines m ON pmc.MachineId = m.Id
                WHERE pmc.ProcessId = @ProcessId AND pmc.MachineId = @MachineId";

            return await connection.QueryFirstOrDefaultAsync<ProcessMachineCapability>(
                sql,
                new { ProcessId = processId, MachineId = machineId }
            );
        }

        public async Task<int> InsertAsync(ProcessMachineCapability capability)
        {
            using var connection = GetConnection();
            const string sql = @"
                INSERT INTO Masters_ProcessMachineCapability (
                    ProcessId, MachineId, SetupTimeHours, CycleTimePerPieceHours,
                    PreferenceLevel, EfficiencyRating, IsPreferredMachine,
                    MaxWorkpieceLength, MaxWorkpieceDiameter, MaxBatchSize,
                    HourlyRate, EstimatedCostPerPiece,
                    IsActive, AvailableFrom, AvailableTo,
                    Remarks, CreatedAt, CreatedBy
                )
                VALUES (
                    @ProcessId, @MachineId, @SetupTimeHours, @CycleTimePerPieceHours,
                    @PreferenceLevel, @EfficiencyRating, @IsPreferredMachine,
                    @MaxWorkpieceLength, @MaxWorkpieceDiameter, @MaxBatchSize,
                    @HourlyRate, @EstimatedCostPerPiece,
                    @IsActive, @AvailableFrom, @AvailableTo,
                    @Remarks, @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, capability);
        }

        public async Task<bool> UpdateAsync(ProcessMachineCapability capability)
        {
            using var connection = GetConnection();
            const string sql = @"
                UPDATE Masters_ProcessMachineCapability
                SET ProcessId = @ProcessId,
                    MachineId = @MachineId,
                    SetupTimeHours = @SetupTimeHours,
                    CycleTimePerPieceHours = @CycleTimePerPieceHours,
                    PreferenceLevel = @PreferenceLevel,
                    EfficiencyRating = @EfficiencyRating,
                    IsPreferredMachine = @IsPreferredMachine,
                    MaxWorkpieceLength = @MaxWorkpieceLength,
                    MaxWorkpieceDiameter = @MaxWorkpieceDiameter,
                    MaxBatchSize = @MaxBatchSize,
                    HourlyRate = @HourlyRate,
                    EstimatedCostPerPiece = @EstimatedCostPerPiece,
                    IsActive = @IsActive,
                    AvailableFrom = @AvailableFrom,
                    AvailableTo = @AvailableTo,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(sql, capability);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = GetConnection();
            const string sql = "DELETE FROM Masters_ProcessMachineCapability WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int processId, int machineId)
        {
            using var connection = GetConnection();
            const string sql = @"
                SELECT COUNT(1)
                FROM Masters_ProcessMachineCapability
                WHERE ProcessId = @ProcessId AND MachineId = @MachineId";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { ProcessId = processId, MachineId = machineId });
            return count > 0;
        }
    }
}
