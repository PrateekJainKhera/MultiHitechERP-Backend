using System.ComponentModel.DataAnnotations;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateWarehouseRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Rack { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RackNo { get; set; } = string.Empty;

        [Required]
        public string MaterialType { get; set; } = "RawMaterial"; // "RawMaterial" or "Component"

        [Range(0, int.MaxValue)]
        public int MinStockPieces { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal MinStockLengthMM { get; set; } = 0;

        public string? CreatedBy { get; set; }
    }

    public class UpdateWarehouseRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Rack { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RackNo { get; set; } = string.Empty;

        [Required]
        public string MaterialType { get; set; } = "RawMaterial";

        [Range(0, int.MaxValue)]
        public int MinStockPieces { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal MinStockLengthMM { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public string? UpdatedBy { get; set; }
    }
}
