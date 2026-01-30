namespace MultiHitechERP.API.DTOs.Response
{
    public class ComponentResponse
    {
        public int Id { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string ComponentName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Manufacturer { get; set; }
        public string? SupplierName { get; set; }
        public string? Specifications { get; set; }
        public int LeadTimeDays { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
