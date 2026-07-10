using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// Shop-floor stock for a component (one row per component).
    /// Quantity = physically on the floor; ReservedQty = claimed by planned orders;
    /// Available = Quantity - ReservedQty.
    /// </summary>
    public class ShopFloorComponentStock
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReservedQty { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
