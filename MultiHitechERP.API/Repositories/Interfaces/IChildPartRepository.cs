using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for ChildPart operations
    /// </summary>
    public interface IChildPartRepository
    {
        // Basic CRUD
        Task<ChildPart?> GetByIdAsync(int id);
        Task<ChildPart?> GetByCodeAsync(string childPartCode);
        Task<IEnumerable<ChildPart>> GetAllAsync();
        Task<int> InsertAsync(ChildPart childPart);
        Task<bool> UpdateAsync(ChildPart childPart);
        Task<bool> DeleteAsync(int id);

        // Query Methods
        Task<IEnumerable<ChildPart>> GetByProductIdAsync(int productId);
        Task<IEnumerable<ChildPart>> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<ChildPart>> GetByMaterialIdAsync(int materialId);
        Task<IEnumerable<ChildPart>> GetByPartTypeAsync(string partType);
        Task<IEnumerable<ChildPart>> GetByCategoryAsync(string category);
        Task<IEnumerable<ChildPart>> GetActiveAsync();
        Task<IEnumerable<ChildPart>> GetByStatusAsync(string status);
        Task<IEnumerable<ChildPart>> GetByMakeOrBuyAsync(string makeOrBuy);
        Task<IEnumerable<ChildPart>> GetByDrawingIdAsync(int drawingId);
        Task<IEnumerable<ChildPart>> GetByProcessTemplateIdAsync(int processTemplateId);

        // Status Update
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}
