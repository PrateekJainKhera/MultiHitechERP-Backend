namespace MultiHitechERP.API.Models.Dispatch
{
    public class ReadyToDispatchItem
    {
        public int OrderItemId { get; set; }
        public string ItemSequence { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PartCode { get; set; }
        public int Quantity { get; set; }
        public int QtyCompleted { get; set; }
        public int QtyDispatched { get; set; }
        public int QtyPendingDispatch { get; set; }
        public DateTime DueDate { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }
}
