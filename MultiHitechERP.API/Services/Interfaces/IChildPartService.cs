using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for ChildPart business logic
    /// </summary>
    public interface IChildPartService
    {
        // Basic CRUD
        Task<ApiResponse<ChildPart>> GetByIdAsync(int id);
        Task<ApiResponse<ChildPart>> GetByCodeAsync(string childPartCode);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetAllAsync();
        Task<ApiResponse<int>> CreateChildPartAsync(ChildPart childPart);
        Task<ApiResponse<bool>> UpdateChildPartAsync(ChildPart childPart);
        Task<ApiResponse<bool>> DeleteChildPartAsync(int id);

        // Query Methods
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductIdAsync(int productId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByMaterialIdAsync(int materialId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByPartTypeAsync(string partType);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetActiveAsync();
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByMakeOrBuyAsync(string makeOrBuy);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByDrawingIdAsync(int drawingId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProcessTemplateIdAsync(int processTemplateId);

        // Status Update
        Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status);
    }
}
