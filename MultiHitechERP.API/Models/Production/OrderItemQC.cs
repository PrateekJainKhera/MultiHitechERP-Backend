using System;

namespace MultiHitechERP.API.Models.Production
{
    /// <summary>
    /// Final QC sign-off at the order-item level.
    /// Created after assembly is complete; must be Passed before item can be dispatched.
    /// </summary>
    public class OrderItemQC
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }

        // Pending | Passed | Failed
        public string QCStatus { get; set; } = "Pending";

        // S3 path for the QC certificate PDF (optional)
        public string? CertificatePath { get; set; }

        public DateTime? QCCompletedAt { get; set; }
        public string? QCCompletedBy { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
