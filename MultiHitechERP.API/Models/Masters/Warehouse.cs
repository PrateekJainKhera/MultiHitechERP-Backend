using System;

namespace MultiHitechERP.API.Models.Masters
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Rack { get; set; } = string.Empty;
        public string RackNo { get; set; } = string.Empty;
        public string MaterialType { get; set; } = "RawMaterial"; // "RawMaterial" or "Component"
        public int MinStockPieces { get; set; } = 0;
        public decimal MinStockLengthMM { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
