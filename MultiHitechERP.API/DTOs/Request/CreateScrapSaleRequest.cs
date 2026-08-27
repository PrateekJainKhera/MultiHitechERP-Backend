using System;

namespace MultiHitechERP.API.DTOs.Request
{
    public class CreateScrapSaleRequest
    {
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public decimal WeightKG { get; set; }
        public decimal RatePerKG { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public DateTime? SaleDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
    }
}
