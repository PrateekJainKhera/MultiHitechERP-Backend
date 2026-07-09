using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class PagedOrdersResponse
    {
        public List<OrderResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)System.Math.Ceiling((double)TotalCount / PageSize) : 0;
    }

    public class OrderSummaryResponse
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int InProgress { get; set; }
        public int ReadyToDispatch { get; set; }
        public int Completed { get; set; }
    }
}
