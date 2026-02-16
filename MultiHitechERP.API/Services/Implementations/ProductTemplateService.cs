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

        #region Template CRUD Operations

        public async Task<ApiResponse<ProductTemplateResponse>> GetByIdAsync(int id)
        {
            try
            {
                var template = await _productTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Product template with ID {id} not found");
                }

                var response = MapToTemplateResponse(template);
                return ApiResponse<ProductTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Error retrieving product template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductTemplateResponse>> GetByCodeAsync(string templateCode)
        {
            try
            {
                var template = await _productTemplateRepository.GetByCodeAsync(templateCode);
                if (template == null)
                {
                    return ApiResponse<ProductTemplateResponse>.ErrorResponse($"Product template with code '{templateCode}' not found");
                }

                var response = MapToTemplateResponse(template);
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

                var response = MapToTemplateResponse(template);
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
                var responses = templates.Select(MapToTemplateResponse).ToList();
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
                var responses = templates.Select(MapToTemplateResponse).ToList();
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
                // Business Rule: Validate template name is unique
                var exists = await _productTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Product template with name '{request.TemplateName}' already exists");
                }

                // Auto-generate TemplateCode
                // Format: ROLLERTYPE-SEQ (e.g., MAG-0001, PRT-0001)
                string prefix = request.RollerType.Length >= 3 ? request.RollerType.Substring(0, 3).ToUpper() : request.RollerType.ToUpper();
                int sequence = await _productTemplateRepository.GetNextSequenceNumberAsync(request.RollerType);
                string templateCode = $"{prefix}-{sequence:D4}";

                // Create template entity
                var template = new ProductTemplate
                {
                    TemplateCode = templateCode,
                    TemplateName = request.TemplateName,
                    Description = request.Description,
                    RollerType = request.RollerType,
                    ProcessTemplateId = request.ProcessTemplateId,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                // Insert template
                var templateId = await _productTemplateRepository.InsertAsync(template);

                // Insert BOM items if any
                if (request.BomItems != null && request.BomItems.Any())
                {
                    var childParts = request.BomItems.Select(bom => new ProductTemplateChildPart
                    {
                        ProductTemplateId = templateId,
                        ChildPartTemplateId = bom.ChildPartTemplateId,
                        Quantity = bom.Quantity,
                        Unit = "pcs", // Default unit
                        Notes = bom.Notes,
                        SequenceNo = bom.SequenceNumber ?? 0,
                        // Name and Code will be fetched from Child Part Template when needed
                        ChildPartName = "", // Placeholder - not used in new architecture
                        ChildPartCode = ""  // Placeholder - not used in new architecture
                    }).ToList();

                    await _productTemplateRepository.InsertChildPartsAsync(templateId, childParts);
                }

                return ApiResponse<int>.SuccessResponse(templateId, $"Product template '{templateCode}' created successfully");
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
                // Verify template exists
                var existingTemplate = await _productTemplateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"Product template with ID {request.Id} not found");
                }

                // Update template entity (preserve TemplateCode and RollerType - they're immutable)
                var template = new ProductTemplate
                {
                    Id = request.Id,
                    TemplateCode = existingTemplate.TemplateCode, // Preserve original code
                    TemplateName = request.TemplateName,
                    Description = request.Description,
                    RollerType = existingTemplate.RollerType, // Preserve original roller type
                    ProcessTemplateId = request.ProcessTemplateId,
                    IsActive = request.IsActive,
                    CreatedAt = existingTemplate.CreatedAt,
                    CreatedBy = existingTemplate.CreatedBy,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = request.UpdatedBy ?? "System"
                };

                // Update template
                var success = await _productTemplateRepository.UpdateAsync(template);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update product template");
                }

                // Update BOM items: Delete all and re-insert
                await _productTemplateRepository.DeleteChildPartsByTemplateIdAsync(request.Id);

                if (request.BomItems != null && request.BomItems.Any())
                {
                    var childParts = request.BomItems.Select(bom => new ProductTemplateChildPart
                    {
                        ProductTemplateId = request.Id,
                        ChildPartTemplateId = bom.ChildPartTemplateId,
                        Quantity = bom.Quantity,
                        Unit = "pcs", // Default unit
                        Notes = bom.Notes,
                        SequenceNo = bom.SequenceNumber ?? 0,
                        // Name and Code will be fetched from Child Part Template when needed
                        ChildPartName = "", // Placeholder
                        ChildPartCode = ""  // Placeholder
                    }).ToList();

                    await _productTemplateRepository.InsertChildPartsAsync(request.Id, childParts);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Product template updated successfully");
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
                // Verify template exists
                var template = await _productTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"Product template with ID {id} not found");
                }

                // Delete template (child parts will cascade)
                var success = await _productTemplateRepository.DeleteAsync(id);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete product template");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Product template deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting product template: {ex.Message}");
            }
        }

        #endregion

        #region Business Queries

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByRollerTypeAsync(string rollerType)
        {
            try
            {
                var templates = await _productTemplateRepository.GetByRollerTypeAsync(rollerType);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving templates by roller type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductTemplateResponse>>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            try
            {
                var templates = await _productTemplateRepository.GetByProcessTemplateIdAsync(processTemplateId);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProductTemplateResponse>>.ErrorResponse($"Error retrieving templates by process template: {ex.Message}");
            }
        }

        #endregion

        #region Mapping Methods

        private ProductTemplateResponse MapToTemplateResponse(ProductTemplate template)
        {
            return new ProductTemplateResponse
            {
                Id = template.Id,
                TemplateCode = template.TemplateCode,
                TemplateName = template.TemplateName,
                Description = template.Description,
                RollerType = template.RollerType,
                ProcessTemplateId = template.ProcessTemplateId,
                ProcessTemplateName = template.ProcessTemplateName,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt ?? template.CreatedAt,
                CreatedBy = template.CreatedBy,
                BomItems = template.ChildParts?.Select(MapToBOMItemResponse).ToList() ?? new List<ProductTemplateBOMItemResponse>()
            };
        }

        private ProductTemplateBOMItemResponse MapToBOMItemResponse(ProductTemplateChildPart childPart)
        {
            return new ProductTemplateBOMItemResponse
            {
                Id = childPart.Id,
                ChildPartTemplateId = childPart.ChildPartTemplateId ?? 0,
                ChildPartTemplateName = childPart.ChildPartName,
                ChildPartTemplateCode = childPart.ChildPartCode,
                ChildPartType = childPart.ChildPartType,
                IsPurchased = childPart.IsPurchased,
                Quantity = (int)childPart.Quantity,
                Notes = childPart.Notes,
                SequenceNumber = childPart.SequenceNo
            };
        }

        #endregion
    }
}
