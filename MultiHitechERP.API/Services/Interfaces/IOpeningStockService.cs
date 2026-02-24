using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IOpeningStockService
    {
        Task<OpeningStockDetailResponse> CreateAsync(CreateOpeningStockRequest request);
        Task<IEnumerable<OpeningStockSummaryResponse>> GetAllAsync();
        Task<OpeningStockDetailResponse?> GetByIdAsync(int id);
        Task<OpeningStockDetailResponse> UpdateAsync(int id, CreateOpeningStockRequest request);
        Task<OpeningStockDetailResponse> ConfirmAsync(int id, ConfirmOpeningStockRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
