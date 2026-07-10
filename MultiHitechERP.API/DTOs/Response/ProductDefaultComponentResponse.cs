namespace MultiHitechERP.API.DTOs.Response
{
    public class ProductDefaultComponentResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public decimal NoOfPieces { get; set; }
        public string? UOM { get; set; }
        public string? Notes { get; set; }
    }
}
