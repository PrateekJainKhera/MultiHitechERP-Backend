using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class MachineRepository : IMachineRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MachineRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Machine?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Machines WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToMachine(reader) : null;
        }

        public async Task<Machine?> GetByMachineCodeAsync(string machineCode)
        {
            const string query = "SELECT * FROM Masters_Machines WHERE MachineCode = @MachineCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineCode", machineCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToMachine(reader) : null;
        }

        public async Task<IEnumerable<Machine>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Machines ORDER BY MachineName";

            var machines = new List<Machine>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                machines.Add(MapToMachine(reader));

            return machines;
        }

        public async Task<IEnumerable<Machine>> GetActiveMachinesAsync()
        {
            const string query = "SELECT * FROM Masters_Machines WHERE IsActive = 1 ORDER BY MachineName";

            var machines = new List<Machine>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                machines.Add(MapToMachine(reader));

            return machines;
        }

        public async Task<IEnumerable<Machine>> GetByMachineTypeAsync(string machineType)
        {
            const string query = "SELECT * FROM Masters_Machines WHERE MachineType = @MachineType ORDER BY MachineName";

            var machines = new List<Machine>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineType", machineType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                machines.Add(MapToMachine(reader));

            return machines;
        }

        public async Task<IEnumerable<Machine>> GetByDepartmentAsync(string department)
        {
            const string query = "SELECT * FROM Masters_Machines WHERE Department = @Department ORDER BY MachineName";

            var machines = new List<Machine>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Department", department);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                machines.Add(MapToMachine(reader));

            return machines;
        }

        public async Task<int> InsertAsync(Machine machine)
        {
            const string query = @"
                INSERT INTO Masters_Machines (
                    MachineCode, MachineName, MachineType, Location, Department,
                    Status, Notes, IsActive, CreatedAt, CreatedBy
                ) VALUES (
                    @MachineCode, @MachineName, @MachineType, @Location, @Department,
                    @Status, @Notes, @IsActive, @CreatedAt, @CreatedBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            machine.CreatedAt = DateTime.UtcNow;
            command.Parameters.AddWithValue("@MachineCode", machine.MachineCode);
            command.Parameters.AddWithValue("@MachineName", machine.MachineName);
            command.Parameters.AddWithValue("@MachineType", (object?)machine.MachineType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)machine.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", (object?)machine.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)machine.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)machine.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", machine.IsActive);
            command.Parameters.AddWithValue("@CreatedAt", machine.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)machine.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync()!;
        }

        public async Task<bool> UpdateAsync(Machine machine)
        {
            const string query = @"
                UPDATE Masters_Machines SET
                    MachineName = @MachineName, MachineType = @MachineType,
                    Location = @Location, Department = @Department,
                    Status = @Status, Notes = @Notes, IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            machine.UpdatedAt = DateTime.UtcNow;
            command.Parameters.AddWithValue("@Id", machine.Id);
            command.Parameters.AddWithValue("@MachineName", machine.MachineName);
            command.Parameters.AddWithValue("@MachineType", (object?)machine.MachineType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)machine.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", (object?)machine.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)machine.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Notes", (object?)machine.Notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", machine.IsActive);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)machine.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)machine.UpdatedBy ?? DBNull.Value);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Machines WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> ExistsAsync(string machineCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Machines WHERE MachineCode = @MachineCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineCode", machineCode);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync()! > 0;
        }

        public async Task<string> GetNextMachineCodeAsync()
        {
            const string query = @"
                SELECT COALESCE(MAX(CAST(RIGHT(MachineCode, LEN(MachineCode) - CHARINDEX('-', MachineCode)) AS INT)), 0)
                FROM Masters_Machines";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            var maxNum = (int)await command.ExecuteScalarAsync()!;
            return $"MCH-{maxNum + 1:D3}";
        }

        private static Machine MapToMachine(SqlDataReader reader)
        {
            return new Machine
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                MachineCode = reader.GetString(reader.GetOrdinal("MachineCode")),
                MachineName = reader.GetString(reader.GetOrdinal("MachineName")),
                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? null : reader.GetString(reader.GetOrdinal("MachineType")),
                Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }
    }
}
