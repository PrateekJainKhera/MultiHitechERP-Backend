using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for BOM (Bill of Materials) business logic
    /// </summary>
    public interface IBOMService
    {
        // Basic CRUD
        Task<ApiResponse<BOM>> GetByIdAsync(Guid id);
        Task<ApiResponse<BOM>> GetByBOMNoAsync(string bomNo);
        Task<ApiResponse<IEnumerable<BOM>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateBOMAsync(BOM bom);
        Task<ApiResponse<bool>> UpdateBOMAsync(BOM bom);
        Task<ApiResponse<bool>> DeleteBOMAsync(Guid id);

        // Query Methods
        Task<ApiResponse<IEnumerable<BOM>>> GetByProductIdAsync(Guid productId);
        Task<ApiResponse<BOM>> GetLatestRevisionAsync(Guid productId);
        Task<ApiResponse<IEnumerable<BOM>>> GetByProductCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<BOM>>> GetActiveAsync();
        Task<ApiResponse<IEnumerable<BOM>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<BOM>>> GetByBOMTypeAsync(string bomType);

        // BOM Item Operations
        Task<ApiResponse<IEnumerable<BOMItem>>> GetBOMItemsAsync(Guid bomId);
        Task<ApiResponse<Guid>> AddBOMItemAsync(BOMItem item);
        Task<ApiResponse<bool>> UpdateBOMItemAsync(BOMItem item);
        Task<ApiResponse<bool>> DeleteBOMItemAsync(Guid itemId);
        Task<ApiResponse<bool>> DeleteAllBOMItemsAsync(Guid bomId);

        // Item Type Queries
        Task<ApiResponse<IEnumerable<BOMItem>>> GetMaterialItemsAsync(Guid bomId);
        Task<ApiResponse<IEnumerable<BOMItem>>> GetChildPartItemsAsync(Guid bomId);
        Task<ApiResponse<IEnumerable<BOMItem>>> GetItemsByTypeAsync(Guid bomId, string itemType);

        // BOM Approval
        Task<ApiResponse<bool>> ApproveBOMAsync(Guid id, string approvedBy);
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);

        // BOM Revision
        Task<ApiResponse<Guid>> CreateRevisionAsync(Guid bomId, string revisionNumber, string createdBy);
    }
}
