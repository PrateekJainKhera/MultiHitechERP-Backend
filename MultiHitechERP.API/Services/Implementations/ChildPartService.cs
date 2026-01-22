using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for ChildPart business logic
    /// </summary>
    public class ChildPartService : IChildPartService
    {
        private readonly IChildPartRepository _childPartRepository;

        public ChildPartService(IChildPartRepository childPartRepository)
        {
            _childPartRepository = childPartRepository;
        }

        public async Task<ApiResponse<ChildPart>> GetByIdAsync(int id)
        {
            var childPart = await _childPartRepository.GetByIdAsync(id);
            if (childPart == null)
                return ApiResponse<ChildPart>.ErrorResponse("Child part not found");

            return ApiResponse<ChildPart>.SuccessResponse(childPart);
        }

        public async Task<ApiResponse<ChildPart>> GetByCodeAsync(string childPartCode)
        {
            if (string.IsNullOrWhiteSpace(childPartCode))
                return ApiResponse<ChildPart>.ErrorResponse("Child part code is required");

            var childPart = await _childPartRepository.GetByCodeAsync(childPartCode);
            if (childPart == null)
                return ApiResponse<ChildPart>.ErrorResponse("Child part not found");

            return ApiResponse<ChildPart>.SuccessResponse(childPart);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetAllAsync()
        {
            var childParts = await _childPartRepository.GetAllAsync();
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<int>> CreateChildPartAsync(ChildPart childPart)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(childPart.ChildPartCode))
                return ApiResponse<int>.ErrorResponse("Child part code is required");

            if (string.IsNullOrWhiteSpace(childPart.ChildPartName))
                return ApiResponse<int>.ErrorResponse("Child part name is required");

            // Check if code already exists
            var existing = await _childPartRepository.GetByCodeAsync(childPart.ChildPartCode);
            if (existing != null)
                return ApiResponse<int>.ErrorResponse($"Child part with code '{childPart.ChildPartCode}' already exists");

            var id = await _childPartRepository.InsertAsync(childPart);
            return ApiResponse<int>.SuccessResponse(id, "Child part created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateChildPartAsync(ChildPart childPart)
        {
            var existing = await _childPartRepository.GetByIdAsync(childPart.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Child part not found");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(childPart.ChildPartCode))
                return ApiResponse<bool>.ErrorResponse("Child part code is required");

            if (string.IsNullOrWhiteSpace(childPart.ChildPartName))
                return ApiResponse<bool>.ErrorResponse("Child part name is required");

            // Check if code is being changed to an existing code
            if (childPart.ChildPartCode != existing.ChildPartCode)
            {
                var duplicate = await _childPartRepository.GetByCodeAsync(childPart.ChildPartCode);
                if (duplicate != null)
                    return ApiResponse<bool>.ErrorResponse($"Child part with code '{childPart.ChildPartCode}' already exists");
            }

            var success = await _childPartRepository.UpdateAsync(childPart);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update child part");

            return ApiResponse<bool>.SuccessResponse(true, "Child part updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteChildPartAsync(int id)
        {
            var existing = await _childPartRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Child part not found");

            var success = await _childPartRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete child part");

            return ApiResponse<bool>.SuccessResponse(true, "Child part deleted successfully");
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductIdAsync(int productId)
        {
            var childParts = await _childPartRepository.GetByProductIdAsync(productId);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByProductCodeAsync(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                return ApiResponse<IEnumerable<ChildPart>>.ErrorResponse("Product code is required");

            var childParts = await _childPartRepository.GetByProductCodeAsync(productCode);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByMaterialIdAsync(int materialId)
        {
            var childParts = await _childPartRepository.GetByMaterialIdAsync(materialId);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByPartTypeAsync(string partType)
        {
            if (string.IsNullOrWhiteSpace(partType))
                return ApiResponse<IEnumerable<ChildPart>>.ErrorResponse("Part type is required");

            var childParts = await _childPartRepository.GetByPartTypeAsync(partType);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return ApiResponse<IEnumerable<ChildPart>>.ErrorResponse("Category is required");

            var childParts = await _childPartRepository.GetByCategoryAsync(category);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetActiveAsync()
        {
            var childParts = await _childPartRepository.GetActiveAsync();
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<ChildPart>>.ErrorResponse("Status is required");

            var childParts = await _childPartRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByMakeOrBuyAsync(string makeOrBuy)
        {
            if (string.IsNullOrWhiteSpace(makeOrBuy))
                return ApiResponse<IEnumerable<ChildPart>>.ErrorResponse("Make or Buy is required");

            var childParts = await _childPartRepository.GetByMakeOrBuyAsync(makeOrBuy);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByDrawingIdAsync(int drawingId)
        {
            var childParts = await _childPartRepository.GetByDrawingIdAsync(drawingId);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<IEnumerable<ChildPart>>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            var childParts = await _childPartRepository.GetByProcessTemplateIdAsync(processTemplateId);
            return ApiResponse<IEnumerable<ChildPart>>.SuccessResponse(childParts);
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            var childPart = await _childPartRepository.GetByIdAsync(id);
            if (childPart == null)
                return ApiResponse<bool>.ErrorResponse("Child part not found");

            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<bool>.ErrorResponse("Status is required");

            var success = await _childPartRepository.UpdateStatusAsync(id, status);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update child part status");

            return ApiResponse<bool>.SuccessResponse(true, "Child part status updated successfully");
        }
    }
}
