namespace MultiHitechERP.API.Models.Sales
{
    public class EstimationItem
    {
        public int Id { get; set; }
        public int EstimationId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? PartCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
    }
}
