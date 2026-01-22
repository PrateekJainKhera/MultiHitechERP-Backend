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
        Task<ApiResponse<BOM>> GetByIdAsync(int id);
        Task<ApiResponse<BOM>> GetByBOMNoAsync(string bomNo);
        Task<ApiResponse<IEnumerable<BOM>>> GetAllAsync();
        Task<ApiResponse<int>> CreateBOMAsync(BOM bom);
        Task<ApiResponse<bool>> UpdateBOMAsync(BOM bom);
        Task<ApiResponse<bool>> DeleteBOMAsync(int id);

        // Query Methods
        Task<ApiResponse<IEnumerable<BOM>>> GetByProductIdAsync(int productId);
        Task<ApiResponse<BOM>> GetLatestRevisionAsync(int productId);
        Task<ApiResponse<IEnumerable<BOM>>> GetByProductCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<BOM>>> GetActiveAsync();
        Task<ApiResponse<IEnumerable<BOM>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<BOM>>> GetByBOMTypeAsync(string bomType);

        // BOM Item Operations
        Task<ApiResponse<IEnumerable<BOMItem>>> GetBOMItemsAsync(int bomId);
        Task<ApiResponse<int>> AddBOMItemAsync(BOMItem item);
        Task<ApiResponse<bool>> UpdateBOMItemAsync(BOMItem item);
        Task<ApiResponse<bool>> DeleteBOMItemAsync(int itemId);
        Task<ApiResponse<bool>> DeleteAllBOMItemsAsync(int bomId);

        // Item Type Queries
        Task<ApiResponse<IEnumerable<BOMItem>>> GetMaterialItemsAsync(int bomId);
        Task<ApiResponse<IEnumerable<BOMItem>>> GetChildPartItemsAsync(int bomId);
        Task<ApiResponse<IEnumerable<BOMItem>>> GetItemsByTypeAsync(int bomId, string itemType);

        // BOM Approval
        Task<ApiResponse<bool>> ApproveBOMAsync(int id, string approvedBy);
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);

        // BOM Revision
        Task<ApiResponse<int>> CreateRevisionAsync(int bomId, string revisionNumber, string createdBy);
    }
}
