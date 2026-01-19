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
        Task<MaterialPiece?> GetByIdAsync(Guid id);
        Task<MaterialPiece?> GetByPieceNoAsync(string pieceNo);
        Task<IEnumerable<MaterialPiece>> GetAllAsync();
        Task<IEnumerable<MaterialPiece>> GetByMaterialIdAsync(Guid materialId);
        Task<IEnumerable<MaterialPiece>> GetByStatusAsync(string status);

        // Create, Update, Delete
        Task<Guid> InsertAsync(MaterialPiece piece);
        Task<bool> UpdateAsync(MaterialPiece piece);
        Task<bool> DeleteAsync(Guid id);

        // Allocation Operations
        Task<bool> AllocatePieceAsync(Guid id, Guid requisitionId);
        Task<bool> IssuePieceAsync(Guid id, Guid jobCardId, DateTime issuedDate, string issuedBy);
        Task<bool> ConsumePieceAsync(Guid id, decimal consumedLengthMM, decimal consumedWeightKG);
        Task<bool> ReturnPieceAsync(Guid id);

        // Queries
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesAsync(Guid materialId);
        Task<IEnumerable<MaterialPiece>> GetAllocatedPiecesAsync(Guid requisitionId);
        Task<IEnumerable<MaterialPiece>> GetIssuedPiecesAsync(Guid jobCardId);
        Task<IEnumerable<MaterialPiece>> GetPiecesByLocationAsync(string location);

        // FIFO Selection
        Task<IEnumerable<MaterialPiece>> GetAvailablePiecesByFIFOAsync(Guid materialId, decimal requiredLengthMM);
        Task<decimal> GetAvailableQuantityAsync(Guid materialId);
    }
}
