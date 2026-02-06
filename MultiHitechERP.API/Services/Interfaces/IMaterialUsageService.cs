using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMaterialUsageService
    {
        Task<int> RecordUsageAsync(
            int pieceId,
            decimal lengthUsedMM,
            int? orderId,
            int? jobCardId,
            string? cutByOperator,
            string? machineUsed,
            string? notes,
            string createdBy
        );
        
        Task<MaterialUsageHistoryResponse?> GetByIdAsync(int id);
        Task<IEnumerable<MaterialUsageHistoryResponse>> GetByPieceIdAsync(int pieceId);
        Task<IEnumerable<MaterialUsageHistoryResponse>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<MaterialUsageHistoryResponse>> GetByJobCardIdAsync(int jobCardId);
        Task<IEnumerable<MaterialUsageHistoryResponse>> GetAllAsync();
    }
}
