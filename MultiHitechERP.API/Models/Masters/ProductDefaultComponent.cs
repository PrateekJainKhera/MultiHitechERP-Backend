using System;

namespace MultiHitechERP.API.Models.Masters
{
    /// <summary>
    /// Remembered component requirement for a Product (mirrors ProductDefaultMaterial).
    /// When a product is planned, these pre-fill the Components section.
    /// </summary>
    public class ProductDefaultComponent
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public decimal NoOfPieces { get; set; } = 1;
        public string? UOM { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
