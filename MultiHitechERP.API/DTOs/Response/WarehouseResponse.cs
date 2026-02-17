using System;

namespace MultiHitechERP.API.DTOs.Response
{
    public class WarehouseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Rack { get; set; } = string.Empty;
        public string RackNo { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public int MinStockPieces { get; set; }
        public decimal MinStockLengthMM { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
