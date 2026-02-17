namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateComponentIssueRequest
    {
        public int ComponentId { get; set; }
        public decimal IssuedQty { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string IssuedBy { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
    }
}
