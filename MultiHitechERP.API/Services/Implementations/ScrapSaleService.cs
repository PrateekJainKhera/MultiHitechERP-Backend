using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class ScrapSaleService : IScrapSaleService
    {
        private readonly IScrapSaleRepository _repo;

        public ScrapSaleService(IScrapSaleRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<ScrapOverviewResponse>> GetOverviewAsync()
        {
            try
            {
                var wastage = (await _repo.GetWastageByMaterialAsync()).ToList();
                var sold = (await _repo.GetSoldByMaterialAsync()).ToList();
                var sales = (await _repo.GetAllAsync()).ToList();

                // Merge wastage (from pieces) and sold (from ledger) by material so the UI
                // can show remaining scrap available to sell.
                var soldByMaterial = sold
                    .GroupBy(s => s.MaterialId ?? 0)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.SoldWeightKG));

                var materials = wastage.Select(w =>
                {
                    var key = w.MaterialId ?? 0;
                    var soldKg = soldByMaterial.TryGetValue(key, out var s) ? s : 0;
                    return new ScrapMaterialSummaryResponse
                    {
                        MaterialId = w.MaterialId,
                        MaterialCode = w.MaterialCode,
                        MaterialName = w.MaterialName,
                        WastagePieces = w.WastagePieces,
                        WastageWeightKG = w.WastageWeightKG,
                        SoldWeightKG = soldKg,
                        RemainingWeightKG = Math.Max(0, w.WastageWeightKG - soldKg),
                    };
                }).ToList();

                // Materials that have sales but no current wastage pieces still belong in the list.
                foreach (var s in sold)
                {
                    var key = s.MaterialId ?? 0;
                    if (!materials.Any(m => (m.MaterialId ?? 0) == key))
                    {
                        materials.Add(new ScrapMaterialSummaryResponse
                        {
                            MaterialId = s.MaterialId,
                            MaterialCode = s.MaterialCode,
                            MaterialName = s.MaterialName,
                            WastagePieces = 0,
                            WastageWeightKG = 0,
                            SoldWeightKG = s.SoldWeightKG,
                            RemainingWeightKG = 0,
                        });
                    }
                }

                var overview = new ScrapOverviewResponse
                {
                    Materials = materials.OrderByDescending(m => m.RemainingWeightKG).ThenBy(m => m.MaterialName).ToList(),
                    Sales = sales,
                    TotalWastageWeightKG = materials.Sum(m => m.WastageWeightKG),
                    TotalSoldWeightKG = sales.Sum(s => s.WeightKG),
                    TotalSaleAmount = sales.Sum(s => s.TotalAmount),
                };
                return ApiResponse<ScrapOverviewResponse>.SuccessResponse(overview);
            }
            catch (Exception ex)
            {
                return ApiResponse<ScrapOverviewResponse>.ErrorResponse($"Error loading scrap overview: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateScrapSaleRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BuyerName))
                    return ApiResponse<int>.ErrorResponse("Buyer name is required");
                if (request.WeightKG <= 0)
                    return ApiResponse<int>.ErrorResponse("Weight (kg) must be greater than 0");
                if (request.RatePerKG < 0)
                    return ApiResponse<int>.ErrorResponse("Rate cannot be negative");

                var sale = new ScrapSale
                {
                    MaterialId = request.MaterialId,
                    MaterialCode = request.MaterialCode,
                    MaterialName = request.MaterialName,
                    WeightKG = request.WeightKG,
                    RatePerKG = request.RatePerKG,
                    TotalAmount = Math.Round(request.WeightKG * request.RatePerKG, 2),
                    BuyerName = request.BuyerName.Trim(),
                    SaleDate = request.SaleDate ?? DateTime.UtcNow.Date,
                    Remarks = request.Remarks,
                    CreatedBy = request.CreatedBy,
                };

                var id = await _repo.CreateAsync(sale);
                return ApiResponse<int>.SuccessResponse(id, "Scrap sale recorded");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Failed to record scrap sale: {ex.Message}");
            }
        }
    }
}
