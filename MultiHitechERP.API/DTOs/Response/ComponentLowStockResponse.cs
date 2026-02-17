namespace MultiHitechERP.API.DTOs.Response
{
    public class ComponentLowStockResponse
    {
        public int     Id             { get; set; }
        public string  PartNumber     { get; set; } = string.Empty;
        public string  ComponentName  { get; set; } = string.Empty;
        public string  Category       { get; set; } = string.Empty;
        public string  Unit           { get; set; } = string.Empty;
        public string? SupplierName   { get; set; }
        public decimal AvailableStock { get; set; }
        public decimal MinimumStock   { get; set; }
        public decimal Shortage       { get; set; }  // MinimumStock - AvailableStock
    }
}
