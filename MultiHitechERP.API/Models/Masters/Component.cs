namespace MultiHitechERP.API.Models.Masters
{
    public class Component
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? SupplierName { get; set; }
        public string? Specifications { get; set; }
        public decimal UnitCost { get; set; }
        public int LeadTimeDays { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
