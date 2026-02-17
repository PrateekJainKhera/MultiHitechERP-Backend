using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Procurement;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PurchaseRequestRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PurchaseRequest>> GetAllAsync()
        {
            const string query = @"
                SELECT pr.Id, pr.PRNumber, pr.ItemType, pr.RequestedBy, pr.RequestDate,
                       pr.Notes, pr.Status, pr.RejectionReason, pr.ApprovedBy, pr.ApprovedAt, pr.CreatedAt,
                       pri.Id AS ItemId2, pri.PurchaseRequestId, pri.ItemType AS ItemItemType,
                       pri.ItemId, pri.ItemName, pri.ItemCode, pri.Unit,
                       pri.RequestedQty, pri.ApprovedQty, pri.VendorId,
                       pri.EstimatedUnitCost, pri.Status AS ItemStatus, pri.Notes AS ItemNotes,
                       v.VendorName
                FROM Procurement_PurchaseRequests pr
                LEFT JOIN Procurement_PurchaseRequestItems pri ON pri.PurchaseRequestId = pr.Id
                LEFT JOIN Masters_Vendors v ON v.Id = pri.VendorId
                ORDER BY pr.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, null);
        }

        public async Task<IEnumerable<PurchaseRequest>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT pr.Id, pr.PRNumber, pr.ItemType, pr.RequestedBy, pr.RequestDate,
                       pr.Notes, pr.Status, pr.RejectionReason, pr.ApprovedBy, pr.ApprovedAt, pr.CreatedAt,
                       pri.Id AS ItemId2, pri.PurchaseRequestId, pri.ItemType AS ItemItemType,
                       pri.ItemId, pri.ItemName, pri.ItemCode, pri.Unit,
                       pri.RequestedQty, pri.ApprovedQty, pri.VendorId,
                       pri.EstimatedUnitCost, pri.Status AS ItemStatus, pri.Notes AS ItemNotes,
                       v.VendorName
                FROM Procurement_PurchaseRequests pr
                LEFT JOIN Procurement_PurchaseRequestItems pri ON pri.PurchaseRequestId = pr.Id
                LEFT JOIN Masters_Vendors v ON v.Id = pri.VendorId
                WHERE pr.Status = @Status
                ORDER BY pr.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, cmd => cmd.Parameters.AddWithValue("@Status", status));
        }

        public async Task<IEnumerable<PurchaseRequest>> GetByItemTypeAsync(string itemType)
        {
            const string query = @"
                SELECT pr.Id, pr.PRNumber, pr.ItemType, pr.RequestedBy, pr.RequestDate,
                       pr.Notes, pr.Status, pr.RejectionReason, pr.ApprovedBy, pr.ApprovedAt, pr.CreatedAt,
                       pri.Id AS ItemId2, pri.PurchaseRequestId, pri.ItemType AS ItemItemType,
                       pri.ItemId, pri.ItemName, pri.ItemCode, pri.Unit,
                       pri.RequestedQty, pri.ApprovedQty, pri.VendorId,
                       pri.EstimatedUnitCost, pri.Status AS ItemStatus, pri.Notes AS ItemNotes,
                       v.VendorName
                FROM Procurement_PurchaseRequests pr
                LEFT JOIN Procurement_PurchaseRequestItems pri ON pri.PurchaseRequestId = pr.Id
                LEFT JOIN Masters_Vendors v ON v.Id = pri.VendorId
                WHERE pr.ItemType = @ItemType
                ORDER BY pr.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, cmd => cmd.Parameters.AddWithValue("@ItemType", itemType));
        }

        public async Task<PurchaseRequest?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT pr.Id, pr.PRNumber, pr.ItemType, pr.RequestedBy, pr.RequestDate,
                       pr.Notes, pr.Status, pr.RejectionReason, pr.ApprovedBy, pr.ApprovedAt, pr.CreatedAt,
                       pri.Id AS ItemId2, pri.PurchaseRequestId, pri.ItemType AS ItemItemType,
                       pri.ItemId, pri.ItemName, pri.ItemCode, pri.Unit,
                       pri.RequestedQty, pri.ApprovedQty, pri.VendorId,
                       pri.EstimatedUnitCost, pri.Status AS ItemStatus, pri.Notes AS ItemNotes,
                       v.VendorName,
                       cl.Id AS ClId, cl.PRItemId AS ClPRItemId, cl.LengthMeter, cl.Pieces, cl.Notes AS ClNotes
                FROM Procurement_PurchaseRequests pr
                LEFT JOIN Procurement_PurchaseRequestItems pri ON pri.PurchaseRequestId = pr.Id
                LEFT JOIN Masters_Vendors v ON v.Id = pri.VendorId
                LEFT JOIN Procurement_PRItemCuttingList cl ON cl.PRItemId = pri.Id
                WHERE pr.Id = @Id
                ORDER BY pri.Id, cl.Id";

            var results = await ExecuteQueryWithItemsAndCutting(query, cmd => cmd.Parameters.AddWithValue("@Id", id));
            return results.FirstOrDefault();
        }

        public async Task<int> InsertAsync(PurchaseRequest pr)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string prQuery = @"
                    INSERT INTO Procurement_PurchaseRequests
                        (PRNumber, ItemType, RequestedBy, RequestDate, Notes, Status, CreatedAt)
                    VALUES (@PRNumber, @ItemType, @RequestedBy, @RequestDate, @Notes, @Status, @CreatedAt);
                    SELECT SCOPE_IDENTITY();";

                int prId;
                using (var cmd = new SqlCommand(prQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@PRNumber", pr.PRNumber);
                    cmd.Parameters.AddWithValue("@ItemType", pr.ItemType);
                    cmd.Parameters.AddWithValue("@RequestedBy", pr.RequestedBy);
                    cmd.Parameters.AddWithValue("@RequestDate", pr.RequestDate);
                    cmd.Parameters.AddWithValue("@Notes", (object?)pr.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", pr.Status);
                    cmd.Parameters.AddWithValue("@CreatedAt", pr.CreatedAt);
                    prId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                foreach (var item in pr.Items)
                {
                    item.PurchaseRequestId = prId;
                    await InsertItemWithConnection(item, connection, transaction);
                }

                transaction.Commit();
                return prId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, string status, string? approvedBy = null, string? rejectionReason = null)
        {
            const string query = @"
                UPDATE Procurement_PurchaseRequests SET
                    Status = @Status,
                    ApprovedBy = @ApprovedBy,
                    ApprovedAt = @ApprovedAt,
                    RejectionReason = @RejectionReason
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@ApprovedBy", (object?)approvedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ApprovedAt", status == "Approved" ? (object)DateTime.UtcNow : DBNull.Value);
            command.Parameters.AddWithValue("@RejectionReason", (object?)rejectionReason ?? DBNull.Value);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateItemAsync(PurchaseRequestItem item)
        {
            const string query = @"
                UPDATE Procurement_PurchaseRequestItems SET
                    ApprovedQty = @ApprovedQty,
                    VendorId = @VendorId,
                    EstimatedUnitCost = @EstimatedUnitCost,
                    Status = @Status,
                    Notes = @Notes
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@ApprovedQty", (object?)item.ApprovedQty ?? DBNull.Value);
            command.Parameters.AddWithValue("@VendorId", (object?)item.VendorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstimatedUnitCost", (object?)item.EstimatedUnitCost ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", item.Status);
            command.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> InsertItemAsync(PurchaseRequestItem item)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            return await InsertItemWithConnection(item, connection, null);
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            const string query = "DELETE FROM Procurement_PurchaseRequestItems WHERE Id = @Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", itemId);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync()
        {
            const string query = "SELECT COUNT(1) FROM Procurement_PurchaseRequests";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
        }

        private async Task<int> InsertItemWithConnection(PurchaseRequestItem item, SqlConnection connection, SqlTransaction? transaction)
        {
            const string query = @"
                INSERT INTO Procurement_PurchaseRequestItems
                    (PurchaseRequestId, ItemType, ItemId, ItemName, ItemCode, Unit, RequestedQty, Status, Notes)
                VALUES (@PurchaseRequestId, @ItemType, @ItemId, @ItemName, @ItemCode, @Unit, @RequestedQty, @Status, @Notes);
                SELECT SCOPE_IDENTITY();";

            using var cmd = transaction != null
                ? new SqlCommand(query, connection, transaction)
                : new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@PurchaseRequestId", item.PurchaseRequestId);
            cmd.Parameters.AddWithValue("@ItemType", item.ItemType);
            cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@ItemCode", (object?)item.ItemCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", item.Unit);
            cmd.Parameters.AddWithValue("@RequestedQty", item.RequestedQty);
            cmd.Parameters.AddWithValue("@Status", item.Status);
            cmd.Parameters.AddWithValue("@Notes", (object?)item.Notes ?? DBNull.Value);
            var itemId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            // Insert cutting list entries if present
            foreach (var cl in item.CuttingList)
            {
                const string clQuery = @"
                    INSERT INTO Procurement_PRItemCuttingList (PRItemId, LengthMeter, Pieces, Notes)
                    VALUES (@PRItemId, @LengthMeter, @Pieces, @Notes)";

                using var clCmd = transaction != null
                    ? new SqlCommand(clQuery, connection, transaction)
                    : new SqlCommand(clQuery, connection);

                clCmd.Parameters.AddWithValue("@PRItemId", itemId);
                clCmd.Parameters.AddWithValue("@LengthMeter", cl.LengthMeter);
                clCmd.Parameters.AddWithValue("@Pieces", cl.Pieces);
                clCmd.Parameters.AddWithValue("@Notes", (object?)cl.Notes ?? DBNull.Value);
                await clCmd.ExecuteNonQueryAsync();
            }

            return itemId;
        }

        private async Task<List<PurchaseRequest>> ExecuteQueryWithItems(string query, Action<SqlCommand>? paramAction)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            paramAction?.Invoke(command);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var prDict = new Dictionary<int, PurchaseRequest>();
            while (await reader.ReadAsync())
            {
                var prId = reader.GetInt32(reader.GetOrdinal("Id"));
                if (!prDict.TryGetValue(prId, out var pr))
                {
                    pr = new PurchaseRequest
                    {
                        Id = prId,
                        PRNumber = reader.GetString(reader.GetOrdinal("PRNumber")),
                        ItemType = reader.GetString(reader.GetOrdinal("ItemType")),
                        RequestedBy = reader.GetString(reader.GetOrdinal("RequestedBy")),
                        RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                        ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                        ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    };
                    prDict[prId] = pr;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("ItemId2")))
                {
                    pr.Items.Add(new PurchaseRequestItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ItemId2")),
                        PurchaseRequestId = prId,
                        ItemType = reader.GetString(reader.GetOrdinal("ItemItemType")),
                        ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                        ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                        ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode")) ? null : reader.GetString(reader.GetOrdinal("ItemCode")),
                        Unit = reader.GetString(reader.GetOrdinal("Unit")),
                        RequestedQty = reader.GetDecimal(reader.GetOrdinal("RequestedQty")),
                        ApprovedQty = reader.IsDBNull(reader.GetOrdinal("ApprovedQty")) ? null : reader.GetDecimal(reader.GetOrdinal("ApprovedQty")),
                        VendorId = reader.IsDBNull(reader.GetOrdinal("VendorId")) ? null : reader.GetInt32(reader.GetOrdinal("VendorId")),
                        EstimatedUnitCost = reader.IsDBNull(reader.GetOrdinal("EstimatedUnitCost")) ? null : reader.GetDecimal(reader.GetOrdinal("EstimatedUnitCost")),
                        Status = reader.GetString(reader.GetOrdinal("ItemStatus")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("ItemNotes")) ? null : reader.GetString(reader.GetOrdinal("ItemNotes")),
                    });
                }
            }
            return prDict.Values.ToList();
        }

        // Used by GetByIdAsync — loads items + cutting list in one query
        private async Task<List<PurchaseRequest>> ExecuteQueryWithItemsAndCutting(string query, Action<SqlCommand>? paramAction)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            paramAction?.Invoke(command);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var prDict = new Dictionary<int, PurchaseRequest>();
            var itemDict = new Dictionary<int, PurchaseRequestItem>();

            while (await reader.ReadAsync())
            {
                var prId = reader.GetInt32(reader.GetOrdinal("Id"));
                if (!prDict.TryGetValue(prId, out var pr))
                {
                    pr = new PurchaseRequest
                    {
                        Id = prId,
                        PRNumber = reader.GetString(reader.GetOrdinal("PRNumber")),
                        ItemType = reader.GetString(reader.GetOrdinal("ItemType")),
                        RequestedBy = reader.GetString(reader.GetOrdinal("RequestedBy")),
                        RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        RejectionReason = reader.IsDBNull(reader.GetOrdinal("RejectionReason")) ? null : reader.GetString(reader.GetOrdinal("RejectionReason")),
                        ApprovedBy = reader.IsDBNull(reader.GetOrdinal("ApprovedBy")) ? null : reader.GetString(reader.GetOrdinal("ApprovedBy")),
                        ApprovedAt = reader.IsDBNull(reader.GetOrdinal("ApprovedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ApprovedAt")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    };
                    prDict[prId] = pr;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("ItemId2")))
                {
                    var itemId = reader.GetInt32(reader.GetOrdinal("ItemId2"));
                    if (!itemDict.TryGetValue(itemId, out var prItem))
                    {
                        prItem = new PurchaseRequestItem
                        {
                            Id = itemId,
                            PurchaseRequestId = prId,
                            ItemType = reader.GetString(reader.GetOrdinal("ItemItemType")),
                            ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                            ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                            ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode")) ? null : reader.GetString(reader.GetOrdinal("ItemCode")),
                            Unit = reader.GetString(reader.GetOrdinal("Unit")),
                            RequestedQty = reader.GetDecimal(reader.GetOrdinal("RequestedQty")),
                            ApprovedQty = reader.IsDBNull(reader.GetOrdinal("ApprovedQty")) ? null : reader.GetDecimal(reader.GetOrdinal("ApprovedQty")),
                            VendorId = reader.IsDBNull(reader.GetOrdinal("VendorId")) ? null : reader.GetInt32(reader.GetOrdinal("VendorId")),
                            EstimatedUnitCost = reader.IsDBNull(reader.GetOrdinal("EstimatedUnitCost")) ? null : reader.GetDecimal(reader.GetOrdinal("EstimatedUnitCost")),
                            Status = reader.GetString(reader.GetOrdinal("ItemStatus")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("ItemNotes")) ? null : reader.GetString(reader.GetOrdinal("ItemNotes")),
                        };
                        itemDict[itemId] = prItem;
                        pr.Items.Add(prItem);
                    }

                    // Add cutting list entry if present
                    if (!reader.IsDBNull(reader.GetOrdinal("ClId")))
                    {
                        prItem.CuttingList.Add(new PRItemCuttingListEntry
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ClId")),
                            PRItemId = reader.GetInt32(reader.GetOrdinal("ClPRItemId")),
                            LengthMeter = reader.GetDecimal(reader.GetOrdinal("LengthMeter")),
                            Pieces = reader.GetInt32(reader.GetOrdinal("Pieces")),
                            Notes = reader.IsDBNull(reader.GetOrdinal("ClNotes")) ? null : reader.GetString(reader.GetOrdinal("ClNotes")),
                        });
                    }
                }
            }
            return prDict.Values.ToList();
        }
    }
}
