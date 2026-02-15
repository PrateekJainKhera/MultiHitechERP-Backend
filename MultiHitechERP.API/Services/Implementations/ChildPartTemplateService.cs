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
    /// Service implementation for Child Part Template business logic
    /// </summary>
    public class ChildPartTemplateService : IChildPartTemplateService
    {
        private readonly IChildPartTemplateRepository _childPartTemplateRepository;

        public ChildPartTemplateService(IChildPartTemplateRepository childPartTemplateRepository)
        {
            _childPartTemplateRepository = childPartTemplateRepository;
        }

        #region Template CRUD Operations

        public async Task<ApiResponse<ChildPartTemplateResponse>> GetByIdAsync(int id)
        {
            try
            {
                var template = await _childPartTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Child part template with ID {id} not found");
                }

                var response = MapToTemplateResponse(template);

                // Load sub-entities for detail view
                var materialReqs = await _childPartTemplateRepository.GetMaterialRequirementsByTemplateIdAsync(id);
                response.MaterialRequirements = materialReqs.Select(MapToMaterialRequirementResponse).ToList();

                var processSteps = await _childPartTemplateRepository.GetProcessStepsByTemplateIdAsync(id);
                response.ProcessSteps = processSteps.Select(MapToProcessStepResponse).ToList();

                return ApiResponse<ChildPartTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Error retrieving child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ChildPartTemplateResponse>> GetByCodeAsync(string templateCode)
        {
            try
            {
                var template = await _childPartTemplateRepository.GetByCodeAsync(templateCode);
                if (template == null)
                {
                    return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Child part template with code '{templateCode}' not found");
                }

                var response = MapToTemplateResponse(template);
                return ApiResponse<ChildPartTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Error retrieving child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ChildPartTemplateResponse>> GetByNameAsync(string templateName)
        {
            try
            {
                var template = await _childPartTemplateRepository.GetByNameAsync(templateName);
                if (template == null)
                {
                    return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Child part template '{templateName}' not found");
                }

                var response = MapToTemplateResponse(template);
                return ApiResponse<ChildPartTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Error retrieving child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetAllAsync()
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetAllAsync();
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving child part templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetActiveTemplatesAsync()
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetActiveTemplatesAsync();
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving active templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateTemplateAsync(CreateChildPartTemplateRequest request)
        {
            try
            {
                // Business Rule: Validate template name is unique
                var exists = await _childPartTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Child part template with name '{request.TemplateName}' already exists");
                }

                // Use provided TemplateCode or auto-generate
                string templateCode = request.TemplateCode ?? string.Empty;
                if (string.IsNullOrWhiteSpace(templateCode))
                {
                    int sequence = await _childPartTemplateRepository.GetNextSequenceNumberAsync();
                    templateCode = $"CPT-{sequence:D4}";
                }

                // Create template entity
                var template = new ChildPartTemplate
                {
                    TemplateCode = templateCode,
                    TemplateName = request.TemplateName,
                    ChildPartType = request.ChildPartType,
                    RollerType = request.RollerType,
                    ProcessTemplateId = request.ProcessTemplateId,
                    IsPurchased = request.IsPurchased,
                    DrawingNumber = request.DrawingNumber,
                    DrawingRevision = request.DrawingRevision,
                    Length = request.Length,
                    Diameter = request.Diameter,
                    InnerDiameter = request.InnerDiameter,
                    OuterDiameter = request.OuterDiameter,
                    Thickness = request.Thickness,
                    DimensionUnit = request.DimensionUnit,
                    Description = request.Description,
                    TechnicalNotes = request.TechnicalNotes,
                    IsActive = request.IsActive,
                    CreatedBy = request.CreatedBy
                };

                // Insert template
                var templateId = await _childPartTemplateRepository.InsertAsync(template);

                // Material requirements and process steps are now handled via ProcessTemplateId reference
                // No need to insert them separately

                return ApiResponse<int>.SuccessResponse(templateId, $"Child part template '{templateCode}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateChildPartTemplateRequest request)
        {
            try
            {
                // Verify template exists
                var existingTemplate = await _childPartTemplateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"Child part template with ID {request.Id} not found");
                }

                // Update template entity (preserve TemplateCode - it's immutable)
                var template = new ChildPartTemplate
                {
                    Id = request.Id,
                    TemplateCode = existingTemplate.TemplateCode, // Preserve original code (immutable)
                    TemplateName = request.TemplateName,
                    ChildPartType = request.ChildPartType,
                    RollerType = request.RollerType,
                    ProcessTemplateId = request.ProcessTemplateId,
                    IsPurchased = request.IsPurchased,
                    DrawingNumber = request.DrawingNumber,
                    DrawingRevision = request.DrawingRevision,
                    Length = request.Length,
                    Diameter = request.Diameter,
                    InnerDiameter = request.InnerDiameter,
                    OuterDiameter = request.OuterDiameter,
                    Thickness = request.Thickness,
                    DimensionUnit = request.DimensionUnit,
                    Description = request.Description,
                    TechnicalNotes = request.TechnicalNotes,
                    IsActive = request.IsActive,
                    CreatedAt = existingTemplate.CreatedAt,
                    CreatedBy = existingTemplate.CreatedBy
                };

                // Update template
                var success = await _childPartTemplateRepository.UpdateAsync(template);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update child part template");
                }

                // Material requirements and process steps are now handled via ProcessTemplateId reference
                // No need to update them separately

                return ApiResponse<bool>.SuccessResponse(true, "Child part template updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTemplateAsync(int id)
        {
            try
            {
                // Verify template exists
                var template = await _childPartTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse($"Child part template with ID {id} not found");
                }

                // Delete template (material requirements and process steps will cascade)
                var success = await _childPartTemplateRepository.DeleteAsync(id);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to delete child part template");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Child part template deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting child part template: {ex.Message}");
            }
        }

        #endregion

        #region Business Queries

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByChildPartTypeAsync(string childPartType)
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetByChildPartTypeAsync(childPartType);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving templates by child part type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByRollerTypeAsync(string rollerType)
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetByRollerTypeAsync(rollerType);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving templates by roller type: {ex.Message}");
            }
        }

        #endregion

        #region Mapping Methods

        private ChildPartTemplateResponse MapToTemplateResponse(ChildPartTemplate template)
        {
            return new ChildPartTemplateResponse
            {
                Id = template.Id,
                TemplateCode = template.TemplateCode,
                TemplateName = template.TemplateName,
                ChildPartType = template.ChildPartType,
                RollerType = template.RollerType,
                ProcessTemplateId = template.ProcessTemplateId,
                IsPurchased = template.IsPurchased,
                DrawingNumber = template.DrawingNumber,
                DrawingRevision = template.DrawingRevision,
                Length = template.Length,
                Diameter = template.Diameter,
                InnerDiameter = template.InnerDiameter,
                OuterDiameter = template.OuterDiameter,
                Thickness = template.Thickness,
                DimensionUnit = template.DimensionUnit,
                Description = template.Description,
                TechnicalNotes = template.TechnicalNotes,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                CreatedBy = template.CreatedBy
            };
        }

        private ChildPartTemplateMaterialRequirementResponse MapToMaterialRequirementResponse(ChildPartTemplateMaterialRequirement req)
        {
            return new ChildPartTemplateMaterialRequirementResponse
            {
                Id = req.Id,
                RawMaterialId = req.RawMaterialId,
                RawMaterialName = req.RawMaterialName,
                MaterialGrade = req.MaterialGrade,
                QuantityRequired = req.QuantityRequired,
                Unit = req.Unit,
                WastageMM = req.WastageMM
            };
        }

        private ChildPartTemplateProcessStepResponse MapToProcessStepResponse(ChildPartTemplateProcessStep step)
        {
            return new ChildPartTemplateProcessStepResponse
            {
                Id = step.Id,
                ProcessId = step.ProcessId,
                ProcessName = step.ProcessName,
                StepNumber = step.StepNumber,
                MachineName = step.MachineName,
                StandardTimeHours = step.StandardTimeHours,
                RestTimeHours = step.RestTimeHours,
                Description = step.Description
            };
        }

        #endregion
    }
}
