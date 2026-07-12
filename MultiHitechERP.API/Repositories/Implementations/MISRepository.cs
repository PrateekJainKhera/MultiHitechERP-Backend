using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    /// <summary>
    /// Real MIS aggregates in one round-trip (multiple result sets).
    /// </summary>
    public class MISRepository : IMISRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MISRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MISOverviewResponse> GetOverviewAsync()
        {
            const string sql = @"
                -- 1: orders per month (last 12 months)
                SELECT FORMAT(o.OrderDate, 'yyyy-MM') AS Mo, COUNT(*) AS Cnt, ISNULL(SUM(o.Quantity),0) AS Qty
                FROM Orders o
                WHERE o.OrderDate >= DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
                GROUP BY FORMAT(o.OrderDate, 'yyyy-MM')
                ORDER BY Mo;

                -- 2: order status counts
                SELECT o.Status AS Label, COUNT(*) AS Cnt FROM Orders o GROUP BY o.Status ORDER BY Cnt DESC;

                -- 3: order source counts
                SELECT ISNULL(o.OrderSource,'Direct') AS Label, COUNT(*) AS Cnt FROM Orders o GROUP BY o.OrderSource ORDER BY Cnt DESC;

                -- 4: top customers by orders
                SELECT TOP 10 ISNULL(c.CustomerName, o.CustomerName) AS CustomerName,
                       COUNT(*) AS Orders, ISNULL(SUM(o.Quantity),0) AS Qty
                FROM Orders o
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
                GROUP BY ISNULL(c.CustomerName, o.CustomerName)
                ORDER BY Orders DESC;

                -- 5: challans per month (last 12 months)
                SELECT FORMAT(dc.ChallanDate, 'yyyy-MM') AS Mo, COUNT(*) AS Cnt, ISNULL(SUM(dc.QuantityDispatched),0) AS Qty
                FROM Dispatch_DeliveryChallans dc
                WHERE dc.ChallanDate >= DATEADD(MONTH, -11, DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1))
                GROUP BY FORMAT(dc.ChallanDate, 'yyyy-MM')
                ORDER BY Mo;

                -- 6: roller type mix (order items -> product)
                SELECT ISNULL(p.RollerType,'Unknown') AS Label, COUNT(*) AS Cnt
                FROM Orders_OrderItems oi
                LEFT JOIN Masters_Products p ON oi.ProductId = p.Id
                GROUP BY p.RollerType ORDER BY Cnt DESC;

                -- 7: scalar totals
                SELECT
                    (SELECT COUNT(*) FROM Orders)                                        AS TotalOrders,
                    (SELECT COUNT(*) FROM Dispatch_DeliveryChallans)                     AS TotalChallans,
                    (SELECT ISNULL(SUM(QuantityDispatched),0) FROM Dispatch_DeliveryChallans) AS TotalDispatchedQty,
                    (SELECT COUNT(*) FROM Planning_JobCards)                             AS JobCardsTotal,
                    (SELECT COUNT(*) FROM Planning_JobCards WHERE ProductionStatus='Completed') AS JobCardsCompletedSteps,
                    (SELECT ISNULL(SUM(RejectedQty),0) FROM Planning_JobCards)           AS TotalRejectedQty,
                    (SELECT COUNT(*) FROM Planning_JobCards WHERE RejectedQty > 0)       AS RejectionJobCards,
                    (SELECT COUNT(*) FROM Planning_JobCards WHERE JobCardNo LIKE '%-RW%') AS ReworkJobCards;";

            var result = new MISOverviewResponse();

            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.OrdersPerMonth.Add(new MonthCount { Month = reader.GetString(0), Count = reader.GetInt32(1), Qty = reader.GetInt32(2) });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.OrderStatusCounts.Add(new StatusCount { Label = reader.GetString(0), Count = reader.GetInt32(1) });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.OrderSourceCounts.Add(new StatusCount { Label = reader.GetString(0), Count = reader.GetInt32(1) });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.TopCustomers.Add(new TopCustomerRow
                {
                    CustomerName = reader.IsDBNull(0) ? "Unknown" : reader.GetString(0),
                    Orders = reader.GetInt32(1),
                    Qty = reader.GetInt32(2),
                });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.ChallansPerMonth.Add(new MonthCount { Month = reader.GetString(0), Count = reader.GetInt32(1), Qty = reader.GetInt32(2) });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.RollerTypeCounts.Add(new StatusCount { Label = reader.GetString(0), Count = reader.GetInt32(1) });

            await reader.NextResultAsync();
            if (await reader.ReadAsync())
            {
                result.TotalOrders = reader.GetInt32(0);
                result.TotalChallans = reader.GetInt32(1);
                result.TotalDispatchedQty = reader.GetInt32(2);
                result.JobCardsTotal = reader.GetInt32(3);
                result.JobCardsCompletedSteps = reader.GetInt32(4);
                result.TotalRejectedQty = reader.GetInt32(5);
                result.RejectionJobCards = reader.GetInt32(6);
                result.ReworkJobCards = reader.GetInt32(7);
            }

            return result;
        }

        // ── Machine Model report ──────────────────────────────────────────────

        public async Task<MachineModelsResponse> GetMachineModelsAsync()
        {
            const string sql = @"
                -- 1: top 10 product combos (Model · Roller · Teeth) by orders received
                SELECT TOP 10 p.ModelName, p.RollerType, p.NumberOfTeeth,
                       COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS TotalQty, MAX(o.OrderDate) AS LastOrderDate
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                JOIN Masters_Products p ON oi.ProductId = p.Id
                GROUP BY p.ModelName, p.RollerType, p.NumberOfTeeth
                ORDER BY Orders DESC, TotalQty DESC;

                -- 2: distinct model names (for the search list)
                SELECT p.ModelName, COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS TotalQty
                FROM Orders_OrderItems oi
                JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE p.ModelName IS NOT NULL AND p.ModelName <> ''
                GROUP BY p.ModelName
                ORDER BY Orders DESC;";

            var result = new MachineModelsResponse();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.Top10.Add(ReadRow(reader));

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Models.Add(new MachineModelName
                {
                    ModelName = reader.GetString(0),
                    Orders = reader.GetInt32(1),
                    TotalQty = reader.GetInt32(2),
                });

            return result;
        }

        public async Task<MachineModelDetailResponse> GetMachineModelDetailAsync(string modelName)
        {
            const string sql = @"
                -- 1: variants (roller/teeth combos) of this model
                SELECT p.ModelName, p.RollerType, p.NumberOfTeeth,
                       COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS TotalQty, MAX(o.OrderDate) AS LastOrderDate
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE p.ModelName = @Model
                GROUP BY p.ModelName, p.RollerType, p.NumberOfTeeth
                ORDER BY Orders DESC, TotalQty DESC;

                -- 2: monthly (last 24 months)
                SELECT FORMAT(o.OrderDate,'yyyy-MM') AS Period, COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS Qty
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE p.ModelName = @Model
                  AND o.OrderDate >= DATEADD(MONTH,-23,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))
                GROUP BY FORMAT(o.OrderDate,'yyyy-MM') ORDER BY Period;

                -- 3: yearly
                SELECT CONVERT(varchar(4),YEAR(o.OrderDate)) AS Period, COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS Qty
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                JOIN Masters_Products p ON oi.ProductId = p.Id
                WHERE p.ModelName = @Model
                GROUP BY YEAR(o.OrderDate) ORDER BY Period;";

            var result = new MachineModelDetailResponse { ModelName = modelName };
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Model", modelName);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = ReadRow(reader);
                result.Variants.Add(row);
                result.TotalOrders += row.Orders;
                result.TotalQty += row.TotalQty;
            }

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Monthly.Add(new ModelPeriodCount { Period = reader.GetString(0), Orders = reader.GetInt32(1), Qty = reader.GetInt32(2) });

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                result.Yearly.Add(new ModelPeriodCount { Period = reader.GetString(0), Orders = reader.GetInt32(1), Qty = reader.GetInt32(2) });

            return result;
        }

        public async Task<List<MachineModelCustomerRow>> GetMachineModelCustomersAsync(string modelName, string? rollerType, int? numberOfTeeth)
        {
            const string sql = @"
                SELECT ISNULL(c.CustomerName, o.CustomerName) AS CustomerName,
                       COUNT(*) AS Orders, ISNULL(SUM(oi.Quantity),0) AS TotalQty, MAX(o.OrderDate) AS LastOrderDate
                FROM Orders_OrderItems oi
                JOIN Orders o ON oi.OrderId = o.Id
                JOIN Masters_Products p ON oi.ProductId = p.Id
                LEFT JOIN Masters_Customers c ON o.CustomerId = c.Id
                WHERE p.ModelName = @Model
                  AND (@Roller IS NULL OR p.RollerType = @Roller)
                  AND (@Teeth IS NULL OR p.NumberOfTeeth = @Teeth)
                GROUP BY ISNULL(c.CustomerName, o.CustomerName)
                ORDER BY LastOrderDate DESC";

            var rows = new List<MachineModelCustomerRow>();
            using var connection = (SqlConnection)_connectionFactory.CreateConnection();
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Model", modelName);
            command.Parameters.AddWithValue("@Roller", (object?)rollerType ?? DBNull.Value);
            command.Parameters.AddWithValue("@Teeth", (object?)numberOfTeeth ?? DBNull.Value);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                rows.Add(new MachineModelCustomerRow
                {
                    CustomerName = reader.IsDBNull(0) ? "—" : reader.GetString(0),
                    Orders = reader.GetInt32(1),
                    TotalQty = reader.GetInt32(2),
                    LastOrderDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                });
            return rows;
        }

        private static MachineModelRow ReadRow(SqlDataReader reader) => new()
        {
            ModelName = reader.GetString(0),
            RollerType = reader.IsDBNull(1) ? null : reader.GetString(1),
            NumberOfTeeth = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            Orders = reader.GetInt32(3),
            TotalQty = reader.GetInt32(4),
            LastOrderDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
        };
    }
}
