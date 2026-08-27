using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Operator action on a job card in production.
    /// Action values: start | pause | resume | complete | direct-complete
    /// </summary>
    public class ProductionActionRequest
    {
        public string Action { get; set; } = string.Empty;   // start | pause | resume | complete | direct-complete

        [Range(0, int.MaxValue, ErrorMessage = "Completed quantity cannot be negative")]
        public int CompletedQty { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Rejected quantity cannot be negative")]
        public int RejectedQty { get; set; }

        public string? Notes { get; set; }
        public string? OperatorName { get; set; }
    }
}
