using System;

namespace MultiHitechERP.API.Models.Stores
{
    /// <summary>
    /// A single scrap/wastage sale to a kabadi (scrap dealer). Weight-based manual entry.
    /// </summary>
    public class ScrapSale
    {
        public int Id { get; set; }
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public decimal WeightKG { get; set; }
        public decimal RatePerKG { get; set; }
        public decimal TotalAmount { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
