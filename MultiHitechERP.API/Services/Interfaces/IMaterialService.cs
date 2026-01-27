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
        Task<ApiResponse<MaterialResponse>> GetByIdAsync(int id);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetAllAsync();

        Task<ApiResponse<int>> CreateMaterialAsync(CreateMaterialRequest request);
        Task<ApiResponse<bool>> UpdateMaterialAsync(UpdateMaterialRequest request);
        Task<ApiResponse<bool>> DeleteMaterialAsync(int id);

        // Business Queries
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByGradeAsync(string grade);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByShapeAsync(string shape);
        Task<ApiResponse<IEnumerable<MaterialResponse>>> SearchByNameAsync(string searchTerm);
    }
}
