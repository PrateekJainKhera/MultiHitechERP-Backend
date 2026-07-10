using System;

namespace MultiHitechERP.API.DTOs.Request
{
    /// <summary>
    /// Per-column filters for the paged Orders list. All provided fields are combined with AND.
    /// </summary>
    public class OrderListFilter
    {
        public string? OrderNo { get; set; }
        public string? Customer { get; set; }
        // Free text matched across PartCode + Model + Roller + Teeth ("110" matches "110T")
        public string? Product { get; set; }
        public string? Source { get; set; }
        public DateTime? OrderDateFrom { get; set; }
        public DateTime? OrderDateTo { get; set; }

        public bool HasAny =>
            !string.IsNullOrWhiteSpace(OrderNo) ||
            !string.IsNullOrWhiteSpace(Customer) ||
            !string.IsNullOrWhiteSpace(Product) ||
            !string.IsNullOrWhiteSpace(Source) ||
            OrderDateFrom.HasValue ||
            OrderDateTo.HasValue;
    }
}
