using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Material business logic
    /// </summary>
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialService(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        public async Task<ApiResponse<MaterialResponse>> GetByIdAsync(int id)
        {
            try
            {
                var material = await _materialRepository.GetByIdAsync(id);
                if (material == null)
                {
                    return ApiResponse<MaterialResponse>.ErrorResponse($"Material with ID {id} not found");
                }

                var response = MapToResponse(material);
                return ApiResponse<MaterialResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<MaterialResponse>.ErrorResponse($"Error retrieving material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<MaterialResponse>> GetByMaterialCodeAsync(string materialCode)
        {
            try
            {
                var material = await _materialRepository.GetByMaterialCodeAsync(materialCode);
                if (material == null)
                {
                    return ApiResponse<MaterialResponse>.ErrorResponse($"Material {materialCode} not found");
                }

                var response = MapToResponse(material);
                return ApiResponse<MaterialResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<MaterialResponse>.ErrorResponse($"Error retrieving material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetAllAsync()
        {
            try
            {
                var materials = await _materialRepository.GetAllAsync();
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetActiveMaterialsAsync()
        {
            try
            {
                var materials = await _materialRepository.GetActiveMaterialsAsync();
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving active materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateMaterialAsync(CreateMaterialRequest request)
        {
            try
            {
                // Business Rule 1: Validate Material Code is unique
                var exists = await _materialRepository.ExistsAsync(request.MaterialCode);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Material code '{request.MaterialCode}' already exists");
                }

                // Business Rule 2: Validate required fields
                if (string.IsNullOrWhiteSpace(request.MaterialName))
                {
                    return ApiResponse<int>.ErrorResponse("Material name is required");
                }

                // Business Rule 3: Validate stock levels
                if (request.ReorderLevel.HasValue && request.MinStockLevel.HasValue)
                {
                    if (request.ReorderLevel < request.MinStockLevel)
                    {
                        return ApiResponse<int>.ErrorResponse("Reorder level cannot be less than minimum stock level");
                    }
                }

                if (request.MaxStockLevel.HasValue && request.MinStockLevel.HasValue)
                {
                    if (request.MaxStockLevel < request.MinStockLevel)
                    {
                        return ApiResponse<int>.ErrorResponse("Maximum stock level cannot be less than minimum stock level");
                    }
                }

                // Business Rule 4: Validate HSN Code format if provided
                if (!string.IsNullOrWhiteSpace(request.HSNCode) && (request.HSNCode.Length < 4 || request.HSNCode.Length > 8))
                {
                    return ApiResponse<int>.ErrorResponse("HSN Code must be between 4 and 8 digits");
                }

                // Create Material
                var material = new Material
                {
                    MaterialCode = request.MaterialCode.Trim().ToUpper(),
                    MaterialName = request.MaterialName.Trim(),
                    Category = request.Category?.Trim(),
                    SubCategory = request.SubCategory?.Trim(),
                    MaterialType = request.MaterialType?.Trim(),
                    Grade = request.Grade?.Trim(),
                    Specification = request.Specification?.Trim(),
                    Description = request.Description?.Trim(),
                    HSNCode = request.HSNCode?.Trim(),
                    StandardLength = request.StandardLength,
                    Diameter = request.Diameter,
                    Thickness = request.Thickness,
                    Width = request.Width,
                    PrimaryUOM = request.PrimaryUOM?.Trim() ?? "KG",
                    SecondaryUOM = request.SecondaryUOM?.Trim(),
                    ConversionFactor = request.ConversionFactor,
                    WeightPerMeter = request.WeightPerMeter,
                    WeightPerPiece = request.WeightPerPiece,
                    Density = request.Density,
                    StandardCost = request.StandardCost,
                    LastPurchasePrice = request.LastPurchasePrice,
                    LastPurchaseDate = request.LastPurchaseDate,
                    MinStockLevel = request.MinStockLevel ?? 0,
                    MaxStockLevel = request.MaxStockLevel,
                    ReorderLevel = request.ReorderLevel,
                    ReorderQuantity = request.ReorderQuantity,
                    LeadTimeDays = request.LeadTimeDays ?? 7,
                    PreferredSupplierId = request.PreferredSupplierId,
                    PreferredSupplierName = request.PreferredSupplierName?.Trim(),
                    StorageLocation = request.StorageLocation?.Trim(),
                    StorageConditions = request.StorageConditions?.Trim(),
                    IsActive = true,
                    Status = "Active",
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var materialId = await _materialRepository.InsertAsync(material);

                return ApiResponse<int>.SuccessResponse(materialId, $"Material '{request.MaterialCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateMaterialAsync(UpdateMaterialRequest request)
        {
            try
            {
                // Get existing material
                var existingMaterial = await _materialRepository.GetByIdAsync(request.Id);
                if (existingMaterial == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Material not found");
                }

                // Business Rule 1: Validate Material Code uniqueness if changed
                if (existingMaterial.MaterialCode != request.MaterialCode)
                {
                    var exists = await _materialRepository.ExistsAsync(request.MaterialCode);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Material code '{request.MaterialCode}' already exists");
                    }
                }

                // Business Rule 2: Validate stock levels
                if (request.ReorderLevel.HasValue && request.MinStockLevel.HasValue)
                {
                    if (request.ReorderLevel < request.MinStockLevel)
                    {
                        return ApiResponse<bool>.ErrorResponse("Reorder level cannot be less than minimum stock level");
                    }
                }

                // Update material
                existingMaterial.MaterialCode = request.MaterialCode.Trim().ToUpper();
                existingMaterial.MaterialName = request.MaterialName.Trim();
                existingMaterial.Category = request.Category?.Trim();
                existingMaterial.SubCategory = request.SubCategory?.Trim();
                existingMaterial.MaterialType = request.MaterialType?.Trim();
                existingMaterial.Grade = request.Grade?.Trim();
                existingMaterial.Specification = request.Specification?.Trim();
                existingMaterial.Description = request.Description?.Trim();
                existingMaterial.HSNCode = request.HSNCode?.Trim();
                existingMaterial.StandardLength = request.StandardLength;
                existingMaterial.Diameter = request.Diameter;
                existingMaterial.Thickness = request.Thickness;
                existingMaterial.Width = request.Width;
                existingMaterial.PrimaryUOM = request.PrimaryUOM?.Trim();
                existingMaterial.SecondaryUOM = request.SecondaryUOM?.Trim();
                existingMaterial.ConversionFactor = request.ConversionFactor;
                existingMaterial.WeightPerMeter = request.WeightPerMeter;
                existingMaterial.WeightPerPiece = request.WeightPerPiece;
                existingMaterial.Density = request.Density;
                existingMaterial.StandardCost = request.StandardCost;
                existingMaterial.LastPurchasePrice = request.LastPurchasePrice;
                existingMaterial.LastPurchaseDate = request.LastPurchaseDate;
                existingMaterial.MinStockLevel = request.MinStockLevel;
                existingMaterial.MaxStockLevel = request.MaxStockLevel;
                existingMaterial.ReorderLevel = request.ReorderLevel;
                existingMaterial.ReorderQuantity = request.ReorderQuantity;
                existingMaterial.LeadTimeDays = request.LeadTimeDays;
                existingMaterial.PreferredSupplierId = request.PreferredSupplierId;
                existingMaterial.PreferredSupplierName = request.PreferredSupplierName?.Trim();
                existingMaterial.StorageLocation = request.StorageLocation?.Trim();
                existingMaterial.StorageConditions = request.StorageConditions?.Trim();
                existingMaterial.IsActive = request.IsActive;
                existingMaterial.Status = request.Status;
                existingMaterial.Remarks = request.Remarks?.Trim();
                existingMaterial.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingMaterial.UpdatedAt = DateTime.UtcNow;

                var success = await _materialRepository.UpdateAsync(existingMaterial);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update material. Please try again.");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Material updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteMaterialAsync(int id)
        {
            try
            {
                var material = await _materialRepository.GetByIdAsync(id);
                if (material == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Material not found");
                }

                // Business Rule: For now, allow deletion. In production, check if material is used in products
                // TODO: Add check for existing products before deletion

                var success = await _materialRepository.DeleteAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete material");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Material deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ActivateMaterialAsync(int id)
        {
            try
            {
                var material = await _materialRepository.GetByIdAsync(id);
                if (material == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Material not found");
                }

                if (material.IsActive)
                {
                    return ApiResponse<bool>.ErrorResponse("Material is already active");
                }

                var success = await _materialRepository.ActivateAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to activate material");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Material activated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error activating material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeactivateMaterialAsync(int id)
        {
            try
            {
                var material = await _materialRepository.GetByIdAsync(id);
                if (material == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Material not found");
                }

                if (!material.IsActive)
                {
                    return ApiResponse<bool>.ErrorResponse("Material is already inactive");
                }

                var success = await _materialRepository.DeactivateAsync(id);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to deactivate material");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Material deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deactivating material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByCategoryAsync(string category)
        {
            try
            {
                var materials = await _materialRepository.GetByCategoryAsync(category);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByGradeAsync(string grade)
        {
            try
            {
                var materials = await _materialRepository.GetByGradeAsync(grade);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByMaterialTypeAsync(string materialType)
        {
            try
            {
                var materials = await _materialRepository.GetByMaterialTypeAsync(materialType);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetLowStockMaterialsAsync()
        {
            try
            {
                var materials = await _materialRepository.GetLowStockMaterialsAsync();
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving low stock materials: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse("Search term is required");
                }

                var materials = await _materialRepository.SearchByNameAsync(searchTerm);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error searching materials: {ex.Message}");
            }
        }

        // Helper Methods

        private static MaterialResponse MapToResponse(Material material)
        {
            return new MaterialResponse
            {
                Id = material.Id,
                MaterialCode = material.MaterialCode,
                MaterialName = material.MaterialName,
                Category = material.Category,
                SubCategory = material.SubCategory,
                MaterialType = material.MaterialType,
                Grade = material.Grade,
                Specification = material.Specification,
                Description = material.Description,
                HSNCode = material.HSNCode,
                StandardLength = material.StandardLength,
                Diameter = material.Diameter,
                Thickness = material.Thickness,
                Width = material.Width,
                PrimaryUOM = material.PrimaryUOM,
                SecondaryUOM = material.SecondaryUOM,
                ConversionFactor = material.ConversionFactor,
                WeightPerMeter = material.WeightPerMeter,
                WeightPerPiece = material.WeightPerPiece,
                Density = material.Density,
                StandardCost = material.StandardCost,
                LastPurchasePrice = material.LastPurchasePrice,
                LastPurchaseDate = material.LastPurchaseDate,
                MinStockLevel = material.MinStockLevel,
                MaxStockLevel = material.MaxStockLevel,
                ReorderLevel = material.ReorderLevel,
                ReorderQuantity = material.ReorderQuantity,
                LeadTimeDays = material.LeadTimeDays,
                PreferredSupplierId = material.PreferredSupplierId,
                PreferredSupplierName = material.PreferredSupplierName,
                StorageLocation = material.StorageLocation,
                StorageConditions = material.StorageConditions,
                IsActive = material.IsActive,
                Status = material.Status,
                Remarks = material.Remarks,
                CreatedAt = material.CreatedAt,
                CreatedBy = material.CreatedBy,
                UpdatedAt = material.UpdatedAt,
                UpdatedBy = material.UpdatedBy
            };
        }
    }
}
