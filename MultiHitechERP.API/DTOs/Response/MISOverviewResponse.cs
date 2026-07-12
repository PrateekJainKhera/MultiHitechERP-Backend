using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Real, countable MIS aggregates. Deliberately excludes revenue/utilization —
    /// those need data (order values, machine run-times) that is not captured yet.
    /// </summary>
    public class MISOverviewResponse
    {
        public List<MonthCount> OrdersPerMonth { get; set; } = new();
        public List<StatusCount> OrderStatusCounts { get; set; } = new();
        public List<StatusCount> OrderSourceCounts { get; set; } = new();
        public List<TopCustomerRow> TopCustomers { get; set; } = new();
        public List<MonthCount> ChallansPerMonth { get; set; } = new();
        public List<StatusCount> RollerTypeCounts { get; set; } = new();

        public int TotalOrders { get; set; }
        public int TotalChallans { get; set; }
        public int TotalDispatchedQty { get; set; }
        public int JobCardsTotal { get; set; }
        public int JobCardsCompletedSteps { get; set; }
        public int TotalRejectedQty { get; set; }
        public int RejectionJobCards { get; set; }
        public int ReworkJobCards { get; set; }
    }

    public class MonthCount
    {
        public string Month { get; set; } = string.Empty; // yyyy-MM
        public int Count { get; set; }
        public int Qty { get; set; }
    }

    public class StatusCount
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopCustomerRow
    {
        public string CustomerName { get; set; } = string.Empty;
        public int Orders { get; set; }
        public int Qty { get; set; }
    }
}
