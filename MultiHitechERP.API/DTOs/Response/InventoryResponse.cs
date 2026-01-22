using System;

namespace MultiHitechERP.API.DTOs.Response
{
    /// <summary>
    /// Response DTO for Inventory
    /// </summary>
    public class InventoryResponse
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }

        // Material Reference
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialCategory { get; set; }

        // Stock Quantities
        public decimal TotalQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }

        // Unit
        public string UOM { get; set; } = "KG";

        // Stock Levels
        public decimal? MinimumStock { get; set; }
        public decimal? MaximumStock { get; set; }
        public decimal? ReorderLevel { get; set; }
        public decimal? ReorderQuantity { get; set; }

        // Location
        public string? PrimaryStorageLocation { get; set; }
        public string? WarehouseCode { get; set; }

        // Valuation
        public decimal? AverageCostPerUnit { get; set; }
        public decimal? TotalStockValue { get; set; }

        // Status
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public bool IsActive { get; set; }

        // Last Movement
        public DateTime? LastStockInDate { get; set; }
        public DateTime? LastStockOutDate { get; set; }
        public DateTime? LastCountDate { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
