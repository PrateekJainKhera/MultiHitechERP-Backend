using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ProcessRepository : IProcessRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProcessRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Process?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                WHERE p.Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcess(reader) : null;
        }

        public async Task<Process?> GetByProcessCodeAsync(string processCode)
        {
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                WHERE p.ProcessCode = @ProcessCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessCode", processCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcess(reader) : null;
        }

        public async Task<IEnumerable<Process>> GetAllAsync()
        {
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                ORDER BY p.ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<IEnumerable<Process>> GetActiveProcessesAsync()
        {
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                WHERE p.IsActive = 1
                ORDER BY p.ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<int> InsertAsync(Process process)
        {
            const string query = @"
                INSERT INTO Masters_Processes (
                    ProcessCode, ProcessName, Category,
                    DefaultMachine, DefaultMachineId, DefaultSetupTimeHours, DefaultCycleTimePerPieceHours,
                    StandardSetupTimeMin, RestTimeHours,
                    Description, IsOutsourced, IsActive, Status,
                    CreatedAt, CreatedBy
                ) VALUES (
                    @ProcessCode, @ProcessName, @Category,
                    @DefaultMachine, @DefaultMachineId, @DefaultSetupTimeHours, @DefaultCycleTimePerPieceHours,
                    @StandardSetupTimeMin, @RestTimeHours,
                    @Description, @IsOutsourced, @IsActive, @Status,
                    @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            process.CreatedAt = DateTime.UtcNow;
            AddProcessParameters(command, process);

            await connection.OpenAsync();
            var processId = (int)await command.ExecuteScalarAsync();
            process.Id = processId;

            return processId;
        }

        public async Task<bool> UpdateAsync(Process process)
        {
            const string query = @"
                UPDATE Masters_Processes SET
                    ProcessName = @ProcessName, Category = @Category,
                    DefaultMachine = @DefaultMachine,
                    DefaultMachineId = @DefaultMachineId,
                    DefaultSetupTimeHours = @DefaultSetupTimeHours,
                    DefaultCycleTimePerPieceHours = @DefaultCycleTimePerPieceHours,
                    StandardSetupTimeMin = @StandardSetupTimeMin,
                    RestTimeHours = @RestTimeHours,
                    Description = @Description,
                    IsOutsourced = @IsOutsourced,
                    IsActive = @IsActive, Status = @Status,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            process.UpdatedAt = DateTime.UtcNow;
            AddProcessParameters(command, process);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Processes WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> ActivateAsync(int id)
        {
            const string query = "UPDATE Masters_Processes SET IsActive = 1, Status = 'Active', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            const string query = "UPDATE Masters_Processes SET IsActive = 0, Status = 'Inactive', UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<Process>> GetByProcessTypeAsync(string processType)
        {
            // ProcessType column was dropped - using Category instead
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                WHERE p.Category = @Category
                ORDER BY p.ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", processType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<IEnumerable<Process>> GetByDepartmentAsync(string department)
        {
            // Department column was dropped - returning empty list
            return new List<Process>();
        }

        public async Task<IEnumerable<Process>> GetByMachineTypeAsync(string machineType)
        {
            // MachineType column was dropped - returning empty list
            return new List<Process>();
        }

        public async Task<IEnumerable<Process>> GetOutsourcedProcessesAsync()
        {
            const string query = @"
                SELECT p.*, m.MachineName AS DefaultMachineName, m.MachineCode AS DefaultMachineCode
                FROM Masters_Processes p
                LEFT JOIN Masters_Machines m ON p.DefaultMachineId = m.Id
                WHERE p.IsOutsourced = 1 AND p.IsActive = 1
                ORDER BY p.ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<bool> ExistsAsync(string processCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Processes WHERE ProcessCode = @ProcessCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessCode", processCode);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync(string category)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Processes WHERE Category = @Category";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Category", category);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count + 1;
        }

        private static Process MapToProcess(SqlDataReader reader)
        {
            return new Process
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProcessCode = reader.GetString(reader.GetOrdinal("ProcessCode")),
                ProcessName = reader.GetString(reader.GetOrdinal("ProcessName")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                DefaultMachine = reader.IsDBNull(reader.GetOrdinal("DefaultMachine")) ? null : reader.GetString(reader.GetOrdinal("DefaultMachine")),
                DefaultMachineId = reader.IsDBNull(reader.GetOrdinal("DefaultMachineId")) ? null : reader.GetInt32(reader.GetOrdinal("DefaultMachineId")),
                DefaultSetupTimeHours = reader.IsDBNull(reader.GetOrdinal("DefaultSetupTimeHours")) ? null : reader.GetDecimal(reader.GetOrdinal("DefaultSetupTimeHours")),
                DefaultCycleTimePerPieceHours = reader.IsDBNull(reader.GetOrdinal("DefaultCycleTimePerPieceHours")) ? null : reader.GetDecimal(reader.GetOrdinal("DefaultCycleTimePerPieceHours")),
                StandardSetupTimeMin = reader.IsDBNull(reader.GetOrdinal("StandardSetupTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("StandardSetupTimeMin")),
                RestTimeHours = reader.IsDBNull(reader.GetOrdinal("RestTimeHours")) ? null : reader.GetDecimal(reader.GetOrdinal("RestTimeHours")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                IsOutsourced = reader.GetBoolean(reader.GetOrdinal("IsOutsourced")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                // Navigation properties from JOIN
                DefaultMachineName = reader.IsDBNull(reader.GetOrdinal("DefaultMachineName")) ? null : reader.GetString(reader.GetOrdinal("DefaultMachineName")),
                DefaultMachineCode = reader.IsDBNull(reader.GetOrdinal("DefaultMachineCode")) ? null : reader.GetString(reader.GetOrdinal("DefaultMachineCode"))
            };
        }

        private static void AddProcessParameters(SqlCommand command, Process process)
        {
            command.Parameters.AddWithValue("@Id", process.Id);
            command.Parameters.AddWithValue("@ProcessCode", process.ProcessCode);
            command.Parameters.AddWithValue("@ProcessName", process.ProcessName);
            command.Parameters.AddWithValue("@Category", (object?)process.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachine", (object?)process.DefaultMachine ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachineId", (object?)process.DefaultMachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultSetupTimeHours", (object?)process.DefaultSetupTimeHours ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultCycleTimePerPieceHours", (object?)process.DefaultCycleTimePerPieceHours ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardSetupTimeMin", (object?)process.StandardSetupTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@RestTimeHours", (object?)process.RestTimeHours ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)process.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsOutsourced", process.IsOutsourced);
            command.Parameters.AddWithValue("@IsActive", process.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)process.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", process.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)process.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)process.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)process.UpdatedBy ?? DBNull.Value);
        }
    }
}
