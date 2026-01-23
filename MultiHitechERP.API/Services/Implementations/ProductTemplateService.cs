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
    /// Service implementation for Product Template business logic
    /// </summary>
    public class ProductTemplateService : IProductTemplateService
    {
        private readonly IProductTemplateRepository _productTemplateRepository;

        public ProductTemplateService(IProductTemplateRepository productTemplateRepository)
        {
            _productTemplateRepository = productTemplateRepository;
        }

        public async Task<ApiResponse<ProductTemplateResponse>> GetByIdAsync(int id)
        {
            try
            {
                var template = await _productTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Product template with ID {id} not found");
                }

                var response = MapToResponse(template);
                return ApiResponse<ProductTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Error retrieving product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductTemplateResponse>> GetByNameAsync(string templateName)
        {
            try
            {
                var template = await _productTemplateRepository.GetByNameAsync(templateName);
                if (template == null)
                {
                    return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Product template '{templateName}' not found");
                }

                var response = MapToResponse(template);
                return ApiResponse<ProductTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Error retrieving product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetAllAsync()
        {
            try
            {
                var templates = await _productTemplateRepository.GetAllAsync();
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving product templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetActiveTemplatesAsync()
        {
            try
            {
                var templates = await _productTemplateRepository.GetActiveTemplatesAsync();
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving active templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateTemplateAsync(CreateProductTemplateRequest request)
        {
            try
            {
                // Business Rule 1: Validate template name is unique
                var exists = await _productTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Product template '{request.TemplateName}' already exists");
                }

                // Business Rule 2: Validate required fields
                if (string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    return ApiResponse<int>.ErrorResponse("Template name is required");
                }

                // Create Template
                var template = new ProductTemplate
                {
                    TemplateName = request.TemplateName.Trim(),
                    ProductType = request.ProductType?.Trim(),
                    Category = request.Category?.Trim(),
                    RollerType = request.RollerType?.Trim(),
                    Description = request.Description?.Trim(),
                    ProcessTemplateId = request.ProcessTemplateId,
                    ProcessTemplateName = request.ProcessTemplateName?.Trim(),
                    EstimatedLeadTimeDays = request.EstimatedLeadTimeDays,
                    StandardCost = request.StandardCost,
                    IsActive = request.IsActive,
                    Status = request.Status?.Trim() ?? "Active",
                    IsDefault = request.IsDefault,
                    ApprovedBy = request.ApprovedBy?.Trim(),
                    ApprovalDate = request.ApprovalDate,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var templateId = await _productTemplateRepository.InsertAsync(template);

                return ApiResponse<int>.SuccessResponse(templateId, $"Product template '{request.TemplateName}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateProductTemplateRequest request)
        {
            try
            {
                // Get existing template
                var existingTemplate = await _productTemplateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Product template not found");
                }

                // Business Rule 1: Validate template name uniqueness if changed
                if (existingTemplate.TemplateName != request.TemplateName)
                {
                    var exists = await _productTemplateRepository.ExistsAsync(request.TemplateName);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Product template '{request.TemplateName}' already exists");
                    }
                }

                // Update template
                existingTemplate.TemplateName = request.TemplateName.Trim();
                existingTemplate.ProductType = request.ProductType?.Trim();
                existingTemplate.Category = request.Category?.Trim();
                existingTemplate.RollerType = request.RollerType?.Trim();
                existingTemplate.Description = request.Description?.Trim();
                existingTemplate.ProcessTemplateId = request.ProcessTemplateId;
                existingTemplate.ProcessTemplateName = request.ProcessTemplateName?.Trim();
                existingTemplate.EstimatedLeadTimeDays = request.EstimatedLeadTimeDays;
                existingTemplate.StandardCost = request.StandardCost;
                existingTemplate.IsActive = request.IsActive;
                existingTemplate.Status = request.Status?.Trim();
                existingTemplate.IsDefault = request.IsDefault;
                existingTemplate.ApprovedBy = request.ApprovedBy?.Trim();
                existingTemplate.ApprovalDate = request.ApprovalDate;
                existingTemplate.Remarks = request.Remarks?.Trim();
                existingTemplate.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";

                var success = await _productTemplateRepository.UpdateAsync(existingTemplate);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Product template '{request.TemplateName}' updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update product template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTemplateAsync(int id)
        {
            try
            {
                var template = await _productTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Product template not found");
                }

                var success = await _productTemplateRepository.DeleteAsync(id);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Product template deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete product template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByProductTypeAsync(string productType)
        {
            try
            {
                var templates = await _productTemplateRepository.GetByProductTypeAsync(productType);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving templates by product type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByCategoryAsync(string category)
        {
            try
            {
                var templates = await _productTemplateRepository.GetByCategoryAsync(category);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving templates by category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            try
            {
                var templates = await _productTemplateRepository.GetByProcessTemplateIdAsync(processTemplateId);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving templates by process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetDefaultTemplatesAsync()
        {
            try
            {
                var templates = await _productTemplateRepository.GetDefaultTemplatesAsync();
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving default templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ApproveTemplateAsync(int id, string approvedBy)
        {
            try
            {
                var template = await _productTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Product template not found");
                }

                var success = await _productTemplateRepository.ApproveTemplateAsync(id, approvedBy);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Product template approved by {approvedBy}")
                    : ApiResponse<bool>.ErrorResponse("Failed to approve product template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error approving template: {ex.Message}");
            }
        }

        private static ProductTemplateResponse MapToResponse(ProductTemplate template)
        {
            return new ProductTemplateResponse
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                ProductType = template.ProductType,
                Category = template.Category,
                RollerType = template.RollerType,
                Description = template.Description,
                ProcessTemplateId = template.ProcessTemplateId,
                ProcessTemplateName = template.ProcessTemplateName,
                EstimatedLeadTimeDays = template.EstimatedLeadTimeDays,
                StandardCost = template.StandardCost,
                IsActive = template.IsActive,
                Status = template.Status,
                IsDefault = template.IsDefault,
                ApprovedBy = template.ApprovedBy,
                ApprovalDate = template.ApprovalDate,
                Remarks = template.Remarks,
                CreatedAt = template.CreatedAt,
                CreatedBy = template.CreatedBy,
                UpdatedAt = template.UpdatedAt,
                UpdatedBy = template.UpdatedBy
            };
        }
    }
}
