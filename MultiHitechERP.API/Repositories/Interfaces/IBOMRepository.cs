using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for BOM (Bill of Materials) operations
    /// </summary>
    public interface IBOMRepository
    {
        // Basic CRUD
        Task<BOM?> GetByIdAsync(Guid id);
        Task<BOM?> GetByBOMNoAsync(string bomNo);
        Task<IEnumerable<BOM>> GetAllAsync();
        Task<Guid> InsertAsync(BOM bom);
        Task<bool> UpdateAsync(BOM bom);
        Task<bool> DeleteAsync(Guid id);

        // Query Methods
        Task<IEnumerable<BOM>> GetByProductIdAsync(Guid productId);
        Task<BOM?> GetLatestRevisionAsync(Guid productId);
        Task<IEnumerable<BOM>> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<BOM>> GetActiveAsync();
        Task<IEnumerable<BOM>> GetByStatusAsync(string status);
        Task<IEnumerable<BOM>> GetByBOMTypeAsync(string bomType);

        // BOM Item Operations
        Task<IEnumerable<BOMItem>> GetBOMItemsAsync(Guid bomId);
        Task<Guid> InsertBOMItemAsync(BOMItem item);
        Task<bool> UpdateBOMItemAsync(BOMItem item);
        Task<bool> DeleteBOMItemAsync(Guid itemId);
        Task<bool> DeleteAllBOMItemsAsync(Guid bomId);

        // Item Type Queries
        Task<IEnumerable<BOMItem>> GetMaterialItemsAsync(Guid bomId);
        Task<IEnumerable<BOMItem>> GetChildPartItemsAsync(Guid bomId);
        Task<IEnumerable<BOMItem>> GetItemsByTypeAsync(Guid bomId, string itemType);

        // BOM Approval
        Task<bool> ApproveBOMAsync(Guid id, string approvedBy, DateTime approvalDate);
        Task<bool> UpdateStatusAsync(Guid id, string status);

        // Revision Management
        Task<bool> MarkAsNonLatestAsync(Guid id);
        Task<int> GetNextRevisionNumberAsync(Guid productId);
    }
}
