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
            const string query = "SELECT * FROM Masters_Processes WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcess(reader) : null;
        }

        public async Task<Process?> GetByProcessCodeAsync(string processCode)
        {
            const string query = "SELECT * FROM Masters_Processes WHERE ProcessCode = @ProcessCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessCode", processCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToProcess(reader) : null;
        }

        public async Task<IEnumerable<Process>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Processes ORDER BY ProcessName";

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
            const string query = "SELECT * FROM Masters_Processes WHERE IsActive = 1 ORDER BY ProcessName";

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
                    ProcessCode, ProcessName, ProcessType, Category, Department,
                    Description, ProcessDetails,
                    MachineType, DefaultMachineId, DefaultMachineName,
                    StandardSetupTimeMin, StandardCycleTimeMin, StandardCycleTimePerPiece,
                    SkillLevel, OperatorsRequired,
                    HourlyRate, StandardCostPerPiece,
                    RequiresQC, QCCheckpoints,
                    IsOutsourced, PreferredVendor,
                    IsActive, Status, Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @ProcessCode, @ProcessName, @ProcessType, @Category, @Department,
                    @Description, @ProcessDetails,
                    @MachineType, @DefaultMachineId, @DefaultMachineName,
                    @StandardSetupTimeMin, @StandardCycleTimeMin, @StandardCycleTimePerPiece,
                    @SkillLevel, @OperatorsRequired,
                    @HourlyRate, @StandardCostPerPiece,
                    @RequiresQC, @QCCheckpoints,
                    @IsOutsourced, @PreferredVendor,
                    @IsActive, @Status, @Remarks, @CreatedAt, @CreatedBy
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
                    ProcessName = @ProcessName, ProcessType = @ProcessType, Category = @Category,
                    Department = @Department, Description = @Description, ProcessDetails = @ProcessDetails,
                    MachineType = @MachineType, DefaultMachineId = @DefaultMachineId,
                    DefaultMachineName = @DefaultMachineName,
                    StandardSetupTimeMin = @StandardSetupTimeMin, StandardCycleTimeMin = @StandardCycleTimeMin,
                    StandardCycleTimePerPiece = @StandardCycleTimePerPiece,
                    SkillLevel = @SkillLevel, OperatorsRequired = @OperatorsRequired,
                    HourlyRate = @HourlyRate, StandardCostPerPiece = @StandardCostPerPiece,
                    RequiresQC = @RequiresQC, QCCheckpoints = @QCCheckpoints,
                    IsOutsourced = @IsOutsourced, PreferredVendor = @PreferredVendor,
                    IsActive = @IsActive, Status = @Status, Remarks = @Remarks,
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
            const string query = "SELECT * FROM Masters_Processes WHERE ProcessType = @ProcessType ORDER BY ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ProcessType", processType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<IEnumerable<Process>> GetByDepartmentAsync(string department)
        {
            const string query = "SELECT * FROM Masters_Processes WHERE Department = @Department ORDER BY ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Department", department);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<IEnumerable<Process>> GetByMachineTypeAsync(string machineType)
        {
            const string query = "SELECT * FROM Masters_Processes WHERE MachineType = @MachineType ORDER BY ProcessName";

            var processes = new List<Process>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineType", machineType);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                processes.Add(MapToProcess(reader));

            return processes;
        }

        public async Task<IEnumerable<Process>> GetOutsourcedProcessesAsync()
        {
            const string query = "SELECT * FROM Masters_Processes WHERE IsOutsourced = 1 AND IsActive = 1 ORDER BY ProcessName";

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

        private static Process MapToProcess(SqlDataReader reader)
        {
            return new Process
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProcessCode = reader.GetString(reader.GetOrdinal("ProcessCode")),
                ProcessName = reader.GetString(reader.GetOrdinal("ProcessName")),
                ProcessType = reader.IsDBNull(reader.GetOrdinal("ProcessType")) ? null : reader.GetString(reader.GetOrdinal("ProcessType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                ProcessDetails = reader.IsDBNull(reader.GetOrdinal("ProcessDetails")) ? null : reader.GetString(reader.GetOrdinal("ProcessDetails")),
                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? null : reader.GetString(reader.GetOrdinal("MachineType")),
                DefaultMachineId = reader.IsDBNull(reader.GetOrdinal("DefaultMachineId")) ? null : reader.GetInt32(reader.GetOrdinal("DefaultMachineId")),
                DefaultMachineName = reader.IsDBNull(reader.GetOrdinal("DefaultMachineName")) ? null : reader.GetString(reader.GetOrdinal("DefaultMachineName")),
                StandardSetupTimeMin = reader.IsDBNull(reader.GetOrdinal("StandardSetupTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("StandardSetupTimeMin")),
                StandardCycleTimeMin = reader.IsDBNull(reader.GetOrdinal("StandardCycleTimeMin")) ? null : reader.GetInt32(reader.GetOrdinal("StandardCycleTimeMin")),
                StandardCycleTimePerPiece = reader.IsDBNull(reader.GetOrdinal("StandardCycleTimePerPiece")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardCycleTimePerPiece")),
                SkillLevel = reader.IsDBNull(reader.GetOrdinal("SkillLevel")) ? null : reader.GetString(reader.GetOrdinal("SkillLevel")),
                OperatorsRequired = reader.IsDBNull(reader.GetOrdinal("OperatorsRequired")) ? null : reader.GetInt32(reader.GetOrdinal("OperatorsRequired")),
                HourlyRate = reader.IsDBNull(reader.GetOrdinal("HourlyRate")) ? null : reader.GetDecimal(reader.GetOrdinal("HourlyRate")),
                StandardCostPerPiece = reader.IsDBNull(reader.GetOrdinal("StandardCostPerPiece")) ? null : reader.GetDecimal(reader.GetOrdinal("StandardCostPerPiece")),
                RequiresQC = reader.GetBoolean(reader.GetOrdinal("RequiresQC")),
                QCCheckpoints = reader.IsDBNull(reader.GetOrdinal("QCCheckpoints")) ? null : reader.GetString(reader.GetOrdinal("QCCheckpoints")),
                IsOutsourced = reader.GetBoolean(reader.GetOrdinal("IsOutsourced")),
                PreferredVendor = reader.IsDBNull(reader.GetOrdinal("PreferredVendor")) ? null : reader.GetString(reader.GetOrdinal("PreferredVendor")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddProcessParameters(SqlCommand command, Process process)
        {
            command.Parameters.AddWithValue("@Id", process.Id);
            command.Parameters.AddWithValue("@ProcessCode", process.ProcessCode);
            command.Parameters.AddWithValue("@ProcessName", process.ProcessName);
            command.Parameters.AddWithValue("@ProcessType", (object?)process.ProcessType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)process.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", (object?)process.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)process.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessDetails", (object?)process.ProcessDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineType", (object?)process.MachineType ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachineId", (object?)process.DefaultMachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DefaultMachineName", (object?)process.DefaultMachineName ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardSetupTimeMin", (object?)process.StandardSetupTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCycleTimeMin", (object?)process.StandardCycleTimeMin ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCycleTimePerPiece", (object?)process.StandardCycleTimePerPiece ?? DBNull.Value);
            command.Parameters.AddWithValue("@SkillLevel", (object?)process.SkillLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@OperatorsRequired", (object?)process.OperatorsRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@HourlyRate", (object?)process.HourlyRate ?? DBNull.Value);
            command.Parameters.AddWithValue("@StandardCostPerPiece", (object?)process.StandardCostPerPiece ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequiresQC", process.RequiresQC);
            command.Parameters.AddWithValue("@QCCheckpoints", (object?)process.QCCheckpoints ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsOutsourced", process.IsOutsourced);
            command.Parameters.AddWithValue("@PreferredVendor", (object?)process.PreferredVendor ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", process.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)process.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)process.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", process.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)process.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)process.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)process.UpdatedBy ?? DBNull.Value);
        }
    }
}
