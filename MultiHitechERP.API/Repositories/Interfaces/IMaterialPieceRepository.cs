using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Stores;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Material Piece operations (length-based tracking)
    /// </summary>
    public interface IMaterialPieceRepository
    {
        // Basic CRUD Operations
        Task<MaterialPiece?> GetByIdAsync(int id);
        Task<MaterialPiece?> GetByPieceNoAsync(string pieceNo);
        Task<IEnumerable<MaterialPiece>> GetAllAsync();
        Task<IEnumerable<MaterialPiece>> GetByMaterialIdAsync(int materialId);
        Task<IEnumerable<MaterialPiece>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<int> InsertAsync(MaterialPiece piece);
        Task<bool> UpdateAsync(MaterialPiece piece);
        Task<bool> DeleteAsync(int id);

        // Allocation Operations
        Task<bool> AllocatePieceAsync(int id, int requisitionId);
        Task<bool> IssuePieceAsync(int id, int jobCardId, DateTime issuedDate, string issuedBy);
        Task<bool> ConsumePieceAsync(int id, decimal consumedLengthMM, decimal consumedWeightKG);
        Task<bool> ReturnPieceAsync(int id);

        // Queries
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesAsync(int materialId);
        Task<IEnumerable<MaterialPiece>> GetAllocatedPiecesAsync(int requisitionId);
        Task<IEnumerable<MaterialPiece>> GetIssuedPiecesAsync(int jobCardId);
        Task<IEnumerable<MaterialPiece>> GetPiecesByLocationAsync(string location);

        // FIFO Selection
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByFIFOAsync(int materialId, decimal requiredLengthMM);
        Task<decimal> GetAvailableQuantityAsync(int materialId);
    }
}
