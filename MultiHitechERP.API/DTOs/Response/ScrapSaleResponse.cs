using System;
using System.Collections.Generic;

namespace MultiHitechERP.API.DTOs.Response
{
    public class ScrapSaleResponse
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

    /// <summary>
    /// Material-wise scrap position: how much wastage exists (from pieces) vs how much
    /// has already been sold, so the UI can show what's still available to sell.
    /// </summary>
    public class ScrapMaterialSummaryResponse
    {
        public int? MaterialId { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public int WastagePieces { get; set; }
        public decimal WastageWeightKG { get; set; }
        public decimal SoldWeightKG { get; set; }
        public decimal RemainingWeightKG { get; set; }
    }

    public class ScrapOverviewResponse
    {
        public List<ScrapMaterialSummaryResponse> Materials { get; set; } = new();
        public List<ScrapSaleResponse> Sales { get; set; } = new();
        public decimal TotalWastageWeightKG { get; set; }
        public decimal TotalSoldWeightKG { get; set; }
        public decimal TotalSaleAmount { get; set; }
    }
}
