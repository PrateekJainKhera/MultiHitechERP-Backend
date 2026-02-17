using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Procurement;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PurchaseOrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            const string query = @"
                SELECT po.Id, po.PONumber, po.PurchaseRequestId, po.VendorId, po.Status,
                       po.TotalAmount, po.ExpectedDeliveryDate, po.Notes, po.CreatedAt, po.CreatedBy,
                       v.VendorName, v.VendorCode,
                       pr.PRNumber,
                       poi.Id AS PoiId, poi.PurchaseRequestItemId, poi.ItemType AS PoiItemType,
                       poi.ItemId AS PoiItemId, poi.ItemName AS PoiItemName, poi.ItemCode AS PoiItemCode,
                       poi.Unit AS PoiUnit, poi.OrderedQty, poi.UnitCost, poi.TotalCost
                FROM Procurement_PurchaseOrders po
                JOIN Masters_Vendors v ON v.Id = po.VendorId
                LEFT JOIN Procurement_PurchaseRequests pr ON pr.Id = po.PurchaseRequestId
                LEFT JOIN Procurement_PurchaseOrderItems poi ON poi.PurchaseOrderId = po.Id
                ORDER BY po.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, null);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByVendorAsync(int vendorId)
        {
            const string query = @"
                SELECT po.Id, po.PONumber, po.PurchaseRequestId, po.VendorId, po.Status,
                       po.TotalAmount, po.ExpectedDeliveryDate, po.Notes, po.CreatedAt, po.CreatedBy,
                       v.VendorName, v.VendorCode,
                       pr.PRNumber,
                       poi.Id AS PoiId, poi.PurchaseRequestItemId, poi.ItemType AS PoiItemType,
                       poi.ItemId AS PoiItemId, poi.ItemName AS PoiItemName, poi.ItemCode AS PoiItemCode,
                       poi.Unit AS PoiUnit, poi.OrderedQty, poi.UnitCost, poi.TotalCost
                FROM Procurement_PurchaseOrders po
                JOIN Masters_Vendors v ON v.Id = po.VendorId
                LEFT JOIN Procurement_PurchaseRequests pr ON pr.Id = po.PurchaseRequestId
                LEFT JOIN Procurement_PurchaseOrderItems poi ON poi.PurchaseOrderId = po.Id
                WHERE po.VendorId = @VendorId
                ORDER BY po.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, cmd => cmd.Parameters.AddWithValue("@VendorId", vendorId));
        }

        public async Task<IEnumerable<PurchaseOrder>> GetByPurchaseRequestAsync(int prId)
        {
            const string query = @"
                SELECT po.Id, po.PONumber, po.PurchaseRequestId, po.VendorId, po.Status,
                       po.TotalAmount, po.ExpectedDeliveryDate, po.Notes, po.CreatedAt, po.CreatedBy,
                       v.VendorName, v.VendorCode,
                       pr.PRNumber,
                       poi.Id AS PoiId, poi.PurchaseRequestItemId, poi.ItemType AS PoiItemType,
                       poi.ItemId AS PoiItemId, poi.ItemName AS PoiItemName, poi.ItemCode AS PoiItemCode,
                       poi.Unit AS PoiUnit, poi.OrderedQty, poi.UnitCost, poi.TotalCost
                FROM Procurement_PurchaseOrders po
                JOIN Masters_Vendors v ON v.Id = po.VendorId
                LEFT JOIN Procurement_PurchaseRequests pr ON pr.Id = po.PurchaseRequestId
                LEFT JOIN Procurement_PurchaseOrderItems poi ON poi.PurchaseOrderId = po.Id
                WHERE po.PurchaseRequestId = @PRId
                ORDER BY po.CreatedAt DESC";

            return await ExecuteQueryWithItems(query, cmd => cmd.Parameters.AddWithValue("@PRId", prId));
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            const string query = @"
                SELECT po.Id, po.PONumber, po.PurchaseRequestId, po.VendorId, po.Status,
                       po.TotalAmount, po.ExpectedDeliveryDate, po.Notes, po.CreatedAt, po.CreatedBy,
                       v.VendorName, v.VendorCode,
                       pr.PRNumber,
                       poi.Id AS PoiId, poi.PurchaseRequestItemId, poi.ItemType AS PoiItemType,
                       poi.ItemId AS PoiItemId, poi.ItemName AS PoiItemName, poi.ItemCode AS PoiItemCode,
                       poi.Unit AS PoiUnit, poi.OrderedQty, poi.UnitCost, poi.TotalCost
                FROM Procurement_PurchaseOrders po
                JOIN Masters_Vendors v ON v.Id = po.VendorId
                LEFT JOIN Procurement_PurchaseRequests pr ON pr.Id = po.PurchaseRequestId
                LEFT JOIN Procurement_PurchaseOrderItems poi ON poi.PurchaseOrderId = po.Id
                WHERE po.Id = @Id";

            var results = await ExecuteQueryWithItems(query, cmd => cmd.Parameters.AddWithValue("@Id", id));
            return results.FirstOrDefault();
        }

        public async Task<int> InsertAsync(PurchaseOrder po)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                const string poQuery = @"
                    INSERT INTO Procurement_PurchaseOrders
                        (PONumber, PurchaseRequestId, VendorId, Status, TotalAmount,
                         ExpectedDeliveryDate, Notes, CreatedAt, CreatedBy)
                    VALUES (@PONumber, @PurchaseRequestId, @VendorId, @Status, @TotalAmount,
                            @ExpectedDeliveryDate, @Notes, @CreatedAt, @CreatedBy);
                    SELECT SCOPE_IDENTITY();";

                int poId;
                using (var cmd = new SqlCommand(poQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@PONumber", po.PONumber);
                    cmd.Parameters.AddWithValue("@PurchaseRequestId", (object?)po.PurchaseRequestId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@VendorId", po.VendorId);
                    cmd.Parameters.AddWithValue("@Status", po.Status);
                    cmd.Parameters.AddWithValue("@TotalAmount", (object?)po.TotalAmount ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpectedDeliveryDate", (object?)po.ExpectedDeliveryDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notes", (object?)po.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", po.CreatedAt);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object?)po.CreatedBy ?? DBNull.Value);
                    poId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }

                foreach (var item in po.Items)
                {
                    const string itemQuery = @"
                        INSERT INTO Procurement_PurchaseOrderItems
                            (PurchaseOrderId, PurchaseRequestItemId, ItemType, ItemId,
                             ItemName, ItemCode, Unit, OrderedQty, UnitCost, TotalCost)
                        VALUES (@PurchaseOrderId, @PurchaseRequestItemId, @ItemType, @ItemId,
                                @ItemName, @ItemCode, @Unit, @OrderedQty, @UnitCost, @TotalCost)";

                    using var itemCmd = new SqlCommand(itemQuery, connection, transaction);
                    itemCmd.Parameters.AddWithValue("@PurchaseOrderId", poId);
                    itemCmd.Parameters.AddWithValue("@PurchaseRequestItemId", (object?)item.PurchaseRequestItemId ?? DBNull.Value);
                    itemCmd.Parameters.AddWithValue("@ItemType", item.ItemType);
                    itemCmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                    itemCmd.Parameters.AddWithValue("@ItemName", item.ItemName);
                    itemCmd.Parameters.AddWithValue("@ItemCode", (object?)item.ItemCode ?? DBNull.Value);
                    itemCmd.Parameters.AddWithValue("@Unit", item.Unit);
                    itemCmd.Parameters.AddWithValue("@OrderedQty", item.OrderedQty);
                    itemCmd.Parameters.AddWithValue("@UnitCost", (object?)item.UnitCost ?? DBNull.Value);
                    itemCmd.Parameters.AddWithValue("@TotalCost", (object?)item.TotalCost ?? DBNull.Value);
                    await itemCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return poId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = "UPDATE Procurement_PurchaseOrders SET Status = @Status WHERE Id = @Id";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> GetNextSequenceNumberAsync()
        {
            const string query = "SELECT COUNT(1) FROM Procurement_PurchaseOrders";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            return Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
        }

        private async Task<List<PurchaseOrder>> ExecuteQueryWithItems(string query, Action<SqlCommand>? paramAction)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            paramAction?.Invoke(command);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var poDict = new Dictionary<int, (PurchaseOrder po, string vendorName, string vendorCode, string? prNumber)>();
            while (await reader.ReadAsync())
            {
                var poId = reader.GetInt32(reader.GetOrdinal("Id"));
                if (!poDict.TryGetValue(poId, out var entry))
                {
                    var po = new PurchaseOrder
                    {
                        Id = poId,
                        PONumber = reader.GetString(reader.GetOrdinal("PONumber")),
                        PurchaseRequestId = reader.IsDBNull(reader.GetOrdinal("PurchaseRequestId")) ? null : reader.GetInt32(reader.GetOrdinal("PurchaseRequestId")),
                        VendorId = reader.GetInt32(reader.GetOrdinal("VendorId")),
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                        ExpectedDeliveryDate = reader.IsDBNull(reader.GetOrdinal("ExpectedDeliveryDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ExpectedDeliveryDate")),
                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                    };
                    var vendorName = reader.GetString(reader.GetOrdinal("VendorName"));
                    var vendorCode = reader.GetString(reader.GetOrdinal("VendorCode"));
                    var prNumber = reader.IsDBNull(reader.GetOrdinal("PRNumber")) ? null : reader.GetString(reader.GetOrdinal("PRNumber"));
                    entry = (po, vendorName, vendorCode, prNumber);
                    poDict[poId] = entry;
                }

                if (!reader.IsDBNull(reader.GetOrdinal("PoiId")))
                {
                    entry.po.Items.Add(new PurchaseOrderItem
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("PoiId")),
                        PurchaseOrderId = poId,
                        PurchaseRequestItemId = reader.IsDBNull(reader.GetOrdinal("PurchaseRequestItemId")) ? null : reader.GetInt32(reader.GetOrdinal("PurchaseRequestItemId")),
                        ItemType = reader.GetString(reader.GetOrdinal("PoiItemType")),
                        ItemId = reader.GetInt32(reader.GetOrdinal("PoiItemId")),
                        ItemName = reader.GetString(reader.GetOrdinal("PoiItemName")),
                        ItemCode = reader.IsDBNull(reader.GetOrdinal("PoiItemCode")) ? null : reader.GetString(reader.GetOrdinal("PoiItemCode")),
                        Unit = reader.GetString(reader.GetOrdinal("PoiUnit")),
                        OrderedQty = reader.GetDecimal(reader.GetOrdinal("OrderedQty")),
                        UnitCost = reader.IsDBNull(reader.GetOrdinal("UnitCost")) ? null : reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                        TotalCost = reader.IsDBNull(reader.GetOrdinal("TotalCost")) ? null : reader.GetDecimal(reader.GetOrdinal("TotalCost")),
                    });
                }
            }

            // Attach vendor info as a temporary storage pattern - store in extended model
            return poDict.Values.Select(e => {
                e.po.Notes = e.po.Notes; // keep notes
                return e.po;
            }).ToList();
        }

        // Helper to get vendor info for mapping - stored in separate query result
        public async Task<Dictionary<int, (string vendorName, string vendorCode, string? prNumber)>> GetPoMetadataAsync(IEnumerable<int> poIds)
        {
            var idList = string.Join(",", poIds);
            if (string.IsNullOrEmpty(idList)) return new();
            var query = $@"
                SELECT po.Id, v.VendorName, v.VendorCode, pr.PRNumber
                FROM Procurement_PurchaseOrders po
                JOIN Masters_Vendors v ON v.Id = po.VendorId
                LEFT JOIN Procurement_PurchaseRequests pr ON pr.Id = po.PurchaseRequestId
                WHERE po.Id IN ({idList})";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var result = new Dictionary<int, (string, string, string?)>();
            while (await reader.ReadAsync())
            {
                result[reader.GetInt32(0)] = (
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3));
            }
            return result;
        }
    }
}
