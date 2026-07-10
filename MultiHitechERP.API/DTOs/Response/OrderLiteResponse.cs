using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Minimal order-ITEM (sub-order) info for pickers/dropdowns — loaded in a single fast query.
    /// One row per order item (ORD-…-A, ORD-…-B), with product spec.
    /// </summary>
    public class OrderLiteResponse
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public string? ItemSequence { get; set; }
        public string OrderNo { get; set; } = string.Empty; // base order no (without -A/-B)
        public string? CustomerName { get; set; }
        public string? MachineModel { get; set; }
        public string? RollerType { get; set; }
        public int? NumberOfTeeth { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
