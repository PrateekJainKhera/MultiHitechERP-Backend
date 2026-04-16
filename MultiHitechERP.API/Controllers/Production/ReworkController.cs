using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Data;
using Dapper;

namespace MultiHitechERP.API.Controllers.Production
{
    [ApiController]
    [Route("api/rework")]
    public class ReworkController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;

        public ReworkController(IDbConnectionFactory db)
        {
            _db = db;
        }

        /// <summary>GET /api/rework/pending — rework job cards awaiting admin approval</summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                using var conn = _db.CreateConnection();
                var rows = await conn.QueryAsync<PendingReworkDto>(@"
                    SELECT
                        jc.Id            AS JobCardId,
                        jc.JobCardNo,
                        jc.OrderId,
                        jc.OrderNo,
                        jc.OrderItemId,
                        jc.ItemSequence,
                        jc.ChildPartName,
                        jc.ProcessName,
                        jc.Quantity,
                        jc.RejectedQty,
                        jc.CreatedAt     AS ReworkCreatedAt,
                        jc.CreatedBy     AS ReworkCreatedBy,
                        mr.Id            AS RequisitionId,
                        mr.RequisitionNo,
                        mr.CreatedAt     AS MrCreatedAt,
                        (SELECT COUNT(*) FROM Stores_MaterialRequisitionItems ri WHERE ri.RequisitionId = mr.Id)
                                         AS ItemCount
                    FROM Planning_JobCards jc
                    INNER JOIN Stores_MaterialRequisitions mr
                        ON mr.JobCardId = jc.Id AND mr.Status = 'Pending'
                    WHERE ISNULL(jc.IsRework, 0) = 1
                    ORDER BY mr.CreatedAt ASC");

                return Ok(rows);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>POST /api/rework/{jobCardId}/approve — approve rework MR so it shows in Cutting Planning</summary>
        [HttpPost("{jobCardId}/approve")]
        public async Task<IActionResult> Approve(int jobCardId, [FromBody] ApproveReworkRequest req)
        {
            try
            {
                using var conn = _db.CreateConnection();

                var updated = await conn.ExecuteAsync(@"
                    UPDATE Stores_MaterialRequisitions
                    SET Status      = 'Approved',
                        ApprovedBy  = @ApprovedBy,
                        ApprovalDate = GETUTCDATE()
                    WHERE JobCardId = @JobCardId AND Status = 'Pending'",
                    new { JobCardId = jobCardId, ApprovedBy = req.ApprovedBy ?? "Admin" });

                if (updated == 0)
                    return NotFound(new { error = "No pending rework MR found for this job card." });

                return Ok(new { message = "Rework approved. Material requisition is now visible in Cutting Planning." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>POST /api/rework/{jobCardId}/reject — reject rework request</summary>
        [HttpPost("{jobCardId}/reject")]
        public async Task<IActionResult> Reject(int jobCardId, [FromBody] ApproveReworkRequest req)
        {
            try
            {
                using var conn = _db.CreateConnection();

                var updated = await conn.ExecuteAsync(@"
                    UPDATE Stores_MaterialRequisitions
                    SET Status = 'Rejected',
                        ApprovedBy = @ApprovedBy,
                        ApprovalDate = GETUTCDATE()
                    WHERE JobCardId = @JobCardId AND Status = 'Pending'",
                    new { JobCardId = jobCardId, ApprovedBy = req.ApprovedBy ?? "Admin" });

                if (updated == 0)
                    return NotFound(new { error = "No pending rework MR found for this job card." });

                return Ok(new { message = "Rework rejected." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ApproveReworkRequest
    {
        public string? ApprovedBy { get; set; }
    }

    public class PendingReworkDto
    {
        public int JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public string? OrderNo { get; set; }
        public int? OrderItemId { get; set; }
        public string? ItemSequence { get; set; }
        public string? ChildPartName { get; set; }
        public string? ProcessName { get; set; }
        public int Quantity { get; set; }
        public int RejectedQty { get; set; }
        public DateTime ReworkCreatedAt { get; set; }
        public string? ReworkCreatedBy { get; set; }
        public int RequisitionId { get; set; }
        public string RequisitionNo { get; set; } = string.Empty;
        public DateTime MrCreatedAt { get; set; }
        public int ItemCount { get; set; }
    }
}
