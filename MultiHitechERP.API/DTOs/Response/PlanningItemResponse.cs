using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class PlanningItemResponse
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string? ItemSequence { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PartCode { get; set; }
        public string? RollerType { get; set; }
        public int? NumberOfTeeth { get; set; }
        public int Quantity { get; set; }
        public DateTime? DueDate { get; set; }
        public string? ItemPriority { get; set; }
        public string? ItemStatus { get; set; }
        public string? PlanningStatus { get; set; }
        public string? DrawingReviewStatus { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderStatus { get; set; }
        public string? OrderPriority { get; set; }
        public int JobCardCount { get; set; }
        public int CompletedJobCardCount { get; set; }
    }

    public class PagedPlanningItemsResponse
    {
        public List<PlanningItemResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)System.Math.Ceiling((double)TotalCount / PageSize) : 0;
    }

    public class PlanningSummaryResponse
    {
        public int TotalOrders { get; set; }
        public int PendingPlanning { get; set; }
        public int Planned { get; set; }
        public int MaterialShortage { get; set; }
    }
}
