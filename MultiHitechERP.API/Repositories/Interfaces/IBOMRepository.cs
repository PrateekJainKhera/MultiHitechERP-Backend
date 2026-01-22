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
        Task<BOM?> GetByIdAsync(int id);
        Task<BOM?> GetByBOMNoAsync(string bomNo);
        Task<IEnumerable<BOM>> GetAllAsync();
        Task<int> InsertAsync(BOM bom);
        Task<bool> UpdateAsync(BOM bom);
        Task<bool> DeleteAsync(int id);

        // Query Methods
        Task<IEnumerable<BOM>> GetByProductIdAsync(int productId);
        Task<BOM?> GetLatestRevisionAsync(int productId);
        Task<IEnumerable<BOM>> GetByProductCodeAsync(string productCode);
        Task<IEnumerable<BOM>> GetActiveAsync();
        Task<IEnumerable<BOM>> GetByStatusAsync(string status);
        Task<IEnumerable<BOM>> GetByBOMTypeAsync(string bomType);

        // BOM Item Operations
        Task<IEnumerable<BOMItem>> GetBOMItemsAsync(int bomId);
        Task<int> InsertBOMItemAsync(BOMItem item);
        Task<bool> UpdateBOMItemAsync(BOMItem item);
        Task<bool> DeleteBOMItemAsync(int itemId);
        Task<bool> DeleteAllBOMItemsAsync(int bomId);

        // Item Type Queries
        Task<IEnumerable<BOMItem>> GetMaterialItemsAsync(int bomId);
        Task<IEnumerable<BOMItem>> GetChildPartItemsAsync(int bomId);
        Task<IEnumerable<BOMItem>> GetItemsByTypeAsync(int bomId, string itemType);

        // BOM Approval
        Task<bool> ApproveBOMAsync(int id, string approvedBy, DateTime approvalDate);
        Task<bool> UpdateStatusAsync(int id, string status);

        // Revision Management
        Task<bool> MarkAsNonLatestAsync(int id);
        Task<int> GetNextRevisionNumberAsync(int productId);
    }
}
