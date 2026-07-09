using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    public class ResubmitGRNRequest
    {
        public string ResubmittedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
        // Optional corrected line values; lines not listed are left unchanged
        public List<ResubmitGRNLine>? Lines { get; set; }
    }

    public class ResubmitGRNLine
    {
        public int LineId { get; set; }
        public int NumberOfPieces { get; set; }
        public decimal TotalWeightKG { get; set; }
    }
}
