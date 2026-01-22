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
        Task<ApiResponse<ChildPart>> GetByIdAsync(Guid id);
        Task<ApiResponse<ChildPart>> GetByCodeAsync(string childPartCode);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetAllAsync();
        Task<ApiResponse<Guid>> CreateChildPartAsync(ChildPart childPart);
        Task<ApiResponse<bool>> UpdateChildPartAsync(ChildPart childPart);
        Task<ApiResponse<bool>> DeleteChildPartAsync(Guid id);

        // Query Methods
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductIdAsync(Guid productId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductCodeAsync(string productCode);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByMaterialIdAsync(Guid materialId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByPartTypeAsync(string partType);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetActiveAsync();
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByStatusAsync(string status);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByMakeOrBuyAsync(string makeOrBuy);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByDrawingIdAsync(Guid drawingId);
        Task<ApiResponse<IEnumerable<ChildPart>>> GetByProcessTemplateIdAsync(Guid processTemplateId);

        // Status Update
        Task<ApiResponse<bool>> UpdateStatusAsync(Guid id, string status);
    }
}
