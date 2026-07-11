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
    /// MaterialRequisition repository implementation using ADO.NET
    /// </summary>
    public class MaterialRequisitionRepository : IMaterialRequisitionRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MaterialRequisitionRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MaterialRequisition?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM Stores_MaterialRequisitions WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToRequisition(reader) : null;
        }

        public async Task<MaterialRequisition?> GetByRequisitionNoAsync(string requisitionNo)
        {
            const string query = "SELECT * FROM Stores_MaterialRequisitions WHERE RequisitionNo = @RequisitionNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionNo", requisitionNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            return await reader.ReadAsync() ? MapToRequisition(reader) : null;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetAllAsync()
        {
            const string query = "SELECT * FROM Stores_MaterialRequisitions ORDER BY RequisitionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetByJobCardIdAsync(int jobCardId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE JobCardId = @JobCardId
                ORDER BY RequisitionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@JobCardId", jobCardId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetByOrderIdAsync(int orderId)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE OrderId = @OrderId
                ORDER BY RequisitionDate DESC";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE Status = @Status
                ORDER BY Priority DESC, RequisitionDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetPendingRequisitionsAsync()
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE Status = 'Pending'
                ORDER BY Priority DESC, RequisitionDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetApprovedRequisitionsAsync()
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE Status = 'Approved'
                ORDER BY Priority DESC, DueDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetByPriorityAsync(string priority)
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE Priority = @Priority AND Status IN ('Pending', 'Approved')
                ORDER BY RequisitionDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Priority", priority);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<IEnumerable<MaterialRequisition>> GetOverdueRequisitionsAsync()
        {
            const string query = @"
                SELECT * FROM Stores_MaterialRequisitions
                WHERE DueDate < @CurrentDate
                AND Status NOT IN ('Completed', 'Cancelled', 'Rejected')
                ORDER BY DueDate";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CurrentDate", DateTime.UtcNow);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var requisitions = new List<MaterialRequisition>();
            while (await reader.ReadAsync())
            {
                requisitions.Add(MapToRequisition(reader));
            }

            return requisitions;
        }

        public async Task<int> InsertAsync(MaterialRequisition requisition)
        {
            const string query = @"
                INSERT INTO Stores_MaterialRequisitions
                (RequisitionNo, RequisitionDate, JobCardId, JobCardNo, OrderId, OrderNo, OrderItemId, ItemSequence, CustomerName,
                 Status, Priority, DueDate, RequestedBy, ApprovedBy, ApprovalDate, Remarks, CreatedAt, CreatedBy)
                VALUES
                (@RequisitionNo, @RequisitionDate, @JobCardId, @JobCardNo, @OrderId, @OrderNo, @OrderItemId, @ItemSequence, @CustomerName,
                 @Status, @Priority, @DueDate, @RequestedBy, @ApprovedBy, @ApprovalDate, @Remarks, @CreatedAt, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            requisition.CreatedAt = DateTime.UtcNow;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddRequisitionParameters(command, requisition);

            await connection.OpenAsync();
            var requisitionId = (int)await command.ExecuteScalarAsync();

            return requisitionId;
        }

        public async Task<bool> UpdateAsync(MaterialRequisition requisition)
        {
            const string query = @"
                UPDATE Stores_MaterialRequisitions SET
                    RequisitionNo = @RequisitionNo,
                    RequisitionDate = @RequisitionDate,
                    JobCardId = @JobCardId,
                    JobCardNo = @JobCardNo,
                    OrderId = @OrderId,
                    OrderNo = @OrderNo,
                    OrderItemId = @OrderItemId,
                    ItemSequence = @ItemSequence,
                    CustomerName = @CustomerName,
                    Status = @Status,
                    Priority = @Priority,
                    DueDate = @DueDate,
                    RequestedBy = @RequestedBy,
                    ApprovedBy = @ApprovedBy,
                    ApprovalDate = @ApprovalDate,
                    Remarks = @Remarks
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            AddRequisitionParameters(command, requisition);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Stores_MaterialRequisitions WHERE Id = @Id";

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
                UPDATE Stores_MaterialRequisitions
                SET Status = @Status
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> ApproveRequisitionAsync(int id, string approvedBy)
        {
            const string query = @"
                UPDATE Stores_MaterialRequisitions
                SET Status = 'Approved',
                    ApprovedBy = @ApprovedBy,
                    ApprovalDate = @ApprovalDate
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@ApprovedBy", approvedBy);
            command.Parameters.AddWithValue("@ApprovalDate", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> RejectRequisitionAsync(int id, string rejectedBy, string? reason)
        {
            const string query = @"
                UPDATE Stores_MaterialRequisitions
                SET Status = 'Rejected',
                    ApprovedBy = @RejectedBy,
                    ApprovalDate = @RejectionDate,
                    Remarks = CONCAT(ISNULL(Remarks, ''), ' [Rejected: ', @Reason, ']')
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@RejectedBy", rejectedBy);
            command.Parameters.AddWithValue("@RejectionDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Reason", reason ?? "No reason provided");

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // Helper Methods
        private static MaterialRequisition MapToRequisition(SqlDataReader reader)
        {
            return new MaterialRequisition
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                RequisitionNo = reader.GetString(reader.GetOrdinal("RequisitionNo")),
                RequisitionDate = reader.GetDateTime(reader.GetOrdinal("RequisitionDate")),
                JobCardId = reader.IsDBNull(reader.GetOrdinal("JobCardId")) ? null : reader.GetInt32(reader.GetOrdinal("JobCardId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo")) ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId")) ? null : reader.GetInt32(reader.GetOrdinal("OrderId")),
                OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? null : reader.GetString(reader.GetOrdinal("OrderNo")),
                OrderItemId = reader.IsDBNull(reader.GetOrdinal("OrderItemId")) ? null : reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                ItemSequence = reader.IsDBNull(reader.GetOrdinal("ItemSequence")) ? null : reader.GetString(reader.GetOrdinal("ItemSequence")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                RequestedBy = reader.IsDBNull(reader.GetOrdinal("RequestedBy")) ? null : reader.GetString(reader.GetOrdinal("RequestedBy")),
                ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                ApprovalDate = reader.IsDBNull(reader.GetOrdinal("ApprovalDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovalDate")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }

        private static void AddRequisitionParameters(SqlCommand command, MaterialRequisition requisition)
        {
            command.Parameters.AddWithValue("@Id", requisition.Id);
            command.Parameters.AddWithValue("@RequisitionNo", requisition.RequisitionNo);
            command.Parameters.AddWithValue("@RequisitionDate", requisition.RequisitionDate);
            command.Parameters.AddWithValue("@JobCardId", (object?)requisition.JobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@JobCardNo", (object?)requisition.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderId", (object?)requisition.OrderId ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)requisition.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderItemId", (object?)requisition.OrderItemId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemSequence", (object?)requisition.ItemSequence ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerName", (object?)requisition.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", requisition.Status);
            command.Parameters.AddWithValue("@Priority", requisition.Priority);
            command.Parameters.AddWithValue("@DueDate", (object?)requisition.DueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@RequestedBy", (object?)requisition.RequestedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)requisition.ApprovedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovalDate", (object?)requisition.ApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)requisition.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", requisition.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)requisition.CreatedBy ?? DBNull.Value);
        }

        // Requisition Item Methods
        public async Task<int> InsertRequisitionItemAsync(MaterialRequisitionItem item)
        {
            const string query = @"
                INSERT INTO [Stores_MaterialRequisitionItems]
                ([RequisitionId], [LineNo], [MaterialId], [MaterialCode], [MaterialName], [MaterialGrade],
                 [ComponentId], [ComponentCode], [ComponentName],
                 [RequestedQuantity], [IssuedQuantity],
                 [QuantityRequired], [UOM], [LengthRequiredMM], [DiameterMM], [NumberOfPieces],
                 [QuantityAllocated], [QuantityIssued], [QuantityPending], [Status],
                 [JobCardId], [JobCardNo], [ProcessId], [ProcessName], [SelectedPieceIds], [SelectedPieceQuantities], [Remarks], [CreatedAt])
                VALUES
                (@RequisitionId, @LineNo, @MaterialId, @MaterialCode, @MaterialName, @MaterialGrade,
                 @ComponentId, @ComponentCode, @ComponentName,
                 @RequestedQuantity, @IssuedQuantity,
                 @QuantityRequired, @UOM, @LengthRequiredMM, @DiameterMM, @NumberOfPieces,
                 @QuantityAllocated, @QuantityIssued, @QuantityPending, @Status,
                 @JobCardId, @JobCardNo, @ProcessId, @ProcessName, @SelectedPieceIds, @SelectedPieceQuantities, @Remarks, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            item.CreatedAt = DateTime.UtcNow;
            item.Status = "Pending";
            item.QuantityPending = item.QuantityRequired;

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@RequisitionId", item.RequisitionId);
            command.Parameters.AddWithValue("@LineNo", item.LineNo);
            command.Parameters.AddWithValue("@MaterialId", (object?)item.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)item.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)item.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)item.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@ComponentId", (object?)item.ComponentId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ComponentCode", (object?)item.ComponentCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ComponentName", (object?)item.ComponentName ?? DBNull.Value);
            // Map to both old and new columns for backward compatibility
            command.Parameters.AddWithValue("@RequestedQuantity", item.QuantityRequired);
            command.Parameters.AddWithValue("@IssuedQuantity", (object?)item.QuantityIssued ?? 0);
            command.Parameters.AddWithValue("@QuantityRequired", item.QuantityRequired);
            command.Parameters.AddWithValue("@UOM", (object?)item.UOM ?? DBNull.Value);
            command.Parameters.AddWithValue("@LengthRequiredMM", (object?)item.LengthRequiredMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@DiameterMM", (object?)item.DiameterMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfPieces", (object?)item.NumberOfPieces ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityAllocated", (object?)item.QuantityAllocated ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityIssued", (object?)item.QuantityIssued ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityPending", (object?)item.QuantityPending ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", item.Status);
            command.Parameters.AddWithValue("@JobCardId", (object?)item.JobCardId ?? DBNull.Value);
            command.Parameters.AddWithValue("@JobCardNo", (object?)item.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessId", (object?)item.ProcessId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProcessName", (object?)item.ProcessName ?? DBNull.Value);
            command.Parameters.AddWithValue("@SelectedPieceIds", (object?)item.SelectedPieceIds ?? DBNull.Value);
            command.Parameters.AddWithValue("@SelectedPieceQuantities", (object?)item.SelectedPieceQuantities ?? DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)item.Remarks ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);

            await connection.OpenAsync();
            var itemId = (int)await command.ExecuteScalarAsync();

            return itemId;
        }

        public async Task<IEnumerable<MaterialRequisitionItem>> GetRequisitionItemsAsync(int requisitionId)
        {
            const string query = @"
                SELECT * FROM [Stores_MaterialRequisitionItems]
                WHERE [RequisitionId] = @RequisitionId
                ORDER BY [LineNo]";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var items = new List<MaterialRequisitionItem>();
            while (await reader.ReadAsync())
            {
                items.Add(MapToRequisitionItem(reader));
            }

            return items;
        }

        public async Task<bool> DeleteRequisitionItemsAsync(int requisitionId)
        {
            const string query = "DELETE FROM Stores_MaterialRequisitionItems WHERE RequisitionId = @RequisitionId";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", requisitionId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateItemSelectedPiecesAsync(int itemId, string? selectedPieceIds)
        {
            const string query = @"
                UPDATE Stores_MaterialRequisitionItems
                SET SelectedPieceIds = @SelectedPieceIds
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", itemId);
            command.Parameters.AddWithValue("@SelectedPieceIds", (object?)selectedPieceIds ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateItemPiecesAndQuantitiesAsync(int itemId, string? selectedPieceIds, string? selectedPieceQuantities)
        {
            const string query = @"
                UPDATE Stores_MaterialRequisitionItems
                SET SelectedPieceIds = @SelectedPieceIds,
                    SelectedPieceQuantities = @SelectedPieceQuantities
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", itemId);
            command.Parameters.AddWithValue("@SelectedPieceIds", (object?)selectedPieceIds ?? DBNull.Value);
            command.Parameters.AddWithValue("@SelectedPieceQuantities", (object?)selectedPieceQuantities ?? DBNull.Value);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<MaterialRequisitionItem?> GetItemByIdAsync(int itemId)
        {
            const string query = "SELECT * FROM [Stores_MaterialRequisitionItems] WHERE [Id] = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", itemId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapToRequisitionItem(reader);
            return null;
        }

        // Update a requisition line's material + spec (length/diameter/pieces/quantity).
        // Clears any pre-selected pieces because the old selection no longer matches the new material.
        public async Task<bool> UpdateItemMaterialAsync(MaterialRequisitionItem item)
        {
            const string query = @"
                UPDATE [Stores_MaterialRequisitionItems]
                SET [MaterialId]        = @MaterialId,
                    [MaterialCode]      = @MaterialCode,
                    [MaterialName]      = @MaterialName,
                    [MaterialGrade]     = @MaterialGrade,
                    [LengthRequiredMM]  = @LengthRequiredMM,
                    [DiameterMM]        = @DiameterMM,
                    [NumberOfPieces]    = @NumberOfPieces,
                    [QuantityRequired]  = @QuantityRequired,
                    [RequestedQuantity] = @QuantityRequired,
                    [QuantityPending]   = @QuantityRequired,
                    [SelectedPieceIds]  = NULL,
                    [SelectedPieceQuantities] = NULL
                WHERE [Id] = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@MaterialId", (object?)item.MaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialCode", (object?)item.MaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialName", (object?)item.MaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGrade", (object?)item.MaterialGrade ?? DBNull.Value);
            command.Parameters.AddWithValue("@LengthRequiredMM", (object?)item.LengthRequiredMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@DiameterMM", (object?)item.DiameterMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@NumberOfPieces", (object?)item.NumberOfPieces ?? DBNull.Value);
            command.Parameters.AddWithValue("@QuantityRequired", item.QuantityRequired);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<int> InsertMaterialChangeLogAsync(RequisitionMaterialChangeLog log)
        {
            const string query = @"
                INSERT INTO [Stores_RequisitionMaterialChangeLog]
                ([RequisitionId], [RequisitionNo], [ItemId], [LineNo], [JobCardNo], [OrderNo], [ChangeType],
                 [FromMaterialId], [FromMaterialCode], [FromMaterialName],
                 [ToMaterialId], [ToMaterialCode], [ToMaterialName],
                 [FromLengthMM], [ToLengthMM], [FromPieces], [ToPieces],
                 [ReApprovalTriggered], [Reason], [ChangedBy], [ChangedByRole], [CreatedAt])
                VALUES
                (@RequisitionId, @RequisitionNo, @ItemId, @LineNo, @JobCardNo, @OrderNo, @ChangeType,
                 @FromMaterialId, @FromMaterialCode, @FromMaterialName,
                 @ToMaterialId, @ToMaterialCode, @ToMaterialName,
                 @FromLengthMM, @ToLengthMM, @FromPieces, @ToPieces,
                 @ReApprovalTriggered, @Reason, @ChangedBy, @ChangedByRole, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RequisitionId", log.RequisitionId);
            command.Parameters.AddWithValue("@RequisitionNo", (object?)log.RequisitionNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemId", log.ItemId);
            command.Parameters.AddWithValue("@LineNo", (object?)log.LineNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@JobCardNo", (object?)log.JobCardNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderNo", (object?)log.OrderNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChangeType", log.ChangeType);
            command.Parameters.AddWithValue("@FromMaterialId", (object?)log.FromMaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromMaterialCode", (object?)log.FromMaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromMaterialName", (object?)log.FromMaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToMaterialId", (object?)log.ToMaterialId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToMaterialCode", (object?)log.ToMaterialCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToMaterialName", (object?)log.ToMaterialName ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromLengthMM", (object?)log.FromLengthMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToLengthMM", (object?)log.ToLengthMM ?? DBNull.Value);
            command.Parameters.AddWithValue("@FromPieces", (object?)log.FromPieces ?? DBNull.Value);
            command.Parameters.AddWithValue("@ToPieces", (object?)log.ToPieces ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReApprovalTriggered", log.ReApprovalTriggered);
            command.Parameters.AddWithValue("@Reason", log.Reason ?? string.Empty);
            command.Parameters.AddWithValue("@ChangedBy", (object?)log.ChangedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ChangedByRole", (object?)log.ChangedByRole ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var id = (int)await command.ExecuteScalarAsync();
            return id;
        }

        public async Task<IEnumerable<MaterialRequisitionItem>> GetItemsByRequisitionIdsAsync(IEnumerable<int> requisitionIds)
        {
            var idList = string.Join(",", requisitionIds);
            var query = $@"
                SELECT * FROM Stores_MaterialRequisitionItems
                WHERE RequisitionId IN ({idList})
                ORDER BY RequisitionId, [LineNo]";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            var items = new List<MaterialRequisitionItem>();
            while (await reader.ReadAsync())
                items.Add(MapToRequisitionItem(reader));
            return items;
        }

        public async Task<Dictionary<int, (string? ModelName, int? NumberOfTeeth, string? RollerType)>> GetProductInfoByRequisitionIdsAsync(IEnumerable<int> requisitionIds)
        {
            var result = new Dictionary<int, (string?, int?, string?)>();
            var idList = string.Join(",", requisitionIds);
            if (string.IsNullOrEmpty(idList)) return result;

            // Prefer order-item product; fall back to order-level product for single-product orders
            var query = $@"
                SELECT mr.Id AS ReqId,
                       COALESCE(pItem.ModelName, pOrder.ModelName) AS ModelName,
                       COALESCE(pItem.NumberOfTeeth, pOrder.NumberOfTeeth) AS NumberOfTeeth,
                       COALESCE(pItem.RollerType, pOrder.RollerType) AS RollerType
                FROM Stores_MaterialRequisitions mr
                LEFT JOIN Orders_OrderItems oi ON oi.Id = mr.OrderItemId
                LEFT JOIN Masters_Products pItem ON pItem.Id = oi.ProductId
                LEFT JOIN Orders o ON o.Id = mr.OrderId
                LEFT JOIN Masters_Products pOrder ON pOrder.Id = o.ProductId
                WHERE mr.Id IN ({idList})";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var reqId = reader.GetInt32(reader.GetOrdinal("ReqId"));
                var model = reader.IsDBNull(reader.GetOrdinal("ModelName")) ? null : reader.GetString(reader.GetOrdinal("ModelName"));
                var teeth = reader.IsDBNull(reader.GetOrdinal("NumberOfTeeth")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NumberOfTeeth"));
                var rollerType = reader.IsDBNull(reader.GetOrdinal("RollerType")) ? null : reader.GetString(reader.GetOrdinal("RollerType"));
                result[reqId] = (model, teeth, rollerType);
            }
            return result;
        }

        private static MaterialRequisitionItem MapToRequisitionItem(SqlDataReader reader)
        {
            return new MaterialRequisitionItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                RequisitionId = reader.GetInt32(reader.GetOrdinal("RequisitionId")),
                LineNo = reader.GetInt32(reader.GetOrdinal("LineNo")),
                MaterialId = reader.IsDBNull(reader.GetOrdinal("MaterialId")) ? null : reader.GetInt32(reader.GetOrdinal("MaterialId")),
                MaterialCode = reader.IsDBNull(reader.GetOrdinal("MaterialCode")) ? null : reader.GetString(reader.GetOrdinal("MaterialCode")),
                MaterialName = reader.IsDBNull(reader.GetOrdinal("MaterialName")) ? null : reader.GetString(reader.GetOrdinal("MaterialName")),
                MaterialGrade = reader.IsDBNull(reader.GetOrdinal("MaterialGrade")) ? null : reader.GetString(reader.GetOrdinal("MaterialGrade")),
                ComponentId = reader.IsDBNull(reader.GetOrdinal("ComponentId")) ? null : reader.GetInt32(reader.GetOrdinal("ComponentId")),
                ComponentCode = reader.IsDBNull(reader.GetOrdinal("ComponentCode")) ? null : reader.GetString(reader.GetOrdinal("ComponentCode")),
                ComponentName = reader.IsDBNull(reader.GetOrdinal("ComponentName")) ? null : reader.GetString(reader.GetOrdinal("ComponentName")),
                QuantityRequired = reader.GetDecimal(reader.GetOrdinal("QuantityRequired")),
                UOM = reader.IsDBNull(reader.GetOrdinal("UOM")) ? null : reader.GetString(reader.GetOrdinal("UOM")),
                LengthRequiredMM = reader.IsDBNull(reader.GetOrdinal("LengthRequiredMM")) ? null : reader.GetDecimal(reader.GetOrdinal("LengthRequiredMM")),
                DiameterMM = reader.IsDBNull(reader.GetOrdinal("DiameterMM")) ? null : reader.GetDecimal(reader.GetOrdinal("DiameterMM")),
                NumberOfPieces = reader.IsDBNull(reader.GetOrdinal("NumberOfPieces")) ? null : reader.GetInt32(reader.GetOrdinal("NumberOfPieces")),
                QuantityAllocated = reader.IsDBNull(reader.GetOrdinal("QuantityAllocated")) ? null : reader.GetDecimal(reader.GetOrdinal("QuantityAllocated")),
                QuantityIssued = reader.IsDBNull(reader.GetOrdinal("QuantityIssued")) ? null : reader.GetDecimal(reader.GetOrdinal("QuantityIssued")),
                QuantityPending = reader.IsDBNull(reader.GetOrdinal("QuantityPending")) ? null : reader.GetDecimal(reader.GetOrdinal("QuantityPending")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                AllocatedAt = reader.IsDBNull(reader.GetOrdinal("AllocatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("AllocatedAt")),
                IssuedAt = reader.IsDBNull(reader.GetOrdinal("IssuedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("IssuedAt")),
                JobCardId = reader.IsDBNull(reader.GetOrdinal("JobCardId")) ? null : reader.GetInt32(reader.GetOrdinal("JobCardId")),
                JobCardNo = reader.IsDBNull(reader.GetOrdinal("JobCardNo")) ? null : reader.GetString(reader.GetOrdinal("JobCardNo")),
                ProcessId = reader.IsDBNull(reader.GetOrdinal("ProcessId")) ? null : reader.GetInt32(reader.GetOrdinal("ProcessId")),
                ProcessName = reader.IsDBNull(reader.GetOrdinal("ProcessName")) ? null : reader.GetString(reader.GetOrdinal("ProcessName")),
                SelectedPieceIds = reader.IsDBNull(reader.GetOrdinal("SelectedPieceIds")) ? null : reader.GetString(reader.GetOrdinal("SelectedPieceIds")),
                SelectedPieceQuantities = reader.IsDBNull(reader.GetOrdinal("SelectedPieceQuantities")) ? null : reader.GetString(reader.GetOrdinal("SelectedPieceQuantities")),
                Remarks = reader.IsDBNull(reader.GetOrdinal("Remarks")) ? null : reader.GetString(reader.GetOrdinal("Remarks")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
