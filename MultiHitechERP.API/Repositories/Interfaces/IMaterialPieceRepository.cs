using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    public interface IMaterialPieceRepository
    {
        Task<int> CreateAsync(MaterialPiece piece);
        Task<MaterialPiece?> GetByIdAsync(int id);
        Task<MaterialPiece?> GetByPieceNoAsync(string pieceNo);
        Task<IEnumerable<MaterialPiece>> GetAllAsync();
        Task<IEnumerable<MaterialPiece>> GetByMaterialIdAsync(int materialId);
        Task<IEnumerable<MaterialPiece>> GetByStatusAsync(string status);
        Task<IEnumerable<MaterialPiece>> GetByGRNIdAsync(int grnId);
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesAsync();
        Task<IEnumerable<MaterialPiece>> GetWastagePiecesAsync();
        Task<bool> UpdateAsync(MaterialPiece piece);
        Task<bool> UpdateLengthAsync(int pieceId, decimal newLengthMM);
        Task<bool> MarkAsWastageAsync(int pieceId, string reason, decimal? scrapValue);
        Task<bool> DeleteAsync(int id);

        // Stock queries
        Task<decimal> GetTotalStockByMaterialIdAsync(int materialId);
        Task<decimal> GetAvailableStockByMaterialIdAsync(int materialId);

        // Requisition/Issue methods (for MaterialRequisitionService compatibility)
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByFIFOAsync(int materialId, decimal requiredQuantityMM);
        Task<IEnumerable<MaterialPiece>> GetAllocatedPiecesAsync(int requisitionId);
        Task<bool> AllocatePieceAsync(int pieceId, int requisitionId);
        Task<bool> ReturnPieceAsync(int pieceId);
        Task<bool> IssuePieceAsync(int pieceId, int jobCardId, System.DateTime issuedDate, string issuedBy);

        // Production completion: mark all pieces issued to a job card as Consumed
        Task<int> ConsumePiecesByJobCardAsync(int jobCardId);

        // Partial cutting: cut specified length from a piece
        Task<bool> CutPieceAsync(
            int pieceId,
            decimal lengthToCutMM,
            int jobCardId,
            string cutByOperator,
            string? orderNo = null,
            string? childPartName = null,
            int minimumUsableLengthMM = 300
        );
    }
}
