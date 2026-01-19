using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Represents physical issuance of materials to production
    /// </summary>
    public class MaterialIssue
    {
        public Guid Id { get; set; }
        public string IssueNo { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }

        public Guid RequisitionId { get; set; }
        public string? JobCardNo { get; set; }
        public string? OrderNo { get; set; }

        public string? MaterialName { get; set; }
        public string? MaterialGrade { get; set; }

        public int? TotalPieces { get; set; }
        public decimal? TotalIssuedLengthMM { get; set; }
        public decimal? TotalIssuedWeightKG { get; set; }

        public string Status { get; set; } = "Issued";

        public string? IssuedById { get; set; }
        public string? IssuedByName { get; set; }
        public string? ReceivedById { get; set; }
        public string? ReceivedByName { get; set; }
    }
}
