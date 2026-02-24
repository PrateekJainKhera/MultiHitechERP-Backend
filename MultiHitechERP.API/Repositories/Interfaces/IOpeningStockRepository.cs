using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IOpeningStockRepository
    {
        Task<int> CreateAsync(OpeningStockEntry entry, IEnumerable<OpeningStockItem> items);
        Task<IEnumerable<OpeningStockSummaryResponse>> GetAllAsync();
        Task<OpeningStockDetailResponse?> GetByIdAsync(int id);
        Task<bool> UpdateItemsAsync(int entryId, IEnumerable<OpeningStockItem> items);
        Task<bool> ConfirmAsync(int id, string? confirmedBy, int totalPieces, int totalComponents);
        Task<bool> DeleteAsync(int id);
        Task<string> GenerateEntryNoAsync();
    }
}
