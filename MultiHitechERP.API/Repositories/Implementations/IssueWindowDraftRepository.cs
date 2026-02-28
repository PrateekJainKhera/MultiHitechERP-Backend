using Microsoft.Data.SqlClient;
using MultiHitechERP.API.Data;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Repositories.Implementations;

public class IssueWindowDraftRepository : IIssueWindowDraftRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public IssueWindowDraftRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> SaveDraftAsync(SaveDraftRequest request)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Generate draft number — use MAX existing sequence for this month to avoid re-use after deletes
            var prefix = $"MIS-{DateTime.Now:yyyyMM}-";
            var countSql = "SELECT ISNULL(MAX(CAST(RIGHT(DraftNo, 3) AS INT)), 0) FROM Stores_IssueWindowDrafts WHERE DraftNo LIKE @Prefix + '%'";
            using var countCmd = new SqlCommand(countSql, connection, transaction);
            countCmd.Parameters.AddWithValue("@Prefix", prefix);
            var count = (int)await countCmd.ExecuteScalarAsync() + 1;
            var draftNo = $"{prefix}{count:D3}";

            var reqIds = string.Join(",", request.RequisitionIds);

            // Insert draft header
            var insertDraftSql = @"
                INSERT INTO Stores_IssueWindowDrafts (DraftNo, RequisitionIds, Status, Notes, CreatedAt)
                VALUES (@DraftNo, @RequisitionIds, 'Draft', @Notes, GETDATE());
                SELECT SCOPE_IDENTITY();";
            using var draftCmd = new SqlCommand(insertDraftSql, connection, transaction);
            draftCmd.Parameters.AddWithValue("@DraftNo", draftNo);
            draftCmd.Parameters.AddWithValue("@RequisitionIds", reqIds);
            draftCmd.Parameters.AddWithValue("@Notes", (object?)request.Notes ?? DBNull.Value);
            var draftId = Convert.ToInt32(await draftCmd.ExecuteScalarAsync());

            // Pre-load MinLengthMM for all materials referenced in bar assignments
            var materialMinLengths = new Dictionary<int, int>();
            var uniqueMaterialIds = request.BarAssignments
                .Where(b => b.MaterialId.HasValue)
                .Select(b => b.MaterialId!.Value)
                .Distinct()
                .ToList();
            if (uniqueMaterialIds.Any())
            {
                var paramNames = uniqueMaterialIds.Select((_, i) => $"@mid{i}").ToList();
                var minLenSql = $"SELECT Id, MinLengthMM FROM Masters_Materials WHERE Id IN ({string.Join(",", paramNames)})";
                using var minLenCmd = new SqlCommand(minLenSql, connection, transaction);
                for (int i = 0; i < uniqueMaterialIds.Count; i++)
                    minLenCmd.Parameters.AddWithValue($"@mid{i}", uniqueMaterialIds[i]);
                using var minLenReader = await minLenCmd.ExecuteReaderAsync();
                while (await minLenReader.ReadAsync())
                    materialMinLengths[minLenReader.GetInt32(0)] = minLenReader.GetInt32(1);
            }

            // Insert bar assignments and their cuts
            var pieceIdsToReserve = new List<int>();

            for (int sortOrder = 0; sortOrder < request.BarAssignments.Count; sortOrder++)
            {
                var bar = request.BarAssignments[sortOrder];
                decimal totalCut = bar.Cuts.Sum(c => c.CutLengthMM);
                decimal remaining = bar.PieceCurrentLengthMM - totalCut;
                int minLen = bar.MaterialId.HasValue && materialMinLengths.TryGetValue(bar.MaterialId.Value, out var ml) ? ml : 300;
                bool willBeScrap = remaining < minLen;

                var insertBarSql = @"
                    INSERT INTO Stores_IssueWindowDraftBarAssignments
                        (DraftId, MaterialId, MaterialName, MaterialCode, Grade, DiameterMM,
                         PieceId, PieceNo, PieceCurrentLengthMM, TotalCutMM, RemainingMM, WillBeScrap, SortOrder)
                    VALUES
                        (@DraftId, @MaterialId, @MaterialName, @MaterialCode, @Grade, @DiameterMM,
                         @PieceId, @PieceNo, @PieceCurrentLengthMM, @TotalCutMM, @RemainingMM, @WillBeScrap, @SortOrder);
                    SELECT SCOPE_IDENTITY();";
                using var barCmd = new SqlCommand(insertBarSql, connection, transaction);
                barCmd.Parameters.AddWithValue("@DraftId", draftId);
                barCmd.Parameters.AddWithValue("@MaterialId", (object?)bar.MaterialId ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@MaterialName", (object?)bar.MaterialName ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@MaterialCode", (object?)bar.MaterialCode ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@Grade", (object?)bar.Grade ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@DiameterMM", (object?)bar.DiameterMM ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@PieceId", bar.PieceId);
                barCmd.Parameters.AddWithValue("@PieceNo", (object?)bar.PieceNo ?? DBNull.Value);
                barCmd.Parameters.AddWithValue("@PieceCurrentLengthMM", bar.PieceCurrentLengthMM);
                barCmd.Parameters.AddWithValue("@TotalCutMM", totalCut);
                barCmd.Parameters.AddWithValue("@RemainingMM", remaining);
                barCmd.Parameters.AddWithValue("@WillBeScrap", willBeScrap);
                barCmd.Parameters.AddWithValue("@SortOrder", sortOrder);
                var barAssignmentId = Convert.ToInt32(await barCmd.ExecuteScalarAsync());

                // Collect piece IDs for reservation
                if (bar.PieceId > 0)
                    pieceIdsToReserve.Add(bar.PieceId);

                for (int cutSort = 0; cutSort < bar.Cuts.Count; cutSort++)
                {
                    var cut = bar.Cuts[cutSort];
                    var insertCutSql = @"
                        INSERT INTO Stores_IssueWindowDraftCuts
                            (BarAssignmentId, DraftId, RequisitionItemId, RequisitionId, CutIndex,
                             CutLengthMM, PartName, JobCardNo, RequisitionNo, MaterialId, SortOrder)
                        VALUES
                            (@BarAssignmentId, @DraftId, @RequisitionItemId, @RequisitionId, @CutIndex,
                             @CutLengthMM, @PartName, @JobCardNo, @RequisitionNo, @MaterialId, @SortOrder)";
                    using var cutCmd = new SqlCommand(insertCutSql, connection, transaction);
                    cutCmd.Parameters.AddWithValue("@BarAssignmentId", barAssignmentId);
                    cutCmd.Parameters.AddWithValue("@DraftId", draftId);
                    cutCmd.Parameters.AddWithValue("@RequisitionItemId", cut.RequisitionItemId);
                    cutCmd.Parameters.AddWithValue("@RequisitionId", (object?)cut.RequisitionId ?? DBNull.Value);
                    cutCmd.Parameters.AddWithValue("@CutIndex", cut.CutIndex);
                    cutCmd.Parameters.AddWithValue("@CutLengthMM", cut.CutLengthMM);
                    cutCmd.Parameters.AddWithValue("@PartName", (object?)cut.PartName ?? DBNull.Value);
                    cutCmd.Parameters.AddWithValue("@JobCardNo", (object?)cut.JobCardNo ?? DBNull.Value);
                    cutCmd.Parameters.AddWithValue("@RequisitionNo", (object?)cut.RequisitionNo ?? DBNull.Value);
                    cutCmd.Parameters.AddWithValue("@MaterialId", (object?)cut.MaterialId ?? DBNull.Value);
                    cutCmd.Parameters.AddWithValue("@SortOrder", cutSort);
                    await cutCmd.ExecuteNonQueryAsync();
                }
            }

            // Reserve pieces — mark as Reserved so they don't appear in available pool for other drafts
            if (pieceIdsToReserve.Count > 0)
            {
                var ids = string.Join(",", pieceIdsToReserve.Distinct());
                var reserveSql = $"UPDATE Stores_MaterialPieces SET Status = 'Reserved', UpdatedAt = GETUTCDATE() WHERE Id IN ({ids})";
                using var reserveCmd = new SqlCommand(reserveSql, connection, transaction);
                await reserveCmd.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return draftId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetDraftsAsync()
    {
        var sql = @"
            SELECT
                d.Id, d.DraftNo, d.Status, d.CreatedAt, d.FinalizedAt, d.IssuedAt,
                LEN(d.RequisitionIds) - LEN(REPLACE(d.RequisitionIds, ',', '')) + 1 AS RequisitionCount,
                (SELECT COUNT(1) FROM Stores_IssueWindowDraftBarAssignments WHERE DraftId = d.Id) AS TotalBars,
                (SELECT COUNT(1) FROM Stores_IssueWindowDraftCuts WHERE DraftId = d.Id) AS TotalCuts
            FROM Stores_IssueWindowDrafts d
            WHERE d.Status IN ('Draft')
            ORDER BY d.CreatedAt DESC";

        return await QueryDraftSummaries(sql);
    }

    public async Task<IEnumerable<IssueWindowDraftSummaryResponse>> GetFinalizedDraftsAsync()
    {
        var sql = @"
            SELECT
                d.Id, d.DraftNo, d.Status, d.CreatedAt, d.FinalizedAt, d.IssuedAt,
                LEN(d.RequisitionIds) - LEN(REPLACE(d.RequisitionIds, ',', '')) + 1 AS RequisitionCount,
                (SELECT COUNT(1) FROM Stores_IssueWindowDraftBarAssignments WHERE DraftId = d.Id) AS TotalBars,
                (SELECT COUNT(1) FROM Stores_IssueWindowDraftCuts WHERE DraftId = d.Id) AS TotalCuts
            FROM Stores_IssueWindowDrafts d
            WHERE d.Status = 'Finalized'
            ORDER BY d.FinalizedAt DESC";

        return await QueryDraftSummaries(sql);
    }

    private async Task<IEnumerable<IssueWindowDraftSummaryResponse>> QueryDraftSummaries(string sql)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var cmd = new SqlCommand(sql, connection);
        using var reader = await cmd.ExecuteReaderAsync();

        var results = new List<IssueWindowDraftSummaryResponse>();
        while (await reader.ReadAsync())
        {
            results.Add(new IssueWindowDraftSummaryResponse
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                DraftNo = reader.GetString(reader.GetOrdinal("DraftNo")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                FinalizedAt = reader.IsDBNull(reader.GetOrdinal("FinalizedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("FinalizedAt")),
                IssuedAt = reader.IsDBNull(reader.GetOrdinal("IssuedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("IssuedAt")),
                RequisitionCount = reader.GetInt32(reader.GetOrdinal("RequisitionCount")),
                TotalBars = reader.GetInt32(reader.GetOrdinal("TotalBars")),
                TotalCuts = reader.GetInt32(reader.GetOrdinal("TotalCuts"))
            });
        }
        return results;
    }

    public async Task<IssueWindowDraftDetailResponse?> GetDraftByIdAsync(int id)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();

        // Load draft header
        var draftSql = "SELECT * FROM Stores_IssueWindowDrafts WHERE Id = @Id";
        using var draftCmd = new SqlCommand(draftSql, connection);
        draftCmd.Parameters.AddWithValue("@Id", id);
        using var draftReader = await draftCmd.ExecuteReaderAsync();

        if (!await draftReader.ReadAsync()) return null;

        var draft = new IssueWindowDraftDetailResponse
        {
            Id = draftReader.GetInt32(draftReader.GetOrdinal("Id")),
            DraftNo = draftReader.GetString(draftReader.GetOrdinal("DraftNo")),
            Status = draftReader.GetString(draftReader.GetOrdinal("Status")),
            RequisitionIds = draftReader.GetString(draftReader.GetOrdinal("RequisitionIds")),
            IssuedBy = draftReader.IsDBNull(draftReader.GetOrdinal("IssuedBy")) ? null : draftReader.GetString(draftReader.GetOrdinal("IssuedBy")),
            ReceivedBy = draftReader.IsDBNull(draftReader.GetOrdinal("ReceivedBy")) ? null : draftReader.GetString(draftReader.GetOrdinal("ReceivedBy")),
            Notes = draftReader.IsDBNull(draftReader.GetOrdinal("Notes")) ? null : draftReader.GetString(draftReader.GetOrdinal("Notes")),
            CreatedAt = draftReader.GetDateTime(draftReader.GetOrdinal("CreatedAt")),
            FinalizedAt = draftReader.IsDBNull(draftReader.GetOrdinal("FinalizedAt")) ? null : draftReader.GetDateTime(draftReader.GetOrdinal("FinalizedAt")),
            IssuedAt = draftReader.IsDBNull(draftReader.GetOrdinal("IssuedAt")) ? null : draftReader.GetDateTime(draftReader.GetOrdinal("IssuedAt"))
        };
        draftReader.Close();

        // Load bar assignments
        var barSql = "SELECT * FROM Stores_IssueWindowDraftBarAssignments WHERE DraftId = @DraftId ORDER BY SortOrder";
        using var barCmd = new SqlCommand(barSql, connection);
        barCmd.Parameters.AddWithValue("@DraftId", id);
        using var barReader = await barCmd.ExecuteReaderAsync();

        var bars = new List<DraftBarAssignmentResponse>();
        while (await barReader.ReadAsync())
        {
            bars.Add(new DraftBarAssignmentResponse
            {
                Id = barReader.GetInt32(barReader.GetOrdinal("Id")),
                DraftId = barReader.GetInt32(barReader.GetOrdinal("DraftId")),
                MaterialId = barReader.IsDBNull(barReader.GetOrdinal("MaterialId")) ? null : barReader.GetInt32(barReader.GetOrdinal("MaterialId")),
                MaterialName = barReader.IsDBNull(barReader.GetOrdinal("MaterialName")) ? null : barReader.GetString(barReader.GetOrdinal("MaterialName")),
                MaterialCode = barReader.IsDBNull(barReader.GetOrdinal("MaterialCode")) ? null : barReader.GetString(barReader.GetOrdinal("MaterialCode")),
                Grade = barReader.IsDBNull(barReader.GetOrdinal("Grade")) ? null : barReader.GetString(barReader.GetOrdinal("Grade")),
                DiameterMM = barReader.IsDBNull(barReader.GetOrdinal("DiameterMM")) ? null : barReader.GetDecimal(barReader.GetOrdinal("DiameterMM")),
                PieceId = barReader.IsDBNull(barReader.GetOrdinal("PieceId")) ? null : barReader.GetInt32(barReader.GetOrdinal("PieceId")),
                PieceNo = barReader.IsDBNull(barReader.GetOrdinal("PieceNo")) ? null : barReader.GetString(barReader.GetOrdinal("PieceNo")),
                PieceCurrentLengthMM = barReader.IsDBNull(barReader.GetOrdinal("PieceCurrentLengthMM")) ? null : barReader.GetDecimal(barReader.GetOrdinal("PieceCurrentLengthMM")),
                TotalCutMM = barReader.IsDBNull(barReader.GetOrdinal("TotalCutMM")) ? null : barReader.GetDecimal(barReader.GetOrdinal("TotalCutMM")),
                RemainingMM = barReader.IsDBNull(barReader.GetOrdinal("RemainingMM")) ? null : barReader.GetDecimal(barReader.GetOrdinal("RemainingMM")),
                WillBeScrap = barReader.GetBoolean(barReader.GetOrdinal("WillBeScrap")),
                SortOrder = barReader.GetInt32(barReader.GetOrdinal("SortOrder"))
            });
        }
        barReader.Close();

        // Load cuts per bar
        var cutSql = "SELECT * FROM Stores_IssueWindowDraftCuts WHERE DraftId = @DraftId ORDER BY BarAssignmentId, SortOrder";
        using var cutCmd = new SqlCommand(cutSql, connection);
        cutCmd.Parameters.AddWithValue("@DraftId", id);
        using var cutReader = await cutCmd.ExecuteReaderAsync();

        var cutsByBar = new Dictionary<int, List<DraftCutResponse>>();
        while (await cutReader.ReadAsync())
        {
            var barAssignmentId = cutReader.GetInt32(cutReader.GetOrdinal("BarAssignmentId"));
            if (!cutsByBar.ContainsKey(barAssignmentId))
                cutsByBar[barAssignmentId] = new List<DraftCutResponse>();

            cutsByBar[barAssignmentId].Add(new DraftCutResponse
            {
                Id = cutReader.GetInt32(cutReader.GetOrdinal("Id")),
                RequisitionItemId = cutReader.GetInt32(cutReader.GetOrdinal("RequisitionItemId")),
                RequisitionId = cutReader.IsDBNull(cutReader.GetOrdinal("RequisitionId")) ? null : cutReader.GetInt32(cutReader.GetOrdinal("RequisitionId")),
                CutIndex = cutReader.GetInt32(cutReader.GetOrdinal("CutIndex")),
                CutLengthMM = cutReader.GetDecimal(cutReader.GetOrdinal("CutLengthMM")),
                PartName = cutReader.IsDBNull(cutReader.GetOrdinal("PartName")) ? null : cutReader.GetString(cutReader.GetOrdinal("PartName")),
                JobCardNo = cutReader.IsDBNull(cutReader.GetOrdinal("JobCardNo")) ? null : cutReader.GetString(cutReader.GetOrdinal("JobCardNo")),
                RequisitionNo = cutReader.IsDBNull(cutReader.GetOrdinal("RequisitionNo")) ? null : cutReader.GetString(cutReader.GetOrdinal("RequisitionNo")),
                MaterialId = cutReader.IsDBNull(cutReader.GetOrdinal("MaterialId")) ? null : cutReader.GetInt32(cutReader.GetOrdinal("MaterialId")),
                SortOrder = cutReader.GetInt32(cutReader.GetOrdinal("SortOrder"))
            });
        }

        foreach (var bar in bars)
        {
            if (cutsByBar.TryGetValue(bar.Id, out var cuts))
                bar.Cuts = cuts;
        }

        draft.BarAssignments = bars;
        return draft;
    }

    public async Task<bool> FinalizeDraftAsync(int id)
    {
        var sql = @"
            UPDATE Stores_IssueWindowDrafts
            SET Status = 'Finalized', FinalizedAt = GETDATE()
            WHERE Id = @Id AND Status = 'Draft'";
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> MarkIssuedAsync(int draftId, string issuedBy, string receivedBy)
    {
        var sql = @"
            UPDATE Stores_IssueWindowDrafts
            SET Status = 'Issued', IssuedBy = @IssuedBy, ReceivedBy = @ReceivedBy, IssuedAt = GETDATE()
            WHERE Id = @Id AND Status = 'Finalized'";
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", draftId);
        cmd.Parameters.AddWithValue("@IssuedBy", issuedBy);
        cmd.Parameters.AddWithValue("@ReceivedBy", receivedBy);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteDraftAsync(int id)
    {
        using var connection = (SqlConnection)_connectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Release reserved pieces back to Available before deleting
            var releaseSql = @"
                UPDATE Stores_MaterialPieces
                SET Status = 'Available', UpdatedAt = GETUTCDATE()
                WHERE Id IN (
                    SELECT PieceId FROM Stores_IssueWindowDraftBarAssignments
                    WHERE DraftId = @Id AND PieceId IS NOT NULL
                )";
            using var releaseCmd = new SqlCommand(releaseSql, connection, transaction);
            releaseCmd.Parameters.AddWithValue("@Id", id);
            await releaseCmd.ExecuteNonQueryAsync();

            // Only allow deleting drafts that are still in Draft status (not Finalized or Issued)
            var deleteSql = "DELETE FROM Stores_IssueWindowDrafts WHERE Id = @Id AND Status = 'Draft'";
            using var deleteCmd = new SqlCommand(deleteSql, connection, transaction);
            deleteCmd.Parameters.AddWithValue("@Id", id);
            var rows = await deleteCmd.ExecuteNonQueryAsync();

            transaction.Commit();
            return rows > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
