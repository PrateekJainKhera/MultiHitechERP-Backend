using System;

namespace MultiHitechERP.API.Models.Stores
{
    public class ComponentIssue
    {
        public int Id { get; set; }
        public string IssueNo { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }

        public int ComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string? PartNumber { get; set; }
        public string Unit { get; set; } = "Pcs";

        public decimal IssuedQty { get; set; }

        public string RequestedBy { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
