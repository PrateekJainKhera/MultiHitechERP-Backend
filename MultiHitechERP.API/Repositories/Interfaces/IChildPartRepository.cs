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
        Task<ChildPart?> GetByIdAsync(Guid id);
        Task<ChildPart?> GetByCodeAsync(string childPartCode);
        Task<IEnumerable<ChildPart>> GetAllAsync();
        Task<Guid> InsertAsync(ChildPart childPart);
        Task<bool> UpdateAsync(ChildPart childPart);
        Task<bool> DeleteAsync(Guid id);

        // Query Methods
        Task<IEnumerable<ChildPart>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<ChildPart>> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<ChildPart>> GetByMaterialIdAsync(Guid materialId);
        Task<IEnumerable<ChildPart>> GetByPartTypeAsync(string partType);
        Task<IEnumerable<ChildPart>> GetByCategoryAsync(string category);
        Task<IEnumerable<ChildPart>> GetActiveAsync();
        Task<IEnumerable<ChildPart>> GetByStatusAsync(string status);
        Task<IEnumerable<ChildPart>> GetByMakeOrBuyAsync(string makeOrBuy);
        Task<IEnumerable<ChildPart>> GetByDrawingIdAsync(Guid drawingId);
        Task<IEnumerable<ChildPart>> GetByProcessTemplateIdAsync(Guid processTemplateId);

        // Status Update
        Task<bool> UpdateStatusAsync(Guid id, string status);
    }
}
