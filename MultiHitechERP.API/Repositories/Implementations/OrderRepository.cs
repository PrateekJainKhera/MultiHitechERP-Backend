using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.Models.Orders;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// ADO.NET implementation of Order repository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrderRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // SQL fragment shared across read queries — resolves CustomerName and ProductName via JOIN
        private const string SelectOrderWithNames = @"
            SELECT
                o.Id, o.OrderNo, o.OrderDate, o.DueDate, o.AdjustedDueDate,
                o.CustomerId,
                ISNULL(o.CustomerName, c.CustomerName) AS CustomerName,
                o.ProductId,
                ISNULL(o.ProductName, mp.ModelName) AS ProductName,
                o.Quantity, o.OriginalQuantity,
                o.QtyCompleted, o.QtyRejected, o.QtyInProgress, o.QtyScrap,
                o.Status, o.Priority, o.PlanningStatus,
                o.OrderSource, o.AgentCustomerId, o.AgentCommission, o.SchedulingStrategy,
                o.DrawingReviewStatus, o.DrawingReviewedBy, o.DrawingReviewedAt, o.DrawingReviewNotes,
                o.PrimaryDrawingId, o.DrawingSource, o.LinkedProductTemplateId,
                o.CustomerMachine, o.MaterialGradeRemark,
                o.CurrentProcess, o.CurrentMachine, o.CurrentOperator,
                o.ProductionStartDate, o.ProductionEndDate,
                o.DelayReason, o.RescheduleCount,
                o.MaterialGradeApproved, o.MaterialGradeApprovalDate, o.MaterialGradeApprovedBy,
                o.OrderValue, o.AdvancePayment, o.BalancePayment,
                o.CreatedAt, o.CreatedBy, o.UpdatedAt, o.UpdatedBy, o.Version
            FROM Orders o
            LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
            LEFT JOIN Masters_Products mp ON o.ProductId = mp.Id";

        public async Task<Order?> GetByIdAsync(int id)
        {
            var query = $"{SelectOrderWithNames} WHERE o.Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToOrder(reader);
            }

            return null;
        }

        public async Task<Order?> GetByOrderNoAsync(string orderNo)
        {
            var query = $"{SelectOrderWithNames} WHERE o.OrderNo = @OrderNo";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@OrderNo", orderNo);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToOrder(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var query = $"{SelectOrderWithNames} ORDER BY o.CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        // Effective-status WHERE fragments (order-level approximation of the UI's effective status)
        private static string EffectiveStatusFilter(string? status) => (status ?? "all").ToLowerInvariant() switch
        {
            "completed"  => "o.Status = 'Completed'",
            "ready"      => "o.Status <> 'Completed' AND o.Quantity > 0 AND o.QtyCompleted >= o.Quantity",
            "inprogress" => "o.Status <> 'Completed' AND NOT (o.Quantity > 0 AND o.QtyCompleted >= o.Quantity) AND (ISNULL(o.QtyInProgress,0) > 0 OR ISNULL(o.QtyCompleted,0) > 0)",
            "pending"    => "o.Status <> 'Completed' AND ISNULL(o.QtyInProgress,0) = 0 AND ISNULL(o.QtyCompleted,0) = 0",
            _            => ""
        };

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? search, string? status, DTOs.Request.OrderListFilter? filter = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;
            filter ??= new DTOs.Request.OrderListFilter();

            // Concatenated product text (order-level, for legacy single-product orders).
            const string orderProductText =
                "(ISNULL(o.ProductName,'')+' '+ISNULL(mp.ModelName,'')+' '+ISNULL(mp.RollerType,'')+' '+ISNULL(CONVERT(varchar(10),mp.NumberOfTeeth),'')+'T')";
            // Product text across the order's items (multi-product orders). Teeth rendered as "110T".
            const string itemProductMatch = @"EXISTS (
                SELECT 1 FROM Orders_OrderItems oi
                JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE oi.OrderId = o.Id AND
                (ISNULL(p.PartCode,'')+' '+ISNULL(p.ModelName,'')+' '+ISNULL(p.RollerType,'')+' '+ISNULL(CONVERT(varchar(10),p.NumberOfTeeth),'')+'T') LIKE @Product)";

            var conditions = new List<string>();
            var statusFilter = EffectiveStatusFilter(status);
            if (!string.IsNullOrEmpty(statusFilter)) conditions.Add(statusFilter);
            if (!string.IsNullOrWhiteSpace(search))
                conditions.Add("(o.OrderNo LIKE @Search OR ISNULL(o.CustomerName, c.CustomerName) LIKE @Search OR ISNULL(o.ProductName, mp.ModelName) LIKE @Search)");
            // Per-column filters (AND-combined)
            if (!string.IsNullOrWhiteSpace(filter.OrderNo))  conditions.Add("o.OrderNo LIKE @OrderNo");
            if (!string.IsNullOrWhiteSpace(filter.Customer)) conditions.Add("ISNULL(o.CustomerName, c.CustomerName) LIKE @Customer");
            if (!string.IsNullOrWhiteSpace(filter.Product))  conditions.Add($"({itemProductMatch} OR {orderProductText} LIKE @Product)");
            if (!string.IsNullOrWhiteSpace(filter.Source))   conditions.Add("o.OrderSource = @Source");
            if (filter.OrderDateFrom.HasValue)               conditions.Add("o.OrderDate >= @OrderDateFrom");
            if (filter.OrderDateTo.HasValue)                 conditions.Add("o.OrderDate < DATEADD(day, 1, @OrderDateTo)"); // inclusive of the To day
            var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            var listQuery = $@"{SelectOrderWithNames} {whereClause}
                ORDER BY o.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var countQuery = $@"
                SELECT COUNT(*)
                FROM Orders o
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
                LEFT JOIN Masters_Products mp ON o.ProductId = mp.Id
                {whereClause}";

            // Adds every filter parameter that applies, shared by both the count and list commands.
            void AddFilterParams(SqlCommand cmd)
            {
                if (!string.IsNullOrWhiteSpace(search))          cmd.Parameters.AddWithValue("@Search", $"%{search}%");
                if (!string.IsNullOrWhiteSpace(filter.OrderNo))  cmd.Parameters.AddWithValue("@OrderNo", $"%{filter.OrderNo}%");
                if (!string.IsNullOrWhiteSpace(filter.Customer)) cmd.Parameters.AddWithValue("@Customer", $"%{filter.Customer}%");
                if (!string.IsNullOrWhiteSpace(filter.Product))  cmd.Parameters.AddWithValue("@Product", $"%{filter.Product}%");
                if (!string.IsNullOrWhiteSpace(filter.Source))   cmd.Parameters.AddWithValue("@Source", filter.Source);
                if (filter.OrderDateFrom.HasValue)               cmd.Parameters.AddWithValue("@OrderDateFrom", filter.OrderDateFrom.Value.Date);
                if (filter.OrderDateTo.HasValue)                 cmd.Parameters.AddWithValue("@OrderDateTo", filter.OrderDateTo.Value.Date);
            }

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            int total;
            using (var countCmd = new SqlCommand(countQuery, connection))
            {
                AddFilterParams(countCmd);
                total = (int)await countCmd.ExecuteScalarAsync();
            }

            var orders = new List<Order>();
            using (var command = new SqlCommand(listQuery, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                AddFilterParams(command);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync()) orders.Add(MapToOrder(reader));
            }

            return (orders, total);
        }

        public async Task<IEnumerable<DTOs.Response.OrderLiteResponse>> GetLiteListAsync()
        {
            // One row per order item (sub-order) with product spec — single fast query.
            const string sql = @"
                SELECT o.Id AS OrderId, oi.Id AS OrderItemId, oi.ItemSequence, o.OrderNo,
                       ISNULL(o.CustomerName, c.CustomerName) AS CustomerName,
                       p.ModelName AS MachineModel, p.RollerType, p.NumberOfTeeth,
                       o.OrderDate
                FROM Orders o
                JOIN Orders_OrderItems oi ON oi.OrderId = o.Id
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
                LEFT JOIN Masters_Products p ON oi.ProductId = p.Id
                ORDER BY o.OrderDate DESC, o.OrderNo DESC, oi.ItemSequence";

            var list = new List<DTOs.Response.OrderLiteResponse>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DTOs.Response.OrderLiteResponse
                {
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    OrderItemId = reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                    ItemSequence = reader.IsDBNull(reader.GetOrdinal("ItemSequence")) ? null : reader.GetString(reader.GetOrdinal("ItemSequence")),
                    OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? "" : reader.GetString(reader.GetOrdinal("OrderNo")),
                    CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                    MachineModel = reader.IsDBNull(reader.GetOrdinal("MachineModel")) ? null : reader.GetString(reader.GetOrdinal("MachineModel")),
                    RollerType = reader.IsDBNull(reader.GetOrdinal("RollerType")) ? null : reader.GetString(reader.GetOrdinal("RollerType")),
                    NumberOfTeeth = reader.IsDBNull(reader.GetOrdinal("NumberOfTeeth")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NumberOfTeeth")),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                });
            }
            return list;
        }

        public async Task<(int RequisitionsUpdated, int ChallansUpdated)> ChangeCustomerAsync(
            int orderId, string? orderNo, int oldCustomerId, string? oldCustomerName,
            int newCustomerId, string newCustomerName, string? changedBy, string? notes)
        {
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var tx = connection.BeginTransaction();
            try
            {
                // 1) The order itself
                using (var cmd = new SqlCommand(
                    "UPDATE Orders SET CustomerId=@New, CustomerName=@NewName, UpdatedAt=GETUTCDATE(), UpdatedBy=@By WHERE Id=@OrderId", connection, tx))
                {
                    cmd.Parameters.AddWithValue("@New", newCustomerId);
                    cmd.Parameters.AddWithValue("@NewName", (object?)newCustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@By", (object?)changedBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    await cmd.ExecuteNonQueryAsync();
                }

                // 2) Cascade denormalized CustomerName to child records (linked by OrderId)
                int reqUpdated, challanUpdated;
                using (var cmd = new SqlCommand(
                    "UPDATE Stores_MaterialRequisitions SET CustomerName=@NewName WHERE OrderId=@OrderId", connection, tx))
                {
                    cmd.Parameters.AddWithValue("@NewName", (object?)newCustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    reqUpdated = await cmd.ExecuteNonQueryAsync();
                }
                using (var cmd = new SqlCommand(
                    "UPDATE Dispatch_DeliveryChallans SET CustomerId=@New, CustomerName=@NewName WHERE OrderId=@OrderId", connection, tx))
                {
                    cmd.Parameters.AddWithValue("@New", newCustomerId);
                    cmd.Parameters.AddWithValue("@NewName", (object?)newCustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    challanUpdated = await cmd.ExecuteNonQueryAsync();
                }

                // 3) Audit
                using (var cmd = new SqlCommand(@"
                    INSERT INTO Orders_CustomerChangeLog
                        (OrderId, OrderNo, OldCustomerId, OldCustomerName, NewCustomerId, NewCustomerName, RequisitionsUpdated, ChallansUpdated, ChangedBy, ChangedAt, Notes)
                    VALUES (@OrderId, @OrderNo, @OldId, @OldName, @NewId, @NewName, @Req, @Challan, @By, GETUTCDATE(), @Notes)", connection, tx))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@OrderNo", (object?)orderNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@OldId", oldCustomerId);
                    cmd.Parameters.AddWithValue("@OldName", (object?)oldCustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewId", newCustomerId);
                    cmd.Parameters.AddWithValue("@NewName", (object?)newCustomerName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Req", reqUpdated);
                    cmd.Parameters.AddWithValue("@Challan", challanUpdated);
                    cmd.Parameters.AddWithValue("@By", (object?)changedBy ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();
                }

                tx.Commit();
                return (reqUpdated, challanUpdated);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<(int Total, int Pending, int InProgress, int Ready, int Completed)> GetSummaryAsync()
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS Total,
                    SUM(CASE WHEN o.Status <> 'Completed' AND ISNULL(o.QtyInProgress,0)=0 AND ISNULL(o.QtyCompleted,0)=0 THEN 1 ELSE 0 END) AS Pending,
                    SUM(CASE WHEN o.Status <> 'Completed' AND NOT (o.Quantity>0 AND o.QtyCompleted>=o.Quantity) AND (ISNULL(o.QtyInProgress,0)>0 OR ISNULL(o.QtyCompleted,0)>0) THEN 1 ELSE 0 END) AS InProgress,
                    SUM(CASE WHEN o.Status <> 'Completed' AND o.Quantity>0 AND o.QtyCompleted>=o.Quantity THEN 1 ELSE 0 END) AS Ready,
                    SUM(CASE WHEN o.Status = 'Completed' THEN 1 ELSE 0 END) AS Completed
                FROM Orders o";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetInt32(0),
                    reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                    reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                );
            }
            return (0, 0, 0, 0, 0);
        }

        public async Task<(IEnumerable<DTOs.Response.PlanningItemResponse> Items, int TotalCount)> GetPlanningItemsAsync(string type, int page, int pageSize, string? search)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 25;

            // Match the dashboard's derivation. Exclude items already completed / ready-to-dispatch
            // (QtyCompleted >= Quantity) — they no longer need planning or production.
            var typeFilter = (type ?? "pending").ToLowerInvariant() == "planned"
                ? "oi.PlanningStatus IN ('Planned','Released') AND oi.QtyCompleted < oi.Quantity"
                : "COALESCE(p.DrawingReviewStatus, o.DrawingReviewStatus) = 'Approved' AND oi.PlanningStatus = 'Not Planned' AND oi.QtyCompleted < oi.Quantity";

            var conditions = new List<string> { $"({typeFilter})" };
            if (!string.IsNullOrWhiteSpace(search))
                conditions.Add("(o.OrderNo LIKE @Search OR ISNULL(o.CustomerName, c.CustomerName) LIKE @Search OR p.ModelName LIKE @Search OR p.PartCode LIKE @Search)");
            var whereClause = "WHERE " + string.Join(" AND ", conditions);

            const string cols = @"
                    oi.Id AS ItemId, oi.OrderId, o.OrderNo, oi.ItemSequence, oi.ProductId,
                    p.ModelName AS ProductName, p.PartCode, p.RollerType, p.NumberOfTeeth,
                    oi.Quantity, oi.DueDate, oi.Priority AS ItemPriority, oi.Status AS ItemStatus,
                    oi.PlanningStatus, COALESCE(p.DrawingReviewStatus, o.DrawingReviewStatus) AS DrawingReviewStatus,
                    ISNULL(o.CustomerName, c.CustomerName) AS CustomerName, o.Status AS OrderStatus, o.Priority AS OrderPriority,
                    (SELECT COUNT(*) FROM Planning_JobCards jc WHERE jc.OrderItemId = oi.Id) AS JobCardCount,
                    (SELECT COUNT(*) FROM Planning_JobCards jc WHERE jc.OrderItemId = oi.Id AND jc.Status = 'Completed') AS CompletedJobCardCount";
            const string fromJoins = @"
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                LEFT JOIN Masters_Products p ON oi.ProductId = p.Id
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id";

            var listQuery = $@"SELECT {cols} {fromJoins} {whereClause}
                ORDER BY oi.DueDate ASC, oi.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var countQuery = $@"SELECT COUNT(*) {fromJoins} {whereClause}";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            int total;
            using (var countCmd = new SqlCommand(countQuery, connection))
            {
                if (!string.IsNullOrWhiteSpace(search)) countCmd.Parameters.AddWithValue("@Search", $"%{search}%");
                total = (int)await countCmd.ExecuteScalarAsync();
            }

            var items = new List<DTOs.Response.PlanningItemResponse>();
            using (var command = new SqlCommand(listQuery, connection))
            {
                command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                command.Parameters.AddWithValue("@PageSize", pageSize);
                if (!string.IsNullOrWhiteSpace(search)) command.Parameters.AddWithValue("@Search", $"%{search}%");
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    items.Add(new DTOs.Response.PlanningItemResponse
                    {
                        ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                        OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                        OrderNo = reader.IsDBNull(reader.GetOrdinal("OrderNo")) ? "" : reader.GetString(reader.GetOrdinal("OrderNo")),
                        ItemSequence = reader.IsDBNull(reader.GetOrdinal("ItemSequence")) ? null : reader.GetString(reader.GetOrdinal("ItemSequence")),
                        ProductId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ProductId")),
                        ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),
                        PartCode = reader.IsDBNull(reader.GetOrdinal("PartCode")) ? null : reader.GetString(reader.GetOrdinal("PartCode")),
                        RollerType = reader.IsDBNull(reader.GetOrdinal("RollerType")) ? null : reader.GetString(reader.GetOrdinal("RollerType")),
                        NumberOfTeeth = reader.IsDBNull(reader.GetOrdinal("NumberOfTeeth")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NumberOfTeeth")),
                        Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                        DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DueDate")),
                        ItemPriority = reader.IsDBNull(reader.GetOrdinal("ItemPriority")) ? null : reader.GetString(reader.GetOrdinal("ItemPriority")),
                        ItemStatus = reader.IsDBNull(reader.GetOrdinal("ItemStatus")) ? null : reader.GetString(reader.GetOrdinal("ItemStatus")),
                        PlanningStatus = reader.IsDBNull(reader.GetOrdinal("PlanningStatus")) ? null : reader.GetString(reader.GetOrdinal("PlanningStatus")),
                        DrawingReviewStatus = reader.IsDBNull(reader.GetOrdinal("DrawingReviewStatus")) ? null : reader.GetString(reader.GetOrdinal("DrawingReviewStatus")),
                        CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                        OrderStatus = reader.IsDBNull(reader.GetOrdinal("OrderStatus")) ? null : reader.GetString(reader.GetOrdinal("OrderStatus")),
                        OrderPriority = reader.IsDBNull(reader.GetOrdinal("OrderPriority")) ? null : reader.GetString(reader.GetOrdinal("OrderPriority")),
                        JobCardCount = reader.GetInt32(reader.GetOrdinal("JobCardCount")),
                        CompletedJobCardCount = reader.GetInt32(reader.GetOrdinal("CompletedJobCardCount")),
                    });
                }
            }
            return (items, total);
        }

        public async Task<(int TotalOrders, int PendingPlanning, int Planned, int MaterialShortage)> GetPlanningSummaryAsync()
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM Orders) AS TotalOrders,
                    (SELECT COUNT(*) FROM Orders_OrderItems oi
                        JOIN Orders o ON oi.OrderId = o.Id
                        LEFT JOIN Masters_Products p ON oi.ProductId = p.Id
                        WHERE COALESCE(p.DrawingReviewStatus, o.DrawingReviewStatus) = 'Approved' AND oi.PlanningStatus = 'Not Planned' AND oi.QtyCompleted < oi.Quantity) AS PendingPlanning,
                    (SELECT COUNT(*) FROM Orders_OrderItems WHERE PlanningStatus IN ('Planned','Released') AND QtyCompleted < Quantity) AS Planned,
                    (SELECT COUNT(*) FROM Planning_JobCards WHERE Status = 'Pending Material') AS MaterialShortage";
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            }
            return (0, 0, 0, 0);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE CustomerId = @CustomerId
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CustomerId", customerId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByProductIdAsync(int productId)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE ProductId = @ProductId
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status = @Status
                ORDER BY CreatedAt DESC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<int> InsertAsync(Order order)
        {
            const string query = @"
                INSERT INTO Orders (
                    OrderNo, OrderDate, DueDate, AdjustedDueDate,
                    CustomerId, CustomerName, ProductId, ProductName, Quantity, OriginalQuantity,
                    QtyCompleted, QtyRejected, QtyInProgress, QtyScrap,
                    Status, Priority, PlanningStatus,
                    OrderSource, AgentCustomerId, AgentCommission, SchedulingStrategy,
                    DrawingReviewStatus, DrawingReviewedBy, DrawingReviewedAt, DrawingReviewNotes,
                    PrimaryDrawingId, DrawingSource, LinkedProductTemplateId,
                    CustomerMachine, MaterialGradeRemark,
                    CurrentProcess, CurrentMachine, CurrentOperator,
                    ProductionStartDate, ProductionEndDate,
                    DelayReason, RescheduleCount,
                    MaterialGradeApproved, MaterialGradeApprovalDate, MaterialGradeApprovedBy,
                    OrderValue, AdvancePayment, BalancePayment,
                    CreatedAt, CreatedBy, Version
                ) VALUES (
                    @OrderNo, @OrderDate, @DueDate, @AdjustedDueDate,
                    @CustomerId, @CustomerName, @ProductId, @ProductName, @Quantity, @OriginalQuantity,
                    @QtyCompleted, @QtyRejected, @QtyInProgress, @QtyScrap,
                    @Status, @Priority, @PlanningStatus,
                    @OrderSource, @AgentCustomerId, @AgentCommission, @SchedulingStrategy,
                    @DrawingReviewStatus, @DrawingReviewedBy, @DrawingReviewedAt, @DrawingReviewNotes,
                    @PrimaryDrawingId, @DrawingSource, @LinkedProductTemplateId,
                    @CustomerMachine, @MaterialGradeRemark,
                    @CurrentProcess, @CurrentMachine, @CurrentOperator,
                    @ProductionStartDate, @ProductionEndDate,
                    @DelayReason, @RescheduleCount,
                    @MaterialGradeApproved, @MaterialGradeApprovalDate, @MaterialGradeApprovedBy,
                    @OrderValue, @AdvancePayment, @BalancePayment,
                    @CreatedAt, @CreatedBy, @Version
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            order.Id = 0;
            order.CreatedAt = DateTime.UtcNow;

            AddOrderParameters(command, order);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var orderId = result != null ? Convert.ToInt32(result) : 0;

            return orderId;
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            const string query = @"
                UPDATE Orders SET
                    OrderDate = @OrderDate,
                    DueDate = @DueDate,
                    AdjustedDueDate = @AdjustedDueDate,
                    CustomerId = @CustomerId,
                    CustomerName = @CustomerName,
                    ProductId = @ProductId,
                    ProductName = @ProductName,
                    Quantity = @Quantity,
                    OriginalQuantity = @OriginalQuantity,
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    QtyScrap = @QtyScrap,
                    Status = @Status,
                    Priority = @Priority,
                    PlanningStatus = @PlanningStatus,
                    OrderSource = @OrderSource,
                    AgentCustomerId = @AgentCustomerId,
                    AgentCommission = @AgentCommission,
                    SchedulingStrategy = @SchedulingStrategy,
                    DrawingReviewStatus = @DrawingReviewStatus,
                    DrawingReviewedBy = @DrawingReviewedBy,
                    DrawingReviewedAt = @DrawingReviewedAt,
                    DrawingReviewNotes = @DrawingReviewNotes,
                    PrimaryDrawingId = @PrimaryDrawingId,
                    DrawingSource = @DrawingSource,
                    LinkedProductTemplateId = @LinkedProductTemplateId,
                    CustomerMachine = @CustomerMachine,
                    MaterialGradeRemark = @MaterialGradeRemark,
                    CurrentProcess = @CurrentProcess,
                    CurrentMachine = @CurrentMachine,
                    CurrentOperator = @CurrentOperator,
                    ProductionStartDate = @ProductionStartDate,
                    ProductionEndDate = @ProductionEndDate,
                    DelayReason = @DelayReason,
                    RescheduleCount = @RescheduleCount,
                    MaterialGradeApproved = @MaterialGradeApproved,
                    MaterialGradeApprovalDate = @MaterialGradeApprovalDate,
                    MaterialGradeApprovedBy = @MaterialGradeApprovedBy,
                    OrderValue = @OrderValue,
                    AdvancePayment = @AdvancePayment,
                    BalancePayment = @BalancePayment,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @Version
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            order.UpdatedAt = DateTime.UtcNow;

            AddOrderParameters(command, order);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string query = "DELETE FROM Orders WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateDrawingReviewStatusAsync(int id, string status, string reviewedBy, string? notes)
        {
            const string query = @"
                UPDATE Orders SET
                    DrawingReviewStatus = @Status,
                    DrawingReviewedBy = @ReviewedBy,
                    DrawingReviewedAt = @ReviewedAt,
                    DrawingReviewNotes = @Notes,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@ReviewedBy", reviewedBy);
            command.Parameters.AddWithValue("@ReviewedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Notes", (object?)notes ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePlanningStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Orders SET
                    PlanningStatus = @Status,
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

        public async Task<bool> UpdateQuantitiesAsync(int id, int qtyCompleted, int qtyRejected, int qtyInProgress)
        {
            const string query = @"
                UPDATE Orders SET
                    QtyCompleted = @QtyCompleted,
                    QtyRejected = @QtyRejected,
                    QtyInProgress = @QtyInProgress,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@QtyCompleted", qtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", qtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", qtyInProgress);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string query = @"
                UPDATE Orders SET
                    Status = @Status,
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

        public async Task<IEnumerable<Order>> GetPendingDrawingReviewAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE DrawingReviewStatus = 'Pending'
                ORDER BY OrderDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetReadyForPlanningAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE DrawingReviewStatus = 'Approved'
                  AND PlanningStatus = 'Not Planned'
                ORDER BY Priority DESC, DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetInProgressOrdersAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status = 'In Progress'
                ORDER BY DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetDelayedOrdersAsync()
        {
            const string query = @"
                SELECT * FROM Orders
                WHERE Status != 'Completed'
                  AND Status != 'Cancelled'
                  AND DueDate < GETDATE()
                ORDER BY DueDate ASC";

            var orders = new List<Order>();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                orders.Add(MapToOrder(reader));
            }

            return orders;
        }

        public async Task<bool> UpdateWithVersionCheckAsync(Order order)
        {
            // Must persist every field UpdateOrderAsync sets — this previously wrote only
            // 6 columns and silently dropped AdjustedDueDate, Priority, DelayReason, etc.
            const string query = @"
                UPDATE Orders SET
                    OrderDate = @OrderDate,
                    DueDate = @DueDate,
                    AdjustedDueDate = @AdjustedDueDate,
                    CustomerId = @CustomerId,
                    ProductId = @ProductId,
                    Quantity = @Quantity,
                    Status = @Status,
                    Priority = @Priority,
                    PlanningStatus = @PlanningStatus,
                    OrderValue = @OrderValue,
                    AdvancePayment = @AdvancePayment,
                    BalancePayment = @BalancePayment,
                    DelayReason = @DelayReason,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy,
                    Version = @NewVersion
                WHERE Id = @Id AND Version = @CurrentVersion";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            var newVersion = order.Version + 1;

            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@DueDate", order.DueDate);
            command.Parameters.AddWithValue("@AdjustedDueDate", (object?)order.AdjustedDueDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@ProductId", order.ProductId);
            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@Priority", order.Priority);
            command.Parameters.AddWithValue("@PlanningStatus", (object?)order.PlanningStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@OrderValue", (object?)order.OrderValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@AdvancePayment", (object?)order.AdvancePayment ?? DBNull.Value);
            command.Parameters.AddWithValue("@BalancePayment", (object?)order.BalancePayment ?? DBNull.Value);
            command.Parameters.AddWithValue("@DelayReason", (object?)order.DelayReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)order.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentVersion", order.Version);
            command.Parameters.AddWithValue("@NewVersion", newVersion);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                order.Version = newVersion;
                return true;
            }

            return false;
        }

        public async Task<int> GetVersionAsync(int id)
        {
            const string query = "SELECT Version FROM Orders WHERE Id = @Id";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public async Task<bool> SyncDrawingReviewStatusByProductAsync(int productId, string reviewedBy)
        {
            // Update single-product orders (no order items) where ProductId matches
            const string singleQuery = @"
                UPDATE Orders SET
                    DrawingReviewStatus = 'Approved',
                    DrawingReviewedBy = @ReviewedBy,
                    DrawingReviewedAt = @ReviewedAt,
                    UpdatedAt = @UpdatedAt
                WHERE ProductId = @ProductId
                  AND DrawingReviewStatus != 'Approved'
                  AND NOT EXISTS (SELECT 1 FROM Orders_OrderItems WHERE OrderId = Orders.Id)";

            // Update multi-product orders where this product is an item AND all items' products are now approved
            const string multiQuery = @"
                UPDATE Orders SET
                    DrawingReviewStatus = 'Approved',
                    DrawingReviewedBy = @ReviewedBy,
                    DrawingReviewedAt = @ReviewedAt,
                    UpdatedAt = @UpdatedAt
                WHERE DrawingReviewStatus != 'Approved'
                  AND EXISTS (
                      SELECT 1 FROM Orders_OrderItems oi
                      WHERE oi.OrderId = Orders.Id AND oi.ProductId = @ProductId
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM Orders_OrderItems oi
                      JOIN Masters_Products p ON p.Id = oi.ProductId
                      WHERE oi.OrderId = Orders.Id
                        AND p.DrawingReviewStatus != 'Approved'
                  )";

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            await connection.OpenAsync();

            using (var cmd = new SqlCommand(singleQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ReviewedBy", reviewedBy);
                cmd.Parameters.AddWithValue("@ReviewedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                await cmd.ExecuteNonQueryAsync();
            }

            using (var cmd = new SqlCommand(multiQuery, connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ReviewedBy", reviewedBy);
                cmd.Parameters.AddWithValue("@ReviewedAt", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        // Helper Methods

        private static Order MapToOrder(SqlDataReader reader)
        {
            return new Order
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                OrderNo = reader.GetString(reader.GetOrdinal("OrderNo")),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                DueDate = reader.GetDateTime(reader.GetOrdinal("DueDate")),
                AdjustedDueDate = reader.IsDBNull(reader.GetOrdinal("AdjustedDueDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("AdjustedDueDate")),

                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("CustomerName")) ? null : reader.GetString(reader.GetOrdinal("CustomerName")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                ProductName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName")),

                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                OriginalQuantity = reader.GetInt32(reader.GetOrdinal("OriginalQuantity")),
                QtyCompleted = reader.GetInt32(reader.GetOrdinal("QtyCompleted")),
                QtyRejected = reader.GetInt32(reader.GetOrdinal("QtyRejected")),
                QtyInProgress = reader.GetInt32(reader.GetOrdinal("QtyInProgress")),
                QtyScrap = reader.GetInt32(reader.GetOrdinal("QtyScrap")),

                Status = reader.GetString(reader.GetOrdinal("Status")),
                Priority = reader.GetString(reader.GetOrdinal("Priority")),
                PlanningStatus = reader.GetString(reader.GetOrdinal("PlanningStatus")),

                OrderSource = reader.GetString(reader.GetOrdinal("OrderSource")),
                AgentCustomerId = reader.IsDBNull(reader.GetOrdinal("AgentCustomerId"))
                    ? null : reader.GetInt32(reader.GetOrdinal("AgentCustomerId")),
                AgentCommission = reader.IsDBNull(reader.GetOrdinal("AgentCommission"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("AgentCommission")),
                SchedulingStrategy = reader.GetString(reader.GetOrdinal("SchedulingStrategy")),

                DrawingReviewStatus = reader.GetString(reader.GetOrdinal("DrawingReviewStatus")),
                DrawingReviewedBy = reader.IsDBNull(reader.GetOrdinal("DrawingReviewedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("DrawingReviewedBy")),
                DrawingReviewedAt = reader.IsDBNull(reader.GetOrdinal("DrawingReviewedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("DrawingReviewedAt")),
                DrawingReviewNotes = reader.IsDBNull(reader.GetOrdinal("DrawingReviewNotes"))
                    ? null : reader.GetString(reader.GetOrdinal("DrawingReviewNotes")),

                PrimaryDrawingId = reader.IsDBNull(reader.GetOrdinal("PrimaryDrawingId"))
                    ? null : reader.GetInt32(reader.GetOrdinal("PrimaryDrawingId")),
                DrawingSource = reader.IsDBNull(reader.GetOrdinal("DrawingSource"))
                    ? null : reader.GetString(reader.GetOrdinal("DrawingSource")),
                LinkedProductTemplateId = reader.IsDBNull(reader.GetOrdinal("LinkedProductTemplateId"))
                    ? null : reader.GetInt32(reader.GetOrdinal("LinkedProductTemplateId")),

                CustomerMachine = reader.IsDBNull(reader.GetOrdinal("CustomerMachine"))
                    ? null : reader.GetString(reader.GetOrdinal("CustomerMachine")),
                MaterialGradeRemark = reader.IsDBNull(reader.GetOrdinal("MaterialGradeRemark"))
                    ? null : reader.GetString(reader.GetOrdinal("MaterialGradeRemark")),

                CurrentProcess = reader.IsDBNull(reader.GetOrdinal("CurrentProcess"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentProcess")),
                CurrentMachine = reader.IsDBNull(reader.GetOrdinal("CurrentMachine"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentMachine")),
                CurrentOperator = reader.IsDBNull(reader.GetOrdinal("CurrentOperator"))
                    ? null : reader.GetString(reader.GetOrdinal("CurrentOperator")),

                ProductionStartDate = reader.IsDBNull(reader.GetOrdinal("ProductionStartDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ProductionStartDate")),
                ProductionEndDate = reader.IsDBNull(reader.GetOrdinal("ProductionEndDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ProductionEndDate")),

                DelayReason = reader.IsDBNull(reader.GetOrdinal("DelayReason"))
                    ? null : reader.GetString(reader.GetOrdinal("DelayReason")),
                RescheduleCount = reader.GetInt32(reader.GetOrdinal("RescheduleCount")),

                MaterialGradeApproved = reader.GetBoolean(reader.GetOrdinal("MaterialGradeApproved")),
                MaterialGradeApprovalDate = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovalDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("MaterialGradeApprovalDate")),
                MaterialGradeApprovedBy = reader.IsDBNull(reader.GetOrdinal("MaterialGradeApprovedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("MaterialGradeApprovedBy")),

                OrderValue = reader.IsDBNull(reader.GetOrdinal("OrderValue"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("OrderValue")),
                AdvancePayment = reader.IsDBNull(reader.GetOrdinal("AdvancePayment"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("AdvancePayment")),
                BalancePayment = reader.IsDBNull(reader.GetOrdinal("BalancePayment"))
                    ? null : reader.GetDecimal(reader.GetOrdinal("BalancePayment")),

                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                Version = reader.GetInt32(reader.GetOrdinal("Version"))
            };
        }

        private static void AddOrderParameters(SqlCommand command, Order order)
        {
            command.Parameters.AddWithValue("@Id", order.Id);
            command.Parameters.AddWithValue("@OrderNo", order.OrderNo);
            command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
            command.Parameters.AddWithValue("@DueDate", order.DueDate);
            command.Parameters.AddWithValue("@AdjustedDueDate", (object?)order.AdjustedDueDate ?? DBNull.Value);

            command.Parameters.AddWithValue("@CustomerId", order.CustomerId);
            command.Parameters.AddWithValue("@CustomerName", (object?)order.CustomerName ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductId", order.ProductId);
            command.Parameters.AddWithValue("@ProductName", (object?)order.ProductName ?? DBNull.Value);

            command.Parameters.AddWithValue("@Quantity", order.Quantity);
            command.Parameters.AddWithValue("@OriginalQuantity", order.OriginalQuantity);
            command.Parameters.AddWithValue("@QtyCompleted", order.QtyCompleted);
            command.Parameters.AddWithValue("@QtyRejected", order.QtyRejected);
            command.Parameters.AddWithValue("@QtyInProgress", order.QtyInProgress);
            command.Parameters.AddWithValue("@QtyScrap", order.QtyScrap);

            command.Parameters.AddWithValue("@Status", order.Status);
            command.Parameters.AddWithValue("@Priority", order.Priority);
            command.Parameters.AddWithValue("@PlanningStatus", order.PlanningStatus);

            command.Parameters.AddWithValue("@OrderSource", order.OrderSource);
            command.Parameters.AddWithValue("@AgentCustomerId", (object?)order.AgentCustomerId ?? DBNull.Value);
            command.Parameters.AddWithValue("@AgentCommission", (object?)order.AgentCommission ?? DBNull.Value);
            command.Parameters.AddWithValue("@SchedulingStrategy", order.SchedulingStrategy);

            command.Parameters.AddWithValue("@DrawingReviewStatus", order.DrawingReviewStatus);
            command.Parameters.AddWithValue("@DrawingReviewedBy", (object?)order.DrawingReviewedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingReviewedAt", (object?)order.DrawingReviewedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingReviewNotes", (object?)order.DrawingReviewNotes ?? DBNull.Value);

            command.Parameters.AddWithValue("@PrimaryDrawingId", (object?)order.PrimaryDrawingId ?? DBNull.Value);
            command.Parameters.AddWithValue("@DrawingSource", (object?)order.DrawingSource ?? DBNull.Value);
            command.Parameters.AddWithValue("@LinkedProductTemplateId", (object?)order.LinkedProductTemplateId ?? DBNull.Value);

            command.Parameters.AddWithValue("@CustomerMachine", (object?)order.CustomerMachine ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeRemark", (object?)order.MaterialGradeRemark ?? DBNull.Value);

            command.Parameters.AddWithValue("@CurrentProcess", (object?)order.CurrentProcess ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentMachine", (object?)order.CurrentMachine ?? DBNull.Value);
            command.Parameters.AddWithValue("@CurrentOperator", (object?)order.CurrentOperator ?? DBNull.Value);

            command.Parameters.AddWithValue("@ProductionStartDate", (object?)order.ProductionStartDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductionEndDate", (object?)order.ProductionEndDate ?? DBNull.Value);

            command.Parameters.AddWithValue("@DelayReason", (object?)order.DelayReason ?? DBNull.Value);
            command.Parameters.AddWithValue("@RescheduleCount", order.RescheduleCount);

            command.Parameters.AddWithValue("@MaterialGradeApproved", order.MaterialGradeApproved);
            command.Parameters.AddWithValue("@MaterialGradeApprovalDate", (object?)order.MaterialGradeApprovalDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@MaterialGradeApprovedBy", (object?)order.MaterialGradeApprovedBy ?? DBNull.Value);

            command.Parameters.AddWithValue("@OrderValue", (object?)order.OrderValue ?? DBNull.Value);
            command.Parameters.AddWithValue("@AdvancePayment", (object?)order.AdvancePayment ?? DBNull.Value);
            command.Parameters.AddWithValue("@BalancePayment", (object?)order.BalancePayment ?? DBNull.Value);

            command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);
            command.Parameters.AddWithValue("@CreatedBy", (object?)order.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedAt", (object?)order.UpdatedAt ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)order.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@Version", order.Version);
        }
    }
}
