namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Operator action on a job card in production.
    /// Action values: start | pause | resume | complete
    /// </summary>
    public class ProductionActionRequest
    {
        public string Action { get; set; } = string.Empty;   // start | pause | resume | complete
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }
        public string? Notes { get; set; }
        public string? OperatorName { get; set; }
    }
}
