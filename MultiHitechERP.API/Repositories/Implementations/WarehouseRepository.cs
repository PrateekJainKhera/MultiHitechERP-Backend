using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public WarehouseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            const string query = @"
                SELECT Id, Name, Rack, RackNo, MaterialType, MinStockPieces, MinStockLengthMM, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Stores_MasterWarehouses
                ORDER BY Name, Rack, RackNo";

            var results = new List<Warehouse>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                results.Add(MapToModel(reader));
            return results;
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, Name, Rack, RackNo, MaterialType, MinStockPieces, MinStockLengthMM, IsActive,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Stores_MasterWarehouses
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapToModel(reader) : null;
        }

        public async Task<int> CreateAsync(Warehouse warehouse)
        {
            const string query = @"
                INSERT INTO Stores_MasterWarehouses
                    (Name, Rack, RackNo, MaterialType, MinStockPieces, MinStockLengthMM, IsActive,
                     CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                VALUES
                    (@Name, @Rack, @RackNo, @MaterialType, @MinStockPieces, @MinStockLengthMM, @IsActive,
                     @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            AddParameters(command, warehouse);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> UpdateAsync(Warehouse warehouse)
        {
            const string query = @"
                UPDATE Stores_MasterWarehouses SET
                    Name              = @Name,
                    Rack              = @Rack,
                    RackNo            = @RackNo,
                    MaterialType      = @MaterialType,
                    MinStockPieces    = @MinStockPieces,
                    MinStockLengthMM  = @MinStockLengthMM,
                    IsActive          = @IsActive,
                    UpdatedAt         = @UpdatedAt,
                    UpdatedBy         = @UpdatedBy
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", warehouse.Id);
            AddParameters(command, warehouse);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Stores_MasterWarehouses WHERE Id = @Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<LowStockAlertResponse>> GetLowStockStatusAsync()
        {
            const string query = @"
                SELECT
                    w.Id              AS WarehouseId,
                    w.Name            AS WarehouseName,
                    w.Rack,
                    w.RackNo,
                    w.MaterialType,
                    w.MinStockPieces,
                    w.MinStockLengthMM,
                    COUNT(mp.Id)                       AS CurrentPieces,
                    ISNULL(SUM(mp.CurrentLengthMM), 0) AS CurrentLengthMM
                FROM Stores_MasterWarehouses w
                LEFT JOIN Stores_MaterialPieces mp
                    ON mp.WarehouseId = w.Id
                    AND mp.Status = 'Available'
                    AND mp.IsWastage = 0
                WHERE w.IsActive = 1
                GROUP BY w.Id, w.Name, w.Rack, w.RackNo, w.MaterialType, w.MinStockPieces, w.MinStockLengthMM
                ORDER BY w.Name, w.Rack, w.RackNo";

            var results = new List<LowStockAlertResponse>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var currentPieces    = reader.GetInt32(reader.GetOrdinal("CurrentPieces"));
                var currentLengthMM  = reader.GetDecimal(reader.GetOrdinal("CurrentLengthMM"));
                var minPieces        = reader.GetInt32(reader.GetOrdinal("MinStockPieces"));
                var minLengthMM      = reader.GetDecimal(reader.GetOrdinal("MinStockLengthMM"));

                results.Add(new LowStockAlertResponse
                {
                    WarehouseId      = reader.GetInt32(reader.GetOrdinal("WarehouseId")),
                    WarehouseName    = reader.GetString(reader.GetOrdinal("WarehouseName")),
                    Rack             = reader.GetString(reader.GetOrdinal("Rack")),
                    RackNo           = reader.GetString(reader.GetOrdinal("RackNo")),
                    MaterialType     = reader.GetString(reader.GetOrdinal("MaterialType")),
                    CurrentPieces    = currentPieces,
                    CurrentLengthMM  = currentLengthMM,
                    MinStockPieces   = minPieces,
                    MinStockLengthMM = minLengthMM,
                    PiecesAlert      = minPieces > 0 && currentPieces < minPieces,
                    LengthAlert      = minLengthMM > 0 && currentLengthMM < minLengthMM,
                });
            }
            return results;
        }

        private static void AddParameters(SqlCommand command, Warehouse w)
        {
            command.Parameters.AddWithValue("@Name", w.Name);
            command.Parameters.AddWithValue("@Rack", w.Rack);
            command.Parameters.AddWithValue("@RackNo", w.RackNo);
            command.Parameters.AddWithValue("@MaterialType", w.MaterialType);
            command.Parameters.AddWithValue("@MinStockPieces", w.MinStockPieces);
            command.Parameters.AddWithValue("@MinStockLengthMM", w.MinStockLengthMM);
            command.Parameters.AddWithValue("@IsActive", w.IsActive);
            command.Parameters.AddWithValue("@CreatedBy", (object?)w.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)w.UpdatedBy ?? DBNull.Value);
        }

        private static Warehouse MapToModel(SqlDataReader r)
        {
            return new Warehouse
            {
                Id                = r.GetInt32(r.GetOrdinal("Id")),
                Name              = r.GetString(r.GetOrdinal("Name")),
                Rack              = r.GetString(r.GetOrdinal("Rack")),
                RackNo            = r.GetString(r.GetOrdinal("RackNo")),
                MaterialType      = r.GetString(r.GetOrdinal("MaterialType")),
                MinStockPieces    = r.GetInt32(r.GetOrdinal("MinStockPieces")),
                MinStockLengthMM  = r.GetDecimal(r.GetOrdinal("MinStockLengthMM")),
                IsActive          = r.GetBoolean(r.GetOrdinal("IsActive")),
                CreatedAt         = r.GetDateTime(r.GetOrdinal("CreatedAt")),
                CreatedBy         = r.IsDBNull(r.GetOrdinal("CreatedBy")) ? null : r.GetString(r.GetOrdinal("CreatedBy")),
                UpdatedAt         = r.GetDateTime(r.GetOrdinal("UpdatedAt")),
                UpdatedBy         = r.IsDBNull(r.GetOrdinal("UpdatedBy")) ? null : r.GetString(r.GetOrdinal("UpdatedBy")),
            };
        }
    }
}
