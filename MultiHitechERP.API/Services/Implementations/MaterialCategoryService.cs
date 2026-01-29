using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class MaterialCategoryService : IMaterialCategoryService
    {
        private readonly IMaterialCategoryRepository _categoryRepository;

        public MaterialCategoryService(IMaterialCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                var response = categories.Select(MapToResponse);
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.SuccessResponse(response, "Material categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse($"Error retrieving material categories: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MaterialCategoryResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<MaterialCategoryResponse>.ErrorResponse("Material category not found");

                return ApiResponse<MaterialCategoryResponse>.SuccessResponse(MapToResponse(category), "Material category retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<MaterialCategoryResponse>.ErrorResponse($"Error retrieving material category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MaterialCategoryResponse>> GetCategoryByCodeAsync(string categoryCode)
        {
            try
            {
                var category = await _categoryRepository.GetByCategoryCodeAsync(categoryCode);
                if (category == null)
                    return ApiResponse<MaterialCategoryResponse>.ErrorResponse("Material category not found");

                return ApiResponse<MaterialCategoryResponse>.SuccessResponse(MapToResponse(category), "Material category retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<MaterialCategoryResponse>.ErrorResponse($"Error retrieving material category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetCategoriesByMaterialTypeAsync(string materialType)
        {
            try
            {
                // Validate material type
                var validTypes = new[] { "raw_material", "component" };
                if (!validTypes.Contains(materialType))
                    return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse($"Invalid material type. Must be one of: {string.Join(", ", validTypes)}");

                var categories = await _categoryRepository.GetByMaterialTypeAsync(materialType);
                var response = categories.Select(MapToResponse);
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.SuccessResponse(response, "Material categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse($"Error retrieving material categories: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetActiveAsync();
                var response = categories.Select(MapToResponse);
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.SuccessResponse(response, "Active material categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse($"Error retrieving active material categories: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialCategoryResponse>>> SearchCategoriesByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse("Search term cannot be empty");

                var categories = await _categoryRepository.SearchByNameAsync(searchTerm);
                var response = categories.Select(MapToResponse);
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.SuccessResponse(response, "Material categories retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialCategoryResponse>>.ErrorResponse($"Error searching material categories: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateCategoryAsync(CreateMaterialCategoryRequest request)
        {
            try
            {
                // Validate material type
                var validTypes = new[] { "raw_material", "component" };
                if (!validTypes.Contains(request.MaterialType))
                    return ApiResponse<int>.ErrorResponse($"Invalid material type. Must be one of: {string.Join(", ", validTypes)}");

                // Auto-generate CategoryCode based on material type
                string codePrefix = request.MaterialType == "raw_material" ? "MAT" : "COMP";
                int nextSequence = await _categoryRepository.GetNextSequenceNumberAsync(request.MaterialType);
                string generatedCode = $"{codePrefix}-{nextSequence:D4}";

                var category = new MaterialCategory
                {
                    CategoryCode = generatedCode,
                    CategoryName = request.CategoryName,
                    Quality = request.Quality,
                    Description = request.Description,
                    DefaultUOM = request.DefaultUOM,
                    MaterialType = request.MaterialType,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var id = await _categoryRepository.CreateAsync(category);
                return ApiResponse<int>.SuccessResponse(id, $"Material category created successfully with code '{generatedCode}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating material category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateCategoryAsync(int id, UpdateMaterialCategoryRequest request)
        {
            try
            {
                // Check if category exists
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                    return ApiResponse<bool>.ErrorResponse("Material category not found");

                // Validate material type
                var validTypes = new[] { "raw_material", "component" };
                if (!validTypes.Contains(request.MaterialType))
                    return ApiResponse<bool>.ErrorResponse($"Invalid material type. Must be one of: {string.Join(", ", validTypes)}");

                // Check for duplicate category code (excluding current category)
                if (await _categoryRepository.CategoryCodeExistsAsync(request.CategoryCode, id))
                    return ApiResponse<bool>.ErrorResponse($"Category code '{request.CategoryCode}' already exists");

                var category = new MaterialCategory
                {
                    Id = id,
                    CategoryCode = request.CategoryCode,
                    CategoryName = request.CategoryName,
                    Quality = request.Quality,
                    Description = request.Description,
                    DefaultUOM = request.DefaultUOM,
                    MaterialType = request.MaterialType,
                    IsActive = request.IsActive,
                    CreatedAt = existingCategory.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _categoryRepository.UpdateAsync(id, category);
                if (result)
                    return ApiResponse<bool>.SuccessResponse(true, "Material category updated successfully");

                return ApiResponse<bool>.ErrorResponse("Failed to update material category");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating material category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<bool>.ErrorResponse("Material category not found");

                var result = await _categoryRepository.DeleteAsync(id);
                if (result)
                    return ApiResponse<bool>.SuccessResponse(true, "Material category deleted successfully");

                return ApiResponse<bool>.ErrorResponse("Failed to delete material category");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting material category: {ex.Message}");
            }
        }

        private MaterialCategoryResponse MapToResponse(MaterialCategory category)
        {
            return new MaterialCategoryResponse
            {
                Id = category.Id,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                Quality = category.Quality,
                Description = category.Description,
                DefaultUOM = category.DefaultUOM,
                MaterialType = category.MaterialType,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
