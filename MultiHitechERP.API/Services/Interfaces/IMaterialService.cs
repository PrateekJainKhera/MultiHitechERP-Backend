using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;

namespace MultiHitechERP.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Material business logic
    /// </summary>
    public interface IMaterialService
    {
        // CRUD Operations
        Task<ApiResponse<MaterialResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<MaterialResponse>> GetByMaterialCodeAsync(string materialCode);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetActiveMaterialsAsync();

        Task<ApiResponse<Guid>> CreateMaterialAsync(CreateMaterialRequest request);
        Task<ApiResponse<bool>> UpdateMaterialAsync(UpdateMaterialRequest request);
        Task<ApiResponse<bool>> DeleteMaterialAsync(Guid id);

        // Status Operations
        Task<ApiResponse<bool>> ActivateMaterialAsync(Guid id);
        Task<ApiResponse<bool>> DeactivateMaterialAsync(Guid id);

        // Business Queries
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByCategoryAsync(string category);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByGradeAsync(string grade);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByMaterialTypeAsync(string materialType);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetLowStockMaterialsAsync();
        Task<ApiResponse<IEnumerable<MaterialResponse>>> SearchByNameAsync(string searchTerm);
    }
}
