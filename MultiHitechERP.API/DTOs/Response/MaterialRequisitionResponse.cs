using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for material requisition data
    /// </summary>
    public class MaterialRequisitionResponse
    {
        public int Id { get; set; }
        public string RequisitionNo { get; set; } = string.Empty;
        public DateTime RequisitionDate { get; set; }

        public int? JobCardId { get; set; }
        public string? JobCardNo { get; set; }
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; }
        public string? CustomerName { get; set; }

        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        public string? RequestedBy { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
