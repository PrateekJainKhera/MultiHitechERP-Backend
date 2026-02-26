using Dapper;
using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations
{
    public class OSPTrackingRepository : IOSPTrackingRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OSPTrackingRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private SqlConnection GetConnection() =>
            (SqlConnection)_connectionFactory.CreateConnection();

        public async Task<IEnumerable<OSPTracking>> GetAllAsync()
        {
            const string sql = @"
                SELECT o.*, v.VendorName
                FROM Production_OSPTracking o
                JOIN Masters_Vendors v ON v.Id = o.VendorId
                ORDER BY o.SentDate DESC, o.Id DESC";

            using var conn = GetConnection();
            await conn.OpenAsync();
            return await conn.QueryAsync<OSPTracking>(sql);
        }

        public async Task<OSPTracking?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT o.*, v.VendorName
                FROM Production_OSPTracking o
                JOIN Masters_Vendors v ON v.Id = o.VendorId
                WHERE o.Id = @id";

            using var conn = GetConnection();
            await conn.OpenAsync();
            return await conn.QueryFirstOrDefaultAsync<OSPTracking>(sql, new { id });
        }

        /// <summary>
        /// Returns scheduled job cards that do NOT already have an active (Sent) OSP entry.
        /// </summary>
        public async Task<IEnumerable<OSPJobCardOption>> GetAvailableJobCardsAsync()
        {
            const string sql = @"
                SELECT
                    jc.Id           AS JobCardId,
                    jc.JobCardNo,
                    jc.OrderId,
                    jc.OrderNo,
                    jc.OrderItemId,
                    jc.ItemSequence,
                    jc.ChildPartName,
                    jc.ProcessName,
                    jc.Quantity
                FROM Planning_JobCards jc
                WHERE jc.Status = 'Scheduled'
                  AND jc.ProductionStatus NOT IN ('Completed')
                  AND NOT EXISTS (
                      SELECT 1 FROM Production_OSPTracking osp
                      WHERE osp.JobCardId = jc.Id AND osp.Status = 'Sent'
                  )
                ORDER BY jc.CreatedAt DESC";

            using var conn = GetConnection();
            await conn.OpenAsync();
            return await conn.QueryAsync<OSPJobCardOption>(sql);
        }

        public async Task<int> InsertAsync(OSPTracking entry)
        {
            const string sql = @"
                INSERT INTO Production_OSPTracking
                    (JobCardId, JobCardNo, OrderId, OrderNo, OrderItemId, ItemSequence,
                     ChildPartName, ProcessName, VendorId, Quantity,
                     SentDate, ExpectedReturnDate, Status, Notes,
                     CreatedAt, CreatedBy)
                VALUES
                    (@JobCardId, @JobCardNo, @OrderId, @OrderNo, @OrderItemId, @ItemSequence,
                     @ChildPartName, @ProcessName, @VendorId, @Quantity,
                     @SentDate, @ExpectedReturnDate, 'Sent', @Notes,
                     GETUTCDATE(), @CreatedBy);
                SELECT SCOPE_IDENTITY();";

            using var conn = GetConnection();
            await conn.OpenAsync();
            return await conn.ExecuteScalarAsync<int>(sql, entry);
        }

        public async Task<IEnumerable<int>> BatchInsertAsync(IEnumerable<OSPTracking> entries)
        {
            const string sql = @"
                INSERT INTO Production_OSPTracking
                    (JobCardId, JobCardNo, OrderId, OrderNo, OrderItemId, ItemSequence,
                     ChildPartName, ProcessName, VendorId, Quantity,
                     SentDate, ExpectedReturnDate, Status, Notes,
                     CreatedAt, CreatedBy)
                VALUES
                    (@JobCardId, @JobCardNo, @OrderId, @OrderNo, @OrderItemId, @ItemSequence,
                     @ChildPartName, @ProcessName, @VendorId, @Quantity,
                     @SentDate, @ExpectedReturnDate, 'Sent', @Notes,
                     GETUTCDATE(), @CreatedBy);
                SELECT SCOPE_IDENTITY();";

            var ids = new List<int>();
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var entry in entries)
                {
                    var id = await conn.ExecuteScalarAsync<int>(sql, entry, tx);
                    ids.Add(id);
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            return ids;
        }

        /// <summary>
        /// Partial/full receive. Accumulates ReceivedQty + RejectedQty.
        /// When total >= Quantity, status flips to Received and job card is auto-completed.
        /// </summary>
        public async Task MarkReceivedAsync(int id, int receivedQty, int rejectedQty, DateTime actualReturnDate, string? notes, string? updatedBy)
        {
            using var conn = GetConnection();
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                // 1. Accumulate quantities. Flip status to Received when total >= Quantity.
                const string ospSql = @"
                    UPDATE Production_OSPTracking
                    SET ReceivedQty      = ReceivedQty + @receivedQty,
                        RejectedQty      = RejectedQty + @rejectedQty,
                        Status           = CASE
                                               WHEN (ReceivedQty + @receivedQty + RejectedQty + @rejectedQty) >= Quantity
                                               THEN 'Received'
                                               ELSE Status
                                           END,
                        ActualReturnDate = @actualReturnDate,
                        Notes            = CASE WHEN @notes IS NOT NULL THEN @notes ELSE Notes END,
                        UpdatedAt        = GETUTCDATE(),
                        UpdatedBy        = @updatedBy
                    OUTPUT INSERTED.JobCardId, INSERTED.Status
                    WHERE Id = @id AND Status = 'Sent'";

                var result = await conn.QueryFirstOrDefaultAsync<(int JobCardId, string Status)>(
                    ospSql, new { id, receivedQty, rejectedQty, actualReturnDate, notes, updatedBy }, tx);

                // 2. Only auto-complete job card when fully received
                if (result.JobCardId > 0 && result.Status == "Received")
                {
                    const string jcSql = @"
                        UPDATE Planning_JobCards
                        SET ProductionStatus = 'Completed',
                            CompletedQty     = Quantity,
                            ActualEndTime    = GETUTCDATE(),
                            UpdatedAt        = GETUTCDATE()
                        WHERE Id = @jobCardId
                          AND ProductionStatus != 'Completed'";

                    await conn.ExecuteAsync(jcSql, new { jobCardId = result.JobCardId }, tx);
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Returns a map of JobCardId → "Sent" for any job cards
        /// that currently have an active (Status=Sent) OSP entry.
        /// </summary>
        public async Task<Dictionary<int, string>> GetActiveOspStatusByJobCardIdsAsync(IEnumerable<int> jobCardIds)
        {
            var ids = jobCardIds.ToList();
            if (!ids.Any()) return new Dictionary<int, string>();

            const string sql = @"
                SELECT JobCardId, Status
                FROM Production_OSPTracking
                WHERE JobCardId IN @ids
                  AND Status = 'Sent'";

            using var conn = GetConnection();
            await conn.OpenAsync();
            var rows = await conn.QueryAsync<(int JobCardId, string Status)>(sql, new { ids });
            return rows
                .GroupBy(r => r.JobCardId)
                .ToDictionary(g => g.Key, g => g.First().Status);
        }
    }
}
