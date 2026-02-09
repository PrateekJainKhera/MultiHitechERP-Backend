using System;

namespace MultiHitechERP.API.Models.Inventory
{
    /// <summary>
    /// Represents real-time inventory stock levels for materials
    /// Aggregates data from MaterialPieces and tracks stock movements
    /// </summary>
    public class Inventory
    {
        public int Id { get; set; }

        // Unified inventory fields - ItemType + ItemId
        public string ItemType { get; set; } = "RawMaterial"; // "RawMaterial", "ChildPart", "FinishedGood"
        public int ItemId { get; set; } // MaterialId or ChildPartId depending on ItemType

        // Legacy field for backwards compatibility
        public int MaterialId { get; set; }

        // Item Reference (denormalized for quick access)
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? MaterialCategory { get; set; }

        // Stock Quantities
        public decimal TotalQuantity { get; set; } = 0;
        public decimal AvailableQuantity { get; set; } = 0;
        public decimal AllocatedQuantity { get; set; } = 0;
        public decimal IssuedQuantity { get; set; } = 0;
        public decimal ReservedQuantity { get; set; } = 0;

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
        public bool IsLowStock { get; set; } = false;
        public bool IsOutOfStock { get; set; } = false;
        public bool IsActive { get; set; } = true;

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
