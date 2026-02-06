using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMaterialUsageHistoryRepository
    {
        Task<int> CreateAsync(MaterialUsageHistory usage);
        Task<MaterialUsageHistory?> GetByIdAsync(int id);
        Task<IEnumerable<MaterialUsageHistory>> GetAllAsync();
        Task<IEnumerable<MaterialUsageHistory>> GetByPieceIdAsync(int pieceId);
        Task<IEnumerable<MaterialUsageHistory>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<MaterialUsageHistory>> GetByJobCardIdAsync(int jobCardId);
        Task<bool> DeleteAsync(int id);
    }
}
