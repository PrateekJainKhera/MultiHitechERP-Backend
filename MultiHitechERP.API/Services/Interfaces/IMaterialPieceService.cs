using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    public interface IMaterialPieceService
    {
        Task<MaterialPieceResponse?> GetByIdAsync(int id);
        Task<MaterialPieceResponse?> GetByPieceNoAsync(string pieceNo);
        Task<IEnumerable<MaterialPieceResponse>> GetAllAsync();
        Task<IEnumerable<MaterialPieceResponse>> GetByMaterialIdAsync(int materialId);
        Task<IEnumerable<MaterialPieceResponse>> GetByStatusAsync(string status);
        Task<IEnumerable<MaterialPieceResponse>> GetByGRNIdAsync(int grnId);
        Task<IEnumerable<MaterialPieceResponse>> GetAvailablePiecesAsync();
        Task<IEnumerable<MaterialPieceResponse>> GetWastagePiecesAsync();
        Task<bool> UpdateLengthAsync(int pieceId, decimal newLengthMM, string updatedBy);
        Task<bool> MarkAsWastageAsync(int pieceId, string reason, decimal? scrapValue, string updatedBy);
        
        // Stock availability for planning
        Task<decimal> GetTotalStockByMaterialIdAsync(int materialId);
        Task<decimal> GetAvailableStockByMaterialIdAsync(int materialId);
        Task<bool> CheckMaterialAvailability(int materialId, decimal requiredLengthMM);
    }
}
