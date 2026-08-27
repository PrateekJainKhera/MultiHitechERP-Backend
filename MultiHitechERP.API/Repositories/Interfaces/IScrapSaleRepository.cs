using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IScrapSaleRepository
    {
        Task<int> CreateAsync(ScrapSale sale);
        Task<IEnumerable<ScrapSaleResponse>> GetAllAsync();
        // Material-wise wastage weight from Stores_MaterialPieces (IsWastage = 1).
        Task<IEnumerable<ScrapMaterialSummaryResponse>> GetWastageByMaterialAsync();
        // Material-wise sold weight from Stores_ScrapSales.
        Task<IEnumerable<ScrapMaterialSummaryResponse>> GetSoldByMaterialAsync();
    }
}
