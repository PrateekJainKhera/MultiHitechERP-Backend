using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IScrapSaleService
    {
        Task<ApiResponse<ScrapOverviewResponse>> GetOverviewAsync();
        Task<ApiResponse<int>> CreateAsync(CreateScrapSaleRequest request);
    }
}
