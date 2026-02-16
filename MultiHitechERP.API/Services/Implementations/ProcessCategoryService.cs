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
    public class ProcessCategoryService : IProcessCategoryService
    {
        private readonly IProcessCategoryRepository _repository;

        public ProcessCategoryService(IProcessCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<ProcessCategoryResponse>>> GetAllAsync()
        {
            try
            {
                var categories = await _repository.GetAllAsync();
                var responses = categories.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProcessCategoryResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessCategoryResponse>>.ErrorResponse($"Error retrieving process categories: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProcessCategoryResponse>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<ProcessCategoryResponse>.ErrorResponse($"Process category with ID {id} not found");

                var response = MapToResponse(category);
                return ApiResponse<ProcessCategoryResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessCategoryResponse>.ErrorResponse($"Error retrieving process category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateProcessCategoryRequest request)
        {
            try
            {
                // Auto-generate category code from name
                var categoryCode = GenerateCategoryCode(request.CategoryName);

                // Check for duplicate category code
                if (await _repository.CategoryCodeExistsAsync(categoryCode))
                {
                    return ApiResponse<int>.ErrorResponse($"Process category with code '{categoryCode}' already exists");
                }

                var category = new ProcessCategory
                {
                    CategoryCode = categoryCode,
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    IsActive = true, // Always active when created
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "Admin"
                };

                var id = await _repository.CreateAsync(category);
                return ApiResponse<int>.SuccessResponse(id, "Process category created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating process category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(UpdateProcessCategoryRequest request)
        {
            try
            {
                var existingCategory = await _repository.GetByIdAsync(request.Id);
                if (existingCategory == null)
                    return ApiResponse<bool>.ErrorResponse($"Process category with ID {request.Id} not found");

                // Check for duplicate category code (excluding current record)
                if (await _repository.CategoryCodeExistsAsync(request.CategoryCode, request.Id))
                {
                    return ApiResponse<bool>.ErrorResponse($"Process category with code '{request.CategoryCode}' already exists");
                }

                existingCategory.CategoryCode = request.CategoryCode;
                existingCategory.CategoryName = request.CategoryName;
                existingCategory.Description = request.Description;
                existingCategory.IsActive = request.IsActive;
                existingCategory.UpdatedAt = DateTime.UtcNow;
                existingCategory.UpdatedBy = "Admin"; // TODO: Get from auth context

                var success = await _repository.UpdateAsync(existingCategory);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Process category updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update process category");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating process category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                    return ApiResponse<bool>.ErrorResponse($"Process category with ID {id} not found");

                var success = await _repository.DeleteAsync(id);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Process category deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete process category");
            }
            catch (Exception ex)
            {
                // Check if error is due to foreign key constraint
                if (ex.Message.Contains("REFERENCE constraint"))
                {
                    return ApiResponse<bool>.ErrorResponse("Cannot delete process category as it is being used by processes or machines");
                }
                return ApiResponse<bool>.ErrorResponse($"Error deleting process category: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a category code from the category name
        /// Examples: "Turning 1" -> "TURN-1", "Heat Treatment" -> "HEAT"
        /// </summary>
        private static string GenerateCategoryCode(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return "CAT";

            // Remove special characters and convert to uppercase
            var cleanName = new string(categoryName
                .ToUpper()
                .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-')
                .ToArray());

            // Split by spaces and take first letters or first word
            var words = cleanName.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0)
                return "CAT";

            if (words.Length == 1)
            {
                // Single word: take first 10 characters
                return words[0].Substring(0, Math.Min(10, words[0].Length));
            }

            // Multiple words: take first 4 letters of first word + dash + rest
            // Examples: "Turning 1" -> "TURN-1", "Grinding Machine 2" -> "GRIN-MACHINE-2"
            var firstPart = words[0].Substring(0, Math.Min(4, words[0].Length));
            var rest = string.Join("-", words.Skip(1));

            var result = $"{firstPart}-{rest}";
            return result.Substring(0, Math.Min(20, result.Length)); // Max 20 chars
        }

        private static ProcessCategoryResponse MapToResponse(ProcessCategory category)
        {
            return new ProcessCategoryResponse
            {
                Id = category.Id,
                CategoryCode = category.CategoryCode,
                CategoryName = category.CategoryName,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                CreatedBy = category.CreatedBy,
                UpdatedAt = category.UpdatedAt,
                UpdatedBy = category.UpdatedBy
            };
        }
    }
}
