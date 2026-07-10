using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Request
{
    public class SaveProductDefaultComponentsRequest
    {
        public List<ProductDefaultComponentItem> Components { get; set; } = new();
        public string? UpdatedBy { get; set; }
    }

    public class ProductDefaultComponentItem
    {
        public int ComponentId { get; set; }
        public string? ComponentName { get; set; }
        public string? PartNumber { get; set; }
        public decimal NoOfPieces { get; set; } = 1;
        public string? UOM { get; set; }
        public string? Notes { get; set; }
    }
}
