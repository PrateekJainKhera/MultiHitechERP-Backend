using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Operator repository implementation using ADO.NET
    /// </summary>
    public class OperatorRepository : IOperatorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OperatorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Operator?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Masters_Operators WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToOperator(reader) : null;
        }

        public async Task<Operator?> GetByOperatorCodeAsync(string operatorCode)
        {
            const string query = "SELECT * FROM Masters_Operators WHERE OperatorCode = @OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OperatorCode", operatorCode);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToOperator(reader) : null;
        }

        public async Task<IEnumerable<Operator>> GetAllAsync()
        {
            const string query = "SELECT * FROM Masters_Operators ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetActiveOperatorsAsync()
        {
            const string query = "SELECT * FROM Masters_Operators WHERE IsActive = 1 ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetAvailableOperatorsAsync()
        {
            const string query = "SELECT * FROM Masters_Operators WHERE IsActive = 1 AND IsAvailable = 1 ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetByDepartmentAsync(string department)
        {
            const string query = "SELECT * FROM Masters_Operators WHERE Department = @Department AND IsActive = 1 ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Department", department);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetByShiftAsync(string shift)
        {
            const string query = "SELECT * FROM Masters_Operators WHERE Shift = @Shift AND IsActive = 1 ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Shift", shift);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetBySkillLevelAsync(string skillLevel)
        {
            const string query = "SELECT * FROM Masters_Operators WHERE SkillLevel = @SkillLevel AND IsActive = 1 ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SkillLevel", skillLevel);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<IEnumerable<Operator>> GetByMachineExpertiseAsync(int machineId)
        {
            const string query = @"
                SELECT * FROM Masters_Operators
                WHERE MachineExpertise LIKE '%' + @MachineId + '%'
                AND IsActive = 1
                ORDER BY OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MachineId", machineId.ToString());

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var operators = new List<Operator>();
            while (await reader.ReadAsync())
            {
                operators.Add(MapToOperator(reader));
            }

            return operators;
        }

        public async Task<int> InsertAsync(Operator operatorEntity)
        {
            const string query = @"
                INSERT INTO Masters_Operators
                (Id, OperatorCode, OperatorName, Email, Phone, Mobile, EmployeeId, JoiningDate, Designation, Department,
                 ShopFloor, SkillLevel, Specialization, CertificationDetails, MachineExpertise, ProcessExpertise,
                 Shift, WorkingHours, EfficiencyRating, QualityRating, HourlyRate, MonthlySalary,
                 IsActive, Status, IsAvailable, CurrentJobCardId, CurrentJobCardNo, CurrentMachineId,
                 Remarks, CreatedAt, CreatedBy)
                VALUES
                (@Id, @OperatorCode, @OperatorName, @Email, @Phone, @Mobile, @EmployeeId, @JoiningDate, @Designation, @Department,
                 @ShopFloor, @SkillLevel, @Specialization, @CertificationDetails, @MachineExpertise, @ProcessExpertise,
                 @Shift, @WorkingHours, @EfficiencyRating, @QualityRating, @HourlyRate, @MonthlySalary,
                 @IsActive, @Status, @IsAvailable, @CurrentJobCardId, @CurrentJobCardNo, @CurrentMachineId,
                 @Remarks, @CreatedAt, @CreatedBy)";

            var operatorId = 0;
            operatorEntity.Id = operatorId;
            operatorEntity.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddOperatorParameters(command, operatorEntity);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return operatorId;
        }

        public async Task<bool> UpdateAsync(Operator operatorEntity)
        {
            const string query = @"
                UPDATE Masters_Operators SET
                    OperatorCode = @OperatorCode,
                    OperatorName = @OperatorName,
                    Email = @Email,
                    Phone = @Phone,
                    Mobile = @Mobile,
                    EmployeeId = @EmployeeId,
                    JoiningDate = @JoiningDate,
                    Designation = @Designation,
                    Department = @Department,
                    ShopFloor = @ShopFloor,
                    SkillLevel = @SkillLevel,
                    Specialization = @Specialization,
                    CertificationDetails = @CertificationDetails,
                    MachineExpertise = @MachineExpertise,
                    ProcessExpertise = @ProcessExpertise,
                    Shift = @Shift,
                    WorkingHours = @WorkingHours,
                    EfficiencyRating = @EfficiencyRating,
                    QualityRating = @QualityRating,
                    HourlyRate = @HourlyRate,
                    MonthlySalary = @MonthlySalary,
                    IsActive = @IsActive,
                    Status = @Status,
                    IsAvailable = @IsAvailable,
                    CurrentJobCardId = @CurrentJobCardId,
                    CurrentJobCardNo = @CurrentJobCardNo,
                    CurrentMachineId = @CurrentMachineId,
                    Remarks = @Remarks,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id";

            operatorEntity.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddOperatorParameters(command, operatorEntity);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Masters_Operators WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Masters_Operators
                SET Status = @Status,
                    IsActive = CASE WHEN @Status = 'Active' THEN 1 ELSE 0 END,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateAvailabilityAsync(int id, bool isAvailable)
        {
            const string query = @"
                UPDATE Masters_Operators
                SET IsAvailable = @IsAvailable, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@IsAvailable", isAvailable);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> AssignToJobCardAsync(int id, int jobCardId, string jobCardNo, int? machineId)
        {
            const string query = @"
                UPDATE Masters_Operators
                SET CurrentJobCardId = @JobCardId,
                    CurrentJobCardNo = @JobCardNo,
                    CurrentMachineId = @MachineId,
                    IsAvailable = 0,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);
            command.Parameters.AddWithValue("@JobCardNo", jobCardNo);
            command.Parameters.AddWithValue("@MachineId", (object?)machineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ReleaseFromJobCardAsync(int id)
        {
            const string query = @"
                UPDATE Masters_Operators
                SET CurrentJobCardId = NULL,
                    CurrentJobCardNo = NULL,
                    CurrentMachineId = NULL,
                    IsAvailable = 1,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string operatorCode)
        {
            const string query = "SELECT COUNT(1) FROM Masters_Operators WHERE OperatorCode = @OperatorCode";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OperatorCode", operatorCode);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        // Helper Methods
        private static Operator MapToOperator(SqlDataReader reader)
        {
            return new Operator
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                OperatorCode = reader.GetString(reader.GetOrdinal("OperatorCode")),
                OperatorName = reader.GetString(reader.GetOrdinal("OperatorName")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Mobile = reader.IsDBNull(reader.GetOrdinal("Mobile")) ? null : reader.GetString(reader.GetOrdinal("Mobile")),
                EmployeeId = reader.IsDBNull(reader.GetOrdinal("EmployeeId")) ? null : reader.GetString(reader.GetOrdinal("EmployeeId")),
                JoiningDate = reader.IsDBNull(reader.GetOrdinal("JoiningDate")) ? null : reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                Designation = reader.IsDBNull(reader.GetOrdinal("Designation")) ? null : reader.GetString(reader.GetOrdinal("Designation")),
                Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department")),
                ShopFloor = reader.IsDBNull(reader.GetOrdinal("ShopFloor")) ? null : reader.GetString(reader.GetOrdinal("ShopFloor")),
                SkillLevel = reader.IsDBNull(reader.GetOrdinal("SkillLevel")) ? null : reader.GetString(reader.GetOrdinal("SkillLevel")),
                Specialization = reader.IsDBNull(reader.GetOrdinal("Specialization")) ? null : reader.GetString(reader.GetOrdinal("Specialization")),
                CertificationDetails = reader.IsDBNull(reader.GetOrdinal("CertificationDetails")) ? null : reader.GetString(reader.GetOrdinal("CertificationDetails")),
                MachineExpertise = reader.IsDBNull(reader.GetOrdinal("MachineExpertise")) ? null : reader.GetString(reader.GetOrdinal("MachineExpertise")),
                ProcessExpertise = reader.IsDBNull(reader.GetOrdinal("ProcessExpertise")) ? null : reader.GetString(reader.GetOrdinal("ProcessExpertise")),
                Shift = reader.IsDBNull(reader.GetOrdinal("Shift")) ? null : reader.GetString(reader.GetOrdinal("Shift")),
                WorkingHours = reader.IsDBNull(reader.GetOrdinal("WorkingHours")) ? null : reader.GetString(reader.GetOrdinal("WorkingHours")),
                EfficiencyRating = reader.IsDBNull(reader.GetOrdinal("EfficiencyRating")) ? null : reader.GetDecimal(reader.GetOrdinal("EfficiencyRating")),
                QualityRating = reader.IsDBNull(reader.GetOrdinal("QualityRating")) ? null : reader.GetDecimal(reader.GetOrdinal("QualityRating")),
                HourlyRate = reader.IsDBNull(reader.GetOrdinal("HourlyRate")) ? null : reader.GetDecimal(reader.GetOrdinal("HourlyRate")),
                MonthlySalary = reader.IsDBNull(reader.GetOrdinal("MonthlySalary")) ? null : reader.GetDecimal(reader.GetOrdinal("MonthlySalary")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? null : reader.GetString(reader.GetOrdinal("Status")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                CurrentJobCardId = reader.IsDBNull(reader.GetOrdinal("CurrentJobCardId")) ? null : reader.GetInt32(reader.GetOrdinal("CurrentJobCardId")),
                CurrentJobCardNo = reader.IsDBNull(reader.GetOrdinal("CurrentJobCardNo")) ? null : reader.GetString(reader.GetOrdinal("CurrentJobCardNo")),
                CurrentMachineId = reader.IsDBNull(reader.GetOrdinal("CurrentMachineId")) ? null : reader.GetInt32(reader.GetOrdinal("CurrentMachineId")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString(reader.GetOrdinal("UpdatedBy"))
            };
        }

        private static void AddOperatorParameters(SqlCommand command, Operator operatorEntity)
        {
            command.Parameters.AddWithValue("@Id", operatorEntity.Id);
            command.Parameters.AddWithValue("@OperatorCode", operatorEntity.OperatorCode);
            command.Parameters.AddWithValue("@OperatorName", operatorEntity.OperatorName);
            command.Parameters.AddWithValue("@Email", (object?)operatorEntity.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object?)operatorEntity.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Mobile", (object?)operatorEntity.Mobile ?? DBNull.Value);
            command.Parameters.AddWithValue("@EmployeeId", (object?)operatorEntity.EmployeeId ?? DBNull.Value);
            command.Parameters.AddWithValue("@JoiningDate", (object?)operatorEntity.JoiningDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Designation", (object?)operatorEntity.Designation ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", (object?)operatorEntity.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@ShopFloor", (object?)operatorEntity.ShopFloor ?? DBNull.Value);
            command.Parameters.AddWithValue("@SkillLevel", (object?)operatorEntity.SkillLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@Specialization", (object?)operatorEntity.Specialization ?? DBNull.Value);
            command.Parameters.AddWithValue("@CertificationDetails", (object?)operatorEntity.CertificationDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@MachineExpertise", (object?)operatorEntity.MachineExpertise ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessExpertise", (object?)operatorEntity.ProcessExpertise ?? DBNull.Value);
            command.Parameters.AddWithValue("@Shift", (object?)operatorEntity.Shift ?? DBNull.Value);
            command.Parameters.AddWithValue("@WorkingHours", (object?)operatorEntity.WorkingHours ?? DBNull.Value);
            command.Parameters.AddWithValue("@EfficiencyRating", (object?)operatorEntity.EfficiencyRating ?? DBNull.Value);
            command.Parameters.AddWithValue("@QualityRating", (object?)operatorEntity.QualityRating ?? DBNull.Value);
            command.Parameters.AddWithValue("@HourlyRate", (object?)operatorEntity.HourlyRate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MonthlySalary", (object?)operatorEntity.MonthlySalary ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", operatorEntity.IsActive);
            command.Parameters.AddWithValue("@Status", (object?)operatorEntity.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsAvailable", operatorEntity.IsAvailable);
            command.Parameters.AddWithValue("@CurrentJobCardId", (object?)operatorEntity.CurrentJobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentJobCardNo", (object?)operatorEntity.CurrentJobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentMachineId", (object?)operatorEntity.CurrentMachineId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)operatorEntity.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", operatorEntity.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)operatorEntity.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)operatorEntity.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)operatorEntity.UpdatedBy ?? DBNull.Value);
        }
    }
}
