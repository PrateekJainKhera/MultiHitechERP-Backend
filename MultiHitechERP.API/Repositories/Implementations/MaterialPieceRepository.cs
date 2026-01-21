using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// MaterialPiece repository implementation using ADO.NET
    /// Handles length-based material tracking with FIFO allocation
    /// </summary>
    public class MaterialPieceRepository : IMaterialPieceRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialPieceRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MaterialPiece?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT * FROM Stores_MaterialPieces WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToPiece(reader) : null;
        }

        public async Task<MaterialPiece?> GetByPieceNoAsync(string pieceNo)
        {
            const string query = "SELECT * FROM Stores_MaterialPieces WHERE PieceNo = @PieceNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PieceNo", pieceNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToPiece(reader) : null;
        }

        public async Task<IEnumerable<MaterialPiece>> GetAllAsync()
        {
            const string query = "SELECT * FROM Stores_MaterialPieces ORDER BY ReceivedDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetByMaterialIdAsync(Guid materialId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId
                ORDER BY ReceivedDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE Status = @Status
                ORDER BY ReceivedDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<Guid> InsertAsync(MaterialPiece piece)
        {
            const string query = @"
                INSERT INTO Stores_MaterialPieces
                (Id, MaterialId, PieceNo, OriginalLengthMM, CurrentLengthMM, OriginalWeightKG, CurrentWeightKG,
                 Status, AllocatedToRequisitionId, IssuedToJobCardId, StorageLocation, BinNumber, RackNumber,
                 GRNNo, ReceivedDate, SupplierBatchNo, SupplierId, UnitCost, CreatedAt, UpdatedAt)
                VALUES
                (@Id, @MaterialId, @PieceNo, @OriginalLengthMM, @CurrentLengthMM, @OriginalWeightKG, @CurrentWeightKG,
                 @Status, @AllocatedToRequisitionId, @IssuedToJobCardId, @StorageLocation, @BinNumber, @RackNumber,
                 @GRNNo, @ReceivedDate, @SupplierBatchNo, @SupplierId, @UnitCost, @CreatedAt, @UpdatedAt)";

            var pieceId = Guid.NewGuid();
            piece.Id = pieceId;
            piece.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddPieceParameters(command, piece);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return pieceId;
        }

        public async Task<bool> UpdateAsync(MaterialPiece piece)
        {
            const string query = @"
                UPDATE Stores_MaterialPieces SET
                    MaterialId = @MaterialId,
                    PieceNo = @PieceNo,
                    OriginalLengthMM = @OriginalLengthMM,
                    CurrentLengthMM = @CurrentLengthMM,
                    OriginalWeightKG = @OriginalWeightKG,
                    CurrentWeightKG = @CurrentWeightKG,
                    Status = @Status,
                    AllocatedToRequisitionId = @AllocatedToRequisitionId,
                    IssuedToJobCardId = @IssuedToJobCardId,
                    StorageLocation = @StorageLocation,
                    BinNumber = @BinNumber,
                    RackNumber = @RackNumber,
                    GRNNo = @GRNNo,
                    ReceivedDate = @ReceivedDate,
                    SupplierBatchNo = @SupplierBatchNo,
                    SupplierId = @SupplierId,
                    UnitCost = @UnitCost,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            piece.UpdatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddPieceParameters(command, piece);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Stores_MaterialPieces WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> AllocatePieceAsync(Guid id, Guid requisitionId)
        {
            const string query = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Allocated',
                    AllocatedToRequisitionId = @RequisitionId,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND Status = 'Available'";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> IssuePieceAsync(Guid id, Guid jobCardId, DateTime issuedDate, string issuedBy)
        {
            const string query = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Issued',
                    IssuedToJobCardId = @JobCardId,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND (Status = 'Available' OR Status = 'Allocated')";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ConsumePieceAsync(Guid id, decimal consumedLengthMM, decimal consumedWeightKG)
        {
            const string query = @"
                UPDATE Stores_MaterialPieces
                SET CurrentLengthMM = CurrentLengthMM - @ConsumedLengthMM,
                    CurrentWeightKG = CurrentWeightKG - @ConsumedWeightKG,
                    Status = CASE
                        WHEN (CurrentLengthMM - @ConsumedLengthMM) <= 0 THEN 'Consumed'
                        ELSE Status
                    END,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
                AND CurrentLengthMM >= @ConsumedLengthMM
                AND CurrentWeightKG >= @ConsumedWeightKG";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ConsumedLengthMM", consumedLengthMM);
            command.Parameters.AddWithValue("@ConsumedWeightKG", consumedWeightKG);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ReturnPieceAsync(Guid id)
        {
            const string query = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Available',
                    AllocatedToRequisitionId = NULL,
                    IssuedToJobCardId = NULL,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND (Status = 'Allocated' OR Status = 'Issued')";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<MaterialPiece>> GetAvailablePiecesAsync(Guid materialId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId
                AND Status = 'Available'
                AND CurrentLengthMM > 0
                ORDER BY ReceivedDate ASC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetAllocatedPiecesAsync(Guid requisitionId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE AllocatedToRequisitionId = @RequisitionId
                ORDER BY ReceivedDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetIssuedPiecesAsync(Guid jobCardId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE IssuedToJobCardId = @JobCardId
                ORDER BY ReceivedDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetPiecesByLocationAsync(string location)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE StorageLocation = @Location
                ORDER BY BinNumber, RackNumber, ReceivedDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Location", location);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByFIFOAsync(Guid materialId, decimal requiredLengthMM)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId
                AND Status = 'Available'
                AND CurrentLengthMM >= @RequiredLengthMM
                ORDER BY ReceivedDate ASC, CurrentLengthMM ASC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);
            command.Parameters.AddWithValue("@RequiredLengthMM", requiredLengthMM);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var pieces = new List<MaterialPiece>();
            while (await reader.ReadAsync())
            {
                pieces.Add(MapToPiece(reader));
            }

            return pieces;
        }

        public async Task<decimal> GetAvailableQuantityAsync(Guid materialId)
        {
            const string query = @"
                SELECT ISNULL(SUM(CurrentLengthMM), 0) AS TotalLength
                FROM Stores_MaterialPieces
                WHERE MaterialId = @MaterialId
                AND Status = 'Available'
                AND CurrentLengthMM > 0";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaterialId", materialId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToDecimal(result) : 0m;
        }

        // Helper Methods
        private static MaterialPiece MapToPiece(SqlDataReader reader)
        {
            return new MaterialPiece
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                MaterialId = reader.GetGuid(reader.GetOrdinal("MaterialId")),
                PieceNo = reader.GetString(reader.GetOrdinal("PieceNo")),
                OriginalLengthMM = reader.GetDecimal(reader.GetOrdinal("OriginalLengthMM")),
                CurrentLengthMM = reader.GetDecimal(reader.GetOrdinal("CurrentLengthMM")),
                OriginalWeightKG = reader.GetDecimal(reader.GetOrdinal("OriginalWeightKG")),
                CurrentWeightKG = reader.GetDecimal(reader.GetOrdinal("CurrentWeightKG")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AllocatedToRequisitionId = reader.IsDBNull(reader.GetOrdinal("AllocatedToRequisitionId"))
                    ? null : reader.GetGuid(reader.GetOrdinal("AllocatedToRequisitionId")),
                IssuedToJobCardId = reader.IsDBNull(reader.GetOrdinal("IssuedToJobCardId"))
                    ? null : reader.GetGuid(reader.GetOrdinal("IssuedToJobCardId")),
                StorageLocation = reader.IsDBNull(reader.GetOrdinal("StorageLocation"))
                    ? null : reader.GetString(reader.GetOrdinal("StorageLocation")),
                BinNumber = reader.IsDBNull(reader.GetOrdinal("BinNumber"))
                    ? null : reader.GetString(reader.GetOrdinal("BinNumber")),
                RackNumber = reader.IsDBNull(reader.GetOrdinal("RackNumber"))
                    ? null : reader.GetString(reader.GetOrdinal("RackNumber")),
                GRNNo = reader.IsDBNull(reader.GetOrdinal("GRNNo"))
                    ? null : reader.GetString(reader.GetOrdinal("GRNNo")),
                ReceivedDate = reader.GetDateTime(reader.GetOrdinal("ReceivedDate")),
                SupplierBatchNo = reader.IsDBNull(reader.GetOrdinal("SupplierBatchNo"))
                    ? null : reader.GetString(reader.GetOrdinal("SupplierBatchNo")),
                SupplierId = reader.IsDBNull(reader.GetOrdinal("SupplierId"))
                    ? null : reader.GetGuid(reader.GetOrdinal("SupplierId")),
                UnitCost = reader.IsDBNull(reader.GetOrdinal("UnitCost"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }

        private static void AddPieceParameters(SqlCommand command, MaterialPiece piece)
        {
            command.Parameters.AddWithValue("@Id", piece.Id);
            command.Parameters.AddWithValue("@MaterialId", piece.MaterialId);
            command.Parameters.AddWithValue("@PieceNo", piece.PieceNo);
            command.Parameters.AddWithValue("@OriginalLengthMM", piece.OriginalLengthMM);
            command.Parameters.AddWithValue("@CurrentLengthMM", piece.CurrentLengthMM);
            command.Parameters.AddWithValue("@OriginalWeightKG", piece.OriginalWeightKG);
            command.Parameters.AddWithValue("@CurrentWeightKG", piece.CurrentWeightKG);
            command.Parameters.AddWithValue("@Status", piece.Status);
            command.Parameters.AddWithValue("@AllocatedToRequisitionId", (object?)piece.AllocatedToRequisitionId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IssuedToJobCardId", (object?)piece.IssuedToJobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@StorageLocation", (object?)piece.StorageLocation ?? DBNull.Value);
            command.Parameters.AddWithValue("@BinNumber", (object?)piece.BinNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@RackNumber", (object?)piece.RackNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@GRNNo", (object?)piece.GRNNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReceivedDate", piece.ReceivedDate);
            command.Parameters.AddWithValue("@SupplierBatchNo", (object?)piece.SupplierBatchNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@SupplierId", (object?)piece.SupplierId ?? DBNull.Value);
            command.Parameters.AddWithValue("@UnitCost", (object?)piece.UnitCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", piece.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)piece.UpdatedAt ?? DBNull.Value);
        }
    }
}
