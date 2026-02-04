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
                    return ApiResponse<MaterialResponse>.ErrorResponse($"Material with ID {id} not found");

                return ApiResponse<MaterialResponse>.SuccessResponse(MapToResponse(material));
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

        public async Task<ApiResponse<int>> CreateMaterialAsync(CreateMaterialRequest request)
        {
            try
            {
                // Business validation: Check if material name already exists
                var exists = await _materialRepository.ExistsByNameAsync(request.MaterialName);
                if (exists)
                    return ApiResponse<int>.ErrorResponse($"Material '{request.MaterialName}' already exists");

                // Business validation: Validate MaterialType
                var validTypes = new[] { "Steel", "Stainless Steel", "Aluminum", "Other" };
                if (!validTypes.Contains(request.MaterialType))
                    return ApiResponse<int>.ErrorResponse($"Invalid material type. Must be one of: {string.Join(", ", validTypes)}");

                // Business validation: Validate Grade
                var validGrades = new[] { "EN8", "EN19", "SS304", "SS316", "Alloy Steel" };
                if (!validGrades.Contains(request.Grade))
                    return ApiResponse<int>.ErrorResponse($"Invalid grade. Must be one of: {string.Join(", ", validGrades)}");

                // Business validation: Validate Shape
                var validShapes = new[] { "Rod", "Pipe", "Forged", "Sheet" };
                if (!validShapes.Contains(request.Shape))
                    return ApiResponse<int>.ErrorResponse($"Invalid shape. Must be one of: {string.Join(", ", validShapes)}");

                // Shape-based dimension validation
                if (request.Shape == "Rod" || request.Shape == "Forged")
                {
                    if (request.Diameter <= 0)
                        return ApiResponse<int>.ErrorResponse("Diameter is required for Rod/Forged and must be greater than 0");
                }
                else if (request.Shape == "Pipe")
                {
                    if (request.Diameter <= 0)
                        return ApiResponse<int>.ErrorResponse("Outer diameter is required for Pipe and must be greater than 0");
                    if (!request.InnerDiameter.HasValue || request.InnerDiameter.Value <= 0)
                        return ApiResponse<int>.ErrorResponse("Inner diameter is required for Pipe and must be greater than 0");
                    if (request.InnerDiameter.Value >= request.Diameter)
                        return ApiResponse<int>.ErrorResponse("Inner diameter must be less than outer diameter");
                }
                else if (request.Shape == "Sheet")
                {
                    if (!request.Width.HasValue || request.Width.Value <= 0)
                        return ApiResponse<int>.ErrorResponse("Width is required for Sheet and must be greater than 0");
                }

                // Auto-generate MaterialCode
                // For Sheet: use Width as the dimension in code; for others use Diameter
                decimal codeDimension = request.Shape == "Sheet" ? (request.Width ?? 0) : request.Diameter;
                string grade = request.Grade.Replace(" ", "");
                string shape = request.Shape.ToUpper().Substring(0, 3);
                string dimensionStr = ((int)codeDimension).ToString("D3");
                int sequence = await _materialRepository.GetNextSequenceNumberAsync(request.Grade, request.Shape, codeDimension);
                string materialCode = $"{grade}-{shape}-{dimensionStr}-{sequence:D3}";

                // Create material entity
                var material = new Material
                {
                    MaterialCode = materialCode,
                    MaterialName = request.MaterialName.Trim(),
                    MaterialType = request.MaterialType.Trim(),
                    Grade = request.Grade.Trim(),
                    Shape = request.Shape.Trim(),
                    Diameter = request.Shape == "Sheet" ? 0 : request.Diameter,
                    InnerDiameter = request.InnerDiameter,
                    Width = request.Width,
                    LengthInMM = request.LengthInMM,
                    Density = request.Density,
                    WeightKG = request.WeightKG,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var materialId = await _materialRepository.InsertAsync(material);
                return ApiResponse<int>.SuccessResponse(materialId, $"Material '{materialCode}' created successfully");
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
                // Verify material exists
                var existingMaterial = await _materialRepository.GetByIdAsync(request.Id);
                if (existingMaterial == null)
                    return ApiResponse<bool>.ErrorResponse($"Material with ID {request.Id} not found");

                // Business validation: Check if material name changed and if new name already exists
                if (existingMaterial.MaterialName != request.MaterialName)
                {
                    var exists = await _materialRepository.ExistsByNameAsync(request.MaterialName);
                    if (exists)
                        return ApiResponse<bool>.ErrorResponse($"Material '{request.MaterialName}' already exists");
                }

                // Business validation: Validate MaterialType
                var validTypes = new[] { "Steel", "Stainless Steel", "Aluminum", "Other" };
                if (!validTypes.Contains(request.MaterialType))
                    return ApiResponse<bool>.ErrorResponse($"Invalid material type. Must be one of: {string.Join(", ", validTypes)}");

                // Business validation: Validate Grade
                var validGrades = new[] { "EN8", "EN19", "SS304", "SS316", "Alloy Steel" };
                if (!validGrades.Contains(request.Grade))
                    return ApiResponse<bool>.ErrorResponse($"Invalid grade. Must be one of: {string.Join(", ", validGrades)}");

                // Business validation: Validate Shape
                var validShapes = new[] { "Rod", "Pipe", "Forged", "Sheet" };
                if (!validShapes.Contains(request.Shape))
                    return ApiResponse<bool>.ErrorResponse($"Invalid shape. Must be one of: {string.Join(", ", validShapes)}");

                // Shape-based dimension validation
                if (request.Shape == "Rod" || request.Shape == "Forged")
                {
                    if (request.Diameter <= 0)
                        return ApiResponse<bool>.ErrorResponse("Diameter is required for Rod/Forged and must be greater than 0");
                }
                else if (request.Shape == "Pipe")
                {
                    if (request.Diameter <= 0)
                        return ApiResponse<bool>.ErrorResponse("Outer diameter is required for Pipe and must be greater than 0");
                    if (!request.InnerDiameter.HasValue || request.InnerDiameter.Value <= 0)
                        return ApiResponse<bool>.ErrorResponse("Inner diameter is required for Pipe and must be greater than 0");
                    if (request.InnerDiameter.Value >= request.Diameter)
                        return ApiResponse<bool>.ErrorResponse("Inner diameter must be less than outer diameter");
                }
                else if (request.Shape == "Sheet")
                {
                    if (!request.Width.HasValue || request.Width.Value <= 0)
                        return ApiResponse<bool>.ErrorResponse("Width is required for Sheet and must be greater than 0");
                }

                // Update material entity
                existingMaterial.MaterialName = request.MaterialName.Trim();
                existingMaterial.MaterialType = request.MaterialType.Trim();
                existingMaterial.Grade = request.Grade.Trim();
                existingMaterial.Shape = request.Shape.Trim();
                existingMaterial.Diameter = request.Shape == "Sheet" ? 0 : request.Diameter;
                existingMaterial.InnerDiameter = request.Shape == "Pipe" ? request.InnerDiameter : null;
                existingMaterial.Width = request.Shape == "Sheet" ? request.Width : null;
                existingMaterial.LengthInMM = request.LengthInMM;
                existingMaterial.Density = request.Density;
                existingMaterial.WeightKG = request.WeightKG;

                var success = await _materialRepository.UpdateAsync(existingMaterial);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update material");

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
                    return ApiResponse<bool>.ErrorResponse($"Material with ID {id} not found");

                var success = await _materialRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete material");

                return ApiResponse<bool>.SuccessResponse(true, "Material deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting material: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> SearchByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse("Search term is required");

                var materials = await _materialRepository.SearchByNameAsync(searchTerm);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error searching materials: {ex.Message}");
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
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials by grade: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<MaterialResponse>>> GetByShapeAsync(string shape)
        {
            try
            {
                var materials = await _materialRepository.GetByShapeAsync(shape);
                var responses = materials.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<MaterialResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<MaterialResponse>>.ErrorResponse($"Error retrieving materials by shape: {ex.Message}");
            }
        }

        private static MaterialResponse MapToResponse(Material material)
        {
            return new MaterialResponse
            {
                Id = material.Id,
                MaterialCode = material.MaterialCode,
                MaterialName = material.MaterialName,
                MaterialType = material.MaterialType,
                Grade = material.Grade,
                Shape = material.Shape,
                Diameter = material.Diameter,
                InnerDiameter = material.InnerDiameter,
                Width = material.Width,
                LengthInMM = material.LengthInMM,
                Density = material.Density,
                WeightKG = material.WeightKG,
                IsActive = material.IsActive,
                CreatedAt = material.CreatedAt,
                CreatedBy = material.CreatedBy,
                UpdatedAt = material.UpdatedAt,
                UpdatedBy = material.UpdatedBy
            };
        }
    }
}
