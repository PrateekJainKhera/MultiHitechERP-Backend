using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ComponentIssueResponse
    {
        public int Id { get; set; }
        public string IssueNo { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;

        public int ComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string? PartNumber { get; set; }
        public string Unit { get; set; } = "Pcs";

        public decimal IssuedQty { get; set; }

        public string RequestedBy { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;

        public string? Remarks { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
