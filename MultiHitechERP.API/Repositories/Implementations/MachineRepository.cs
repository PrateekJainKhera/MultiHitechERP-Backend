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

        public async Task<Machine?> GetByIdAsync(Guid id)
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

        public async Task<Guid> InsertAsync(Machine machine)
        {
            const string query = @"
                INSERT INTO Masters_Machines (
                    Id, MachineCode, MachineName, MachineType, Category,
                    Manufacturer, Model, SerialNumber, YearOfManufacture,
                    Capacity, CapacityUnit, Specifications,
                    MaxWorkpieceLength, MaxWorkpieceDiameter, ChuckSize,
                    Department, ShopFloor, Location,
                    HourlyRate, PowerConsumption, OperatorsRequired,
                    PurchaseDate, LastMaintenanceDate, NextMaintenanceDate, MaintenanceSchedule,
                    IsActive, Status, CurrentJobCardNo, IsAvailable, AvailableFrom,
                    Remarks, CreatedAt, CreatedBy
                ) VALUES (
                    @Id, @MachineCode, @MachineName, @MachineType, @Category,
                    @Manufacturer, @Model, @SerialNumber, @YearOfManufacture,
                    @Capacity, @CapacityUnit, @Specifications,
                    @MaxWorkpieceLength, @MaxWorkpieceDiameter, @ChuckSize,
                    @Department, @ShopFloor, @Location,
                    @HourlyRate, @PowerConsumption, @OperatorsRequired,
                    @PurchaseDate, @LastMaintenanceDate, @NextMaintenanceDate, @MaintenanceSchedule,
                    @IsActive, @Status, @CurrentJobCardNo, @IsAvailable, @AvailableFrom,
                    @Remarks, @CreatedAt, @CreatedBy
                )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            machine.Id = Guid.NewGuid();
            machine.CreatedAt = DateTime.UtcNow;
            AddMachineParameters(command, machine);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return machine.Id;
        }

        public async Task<bool> UpdateAsync(Machine machine)
        {
            const string query = @"
                UPDATE Masters_Machines SET
                    MachineName = @MachineName, MachineType = @MachineType, Category = @Category,
                    Manufacturer = @Manufacturer, Model = @Model, SerialNumber = @SerialNumber,
                    YearOfManufacture = @YearOfManufacture,
                    Capacity = @Capacity, CapacityUnit = @CapacityUnit, Specifications = @Specifications,
                    MaxWorkpieceLength = @MaxWorkpieceLength, MaxWorkpieceDiameter = @MaxWorkpieceDiameter,
                    ChuckSize = @ChuckSize, Department = @Department, ShopFloor = @ShopFloor, Location = @Location,
                    HourlyRate = @HourlyRate, PowerConsumption = @PowerConsumption, OperatorsRequired = @OperatorsRequired,
                    PurchaseDate = @PurchaseDate, LastMaintenanceDate = @LastMaintenanceDate,
                    NextMaintenanceDate = @NextMaintenanceDate, MaintenanceSchedule = @MaintenanceSchedule,
                    IsActive = @IsActive, Status = @Status, CurrentJobCardNo = @CurrentJobCardNo,
                    IsAvailable = @IsAvailable, AvailableFrom = @AvailableFrom,
                    Remarks = @Remarks, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            machine.UpdatedAt = DateTime.UtcNow;
            AddMachineParameters(command, machine);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Masters_Machines WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            const string query = "UPDATE Masters_Machines SET Status = @Status, UpdatedAt = @UpdatedAt WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAvailabilityAsync(Guid id, bool isAvailable, DateTime? availableFrom)
        {
            const string query = @"
                UPDATE Masters_Machines
                SET IsAvailable = @IsAvailable, AvailableFrom = @AvailableFrom, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@IsAvailable", isAvailable);
            command.Parameters.AddWithValue("@AvailableFrom", (object?)availableFrom ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> AssignToJobCardAsync(Guid id, string jobCardNo)
        {
            const string query = @"
                UPDATE Masters_Machines
                SET CurrentJobCardNo = @JobCardNo, IsAvailable = 0, Status = 'Busy', UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@JobCardNo", jobCardNo);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> ReleaseFromJobCardAsync(Guid id)
        {
            const string query = @"
                UPDATE Masters_Machines
                SET CurrentJobCardNo = NULL, IsAvailable = 1, Status = 'Available', UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<Machine>> GetAvailableMachinesAsync()
        {
            const string query = "SELECT * FROM Masters_Machines WHERE IsActive = 1 AND IsAvailable = 1 ORDER BY MachineName";

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

        public async Task<IEnumerable<Machine>> GetDueForMaintenanceAsync()
        {
            const string query = @"
                SELECT * FROM Masters_Machines
                WHERE IsActive = 1 AND NextMaintenanceDate IS NOT NULL AND NextMaintenanceDate <= GETDATE()
                ORDER BY NextMaintenanceDate";

            var machines = new List<Machine>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                machines.Add(MapToMachine(reader));

            return machines;
        }

        public async Task<bool> ExistsAsync(string machineCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Machines WHERE MachineCode = @MachineCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineCode", machineCode);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        private static Machine MapToMachine(SqlDataReader reader)
        {
            return new Machine
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                MachineCode = reader.GetString(reader.GetOrdinal("MachineCode")),
                MachineName = reader.GetString(reader.GetOrdinal("MachineName")),
                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? null : reader.GetString(reader.GetOrdinal("MachineType")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString(reader.GetOrdinal("Category")),
                Manufacturer = reader.IsDBNull(reader.GetOrdinal("Manufacturer")) ? null : reader.GetString(reader.GetOrdinal("Manufacturer")),
                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                SerialNumber = reader.IsDBNull(reader.GetOrdinal("SerialNumber")) ? null : reader.GetString(reader.GetOrdinal("SerialNumber")),
                YearOfManufacture = reader.IsDBNull(reader.GetOrdinal("YearOfManufacture")) ? null : reader.GetInt32(reader.GetOrdinal("YearOfManufacture")),
                Capacity = reader.IsDBNull(reader.GetOrdinal("Capacity")) ? null : reader.GetDecimal(reader.GetOrdinal("Capacity")),
                CapacityUnit = reader.IsDBNull(reader.GetOrdinal("CapacityUnit")) ? null : reader.GetString(reader.GetOrdinal("CapacityUnit")),
                Specifications = reader.IsDBNull(reader.GetOrdinal("Specifications")) ? null : reader.GetString(reader.GetOrdinal("Specifications")),
                MaxWorkpieceLength = reader.IsDBNull(reader.GetOrdinal("MaxWorkpieceLength")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxWorkpieceLength")),
                MaxWorkpieceDiameter = reader.IsDBNull(reader.GetOrdinal("MaxWorkpieceDiameter")) ? null : reader.GetDecimal(reader.GetOrdinal("MaxWorkpieceDiameter")),
                ChuckSize = reader.IsDBNull(reader.GetOrdinal("ChuckSize")) ? null : reader.GetDecimal(reader.GetOrdinal("ChuckSize")),
                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                ShopFloor = reader.IsDBNull(reader.GetOrdinal("ShopFloor")) ? null : reader.GetString(reader.GetOrdinal("ShopFloor")),
                Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : reader.GetString(reader.GetOrdinal("Location")),
                HourlyRate = reader.IsDBNull(reader.GetOrdinal("HourlyRate")) ? null : reader.GetDecimal(reader.GetOrdinal("HourlyRate")),
                PowerConsumption = reader.IsDBNull(reader.GetOrdinal("PowerConsumption")) ? null : reader.GetDecimal(reader.GetOrdinal("PowerConsumption")),
                OperatorsRequired = reader.IsDBNull(reader.GetOrdinal("OperatorsRequired")) ? null : reader.GetInt32(reader.GetOrdinal("OperatorsRequired")),
                PurchaseDate = reader.IsDBNull(reader.GetOrdinal("PurchaseDate")) ? null : reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                LastMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("LastMaintenanceDate")) ? null : reader.GetDateTime(reader.GetOrdinal("LastMaintenanceDate")),
                NextMaintenanceDate = reader.IsDBNull(reader.GetOrdinal("NextMaintenanceDate")) ? null : reader.GetDateTime(reader.GetOrdinal("NextMaintenanceDate")),
                MaintenanceSchedule = reader.IsDBNull(reader.GetOrdinal("MaintenanceSchedule")) ? null : reader.GetString(reader.GetOrdinal("MaintenanceSchedule")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                CurrentJobCardNo = reader.IsDBNull(reader.GetOrdinal("CurrentJobCardNo")) ? null : reader.GetString(reader.GetOrdinal("CurrentJobCardNo")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                AvailableFrom = reader.IsDBNull(reader.GetOrdinal("AvailableFrom")) ? null : reader.GetDateTime(reader.GetOrdinal("AvailableFrom")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddMachineParameters(SqlCommand command, Machine machine)
        {
            command.Parameters.AddWithValue("@Id", machine.Id);
            command.Parameters.AddWithValue("@MachineCode", machine.MachineCode);
            command.Parameters.AddWithValue("@MachineName", machine.MachineName);
            command.Parameters.AddWithValue("@MachineType", (object?)machine.MachineType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)machine.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Manufacturer", (object?)machine.Manufacturer ?? DBNull.Value);
            command.Parameters.AddWithValue("@Model", (object?)machine.Model ?? DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", (object?)machine.SerialNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@YearOfManufacture", (object?)machine.YearOfManufacture ?? DBNull.Value);
            command.Parameters.AddWithValue("@Capacity", (object?)machine.Capacity ?? DBNull.Value);
            command.Parameters.AddWithValue("@CapacityUnit", (object?)machine.CapacityUnit ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specifications", (object?)machine.Specifications ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaxWorkpieceLength", (object?)machine.MaxWorkpieceLength ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaxWorkpieceDiameter", (object?)machine.MaxWorkpieceDiameter ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChuckSize", (object?)machine.ChuckSize ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", (object?)machine.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@ShopFloor", (object?)machine.ShopFloor ?? DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)machine.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@HourlyRate", (object?)machine.HourlyRate ?? DBNull.Value);
            command.Parameters.AddWithValue("@PowerConsumption", (object?)machine.PowerConsumption ?? DBNull.Value);
            command.Parameters.AddWithValue("@OperatorsRequired", (object?)machine.OperatorsRequired ?? DBNull.Value);
            command.Parameters.AddWithValue("@PurchaseDate", (object?)machine.PurchaseDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastMaintenanceDate", (object?)machine.LastMaintenanceDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@NextMaintenanceDate", (object?)machine.NextMaintenanceDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaintenanceSchedule", (object?)machine.MaintenanceSchedule ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", machine.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)machine.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentJobCardNo", (object?)machine.CurrentJobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsAvailable", machine.IsAvailable);
            command.Parameters.AddWithValue("@AvailableFrom", (object?)machine.AvailableFrom ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)machine.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", machine.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)machine.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)machine.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)machine.UpdatedBy ?? DBNull.Value);
        }
    }
}
