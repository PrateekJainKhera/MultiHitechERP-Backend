using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class ComponentIssueRepository : IComponentIssueRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ComponentIssueRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(ComponentIssue issue)
        {
            const string sql = @"
                INSERT INTO Stores_ComponentIssues
                    (IssueNo, IssueDate, ComponentId, ComponentName, PartNumber, Unit,
                     IssuedQty, RequestedBy, IssuedBy, Remarks, CreatedAt, CreatedBy)
                VALUES
                    (@IssueNo, @IssueDate, @ComponentId, @ComponentName, @PartNumber, @Unit,
                     @IssuedQty, @RequestedBy, @IssuedBy, @Remarks, @CreatedAt, @CreatedBy);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@IssueNo", issue.IssueNo);
            cmd.Parameters.AddWithValue("@IssueDate", issue.IssueDate);
            cmd.Parameters.AddWithValue("@ComponentId", issue.ComponentId);
            cmd.Parameters.AddWithValue("@ComponentName", issue.ComponentName);
            cmd.Parameters.AddWithValue("@PartNumber", (object?)issue.PartNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Unit", issue.Unit);
            cmd.Parameters.AddWithValue("@IssuedQty", issue.IssuedQty);
            cmd.Parameters.AddWithValue("@RequestedBy", issue.RequestedBy);
            cmd.Parameters.AddWithValue("@IssuedBy", issue.IssuedBy);
            cmd.Parameters.AddWithValue("@Remarks", (object?)issue.Remarks ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", issue.CreatedAt);
            cmd.Parameters.AddWithValue("@CreatedBy", (object?)issue.CreatedBy ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<IEnumerable<ComponentIssue>> GetAllAsync()
        {
            const string sql = @"
                SELECT Id, IssueNo, IssueDate, ComponentId, ComponentName, PartNumber, Unit,
                       IssuedQty, RequestedBy, IssuedBy, Remarks, CreatedAt, CreatedBy
                FROM Stores_ComponentIssues
                ORDER BY IssueDate DESC";

            var list = new List<ComponentIssue>();
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(Map(reader));
            return list;
        }

        public async Task<IEnumerable<ComponentIssue>> GetByComponentIdAsync(int componentId)
        {
            const string sql = @"
                SELECT Id, IssueNo, IssueDate, ComponentId, ComponentName, PartNumber, Unit,
                       IssuedQty, RequestedBy, IssuedBy, Remarks, CreatedAt, CreatedBy
                FROM Stores_ComponentIssues
                WHERE ComponentId = @ComponentId
                ORDER BY IssueDate DESC";

            var list = new List<ComponentIssue>();
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ComponentId", componentId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(Map(reader));
            return list;
        }

        public async Task<IEnumerable<ComponentWithStockResponse>> GetComponentsWithStockAsync()
        {
            const string sql = @"
                SELECT
                    c.Id, c.PartNumber, c.ComponentName, c.Category, c.Unit,
                    ISNULL(s.AvailableStock, 0) AS AvailableStock,
                    ISNULL(s.CurrentStock,   0) AS CurrentStock,
                    ISNULL(s.Location,       '') AS StockLocation
                FROM Masters_Components c
                LEFT JOIN Inventory_Stock s
                    ON s.ItemType = 'Component' AND s.ItemId = c.Id
                WHERE c.IsActive = 1
                  AND ISNULL(s.AvailableStock, 0) > 0
                ORDER BY c.ComponentName";

            var list = new List<ComponentWithStockResponse>();
            using var conn = (SqlConnection)_connectionFactory.CreateConnection();
            await conn.OpenAsync();
            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ComponentWithStockResponse
                {
                    Id            = reader.GetInt32(reader.GetOrdinal("Id")),
                    PartNumber    = reader.GetString(reader.GetOrdinal("PartNumber")),
                    ComponentName = reader.GetString(reader.GetOrdinal("ComponentName")),
                    Category      = reader.GetString(reader.GetOrdinal("Category")),
                    Unit          = reader.GetString(reader.GetOrdinal("Unit")),
                    AvailableStock = reader.GetDecimal(reader.GetOrdinal("AvailableStock")),
                    CurrentStock   = reader.GetDecimal(reader.GetOrdinal("CurrentStock")),
                    StockLocation  = reader.GetString(reader.GetOrdinal("StockLocation")),
                });
            }
            return list;
        }

        private static ComponentIssue Map(SqlDataReader r) => new()
        {
            Id            = r.GetInt32(r.GetOrdinal("Id")),
            IssueNo       = r.GetString(r.GetOrdinal("IssueNo")),
            IssueDate     = r.GetDateTime(r.GetOrdinal("IssueDate")),
            ComponentId   = r.GetInt32(r.GetOrdinal("ComponentId")),
            ComponentName = r.GetString(r.GetOrdinal("ComponentName")),
            PartNumber    = r.IsDBNull(r.GetOrdinal("PartNumber")) ? null : r.GetString(r.GetOrdinal("PartNumber")),
            Unit          = r.GetString(r.GetOrdinal("Unit")),
            IssuedQty     = r.GetDecimal(r.GetOrdinal("IssuedQty")),
            RequestedBy   = r.GetString(r.GetOrdinal("RequestedBy")),
            IssuedBy      = r.GetString(r.GetOrdinal("IssuedBy")),
            Remarks       = r.IsDBNull(r.GetOrdinal("Remarks")) ? null : r.GetString(r.GetOrdinal("Remarks")),
            CreatedAt     = r.GetDateTime(r.GetOrdinal("CreatedAt")),
            CreatedBy     = r.IsDBNull(r.GetOrdinal("CreatedBy")) ? null : r.GetString(r.GetOrdinal("CreatedBy")),
        };
    }
}
