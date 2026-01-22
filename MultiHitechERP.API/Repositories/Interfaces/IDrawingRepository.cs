using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Drawing master operations
    /// </summary>
    public interface IDrawingRepository
    {
        // Basic CRUD Operations
        Task<Drawing?> GetByIdAsync(int id);
        Task<Drawing?> GetByDrawingNumberAsync(string drawingNumber);
        Task<IEnumerable<Drawing>> GetAllAsync();
        Task<IEnumerable<Drawing>> GetActiveDrawingsAsync();

        // Create, Update, Delete
        Task<int> InsertAsync(Drawing drawing);
        Task<bool> UpdateAsync(Drawing drawing);
        Task<bool> DeleteAsync(int id);

        // Revision Operations
        Task<IEnumerable<Drawing>> GetRevisionHistoryAsync(string drawingNumber);
        Task<Drawing?> GetLatestRevisionAsync(string drawingNumber);
        Task<bool> MarkAsLatestRevisionAsync(int id);

        // Queries
        Task<IEnumerable<Drawing>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Drawing>> GetByDrawingTypeAsync(string drawingType);
        Task<IEnumerable<Drawing>> GetPendingApprovalAsync();
        Task<bool> ExistsAsync(string drawingNumber, string revisionNumber);
    }
}
