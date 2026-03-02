using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Returned for a single QC record (after submit or status check).
    /// </summary>
    public class OrderItemQCResponse
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string QCStatus { get; set; } = string.Empty;
        public string? CertificatePath { get; set; }
        public DateTime? QCCompletedAt { get; set; }
        public string? QCCompletedBy { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// One row in the "Pending QC" list shown in the execution view.
    /// Includes order/product info from JOINs.
    /// </summary>
    public class QCPendingItemResponse
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string ItemSequence { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int Quantity { get; set; }

        // Latest QC record (null if never submitted)
        public int? QCRecordId { get; set; }
        public string? QCStatus { get; set; }    // null = no QC yet
        public string? CertificatePath { get; set; }
        public DateTime? QCCompletedAt { get; set; }
        public string? QCCompletedBy { get; set; }
        public string? Notes { get; set; }
    }
}
