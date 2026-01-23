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
    public class ChildPartTemplateService : IChildPartTemplateService
    {
        private readonly IChildPartTemplateRepository _childPartTemplateRepository;

        public ChildPartTemplateService(IChildPartTemplateRepository childPartTemplateRepository)
        {
            _childPartTemplateRepository = childPartTemplateRepository;
        }

        public async Task<ApiResponse<ChildPartTemplateResponse>> GetByIdAsync(int id)
        {
            try
            {
                var template = await _childPartTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<ChildPartTemplateResponse>.ErrorResponse($"Child part template with ID {id} not found");
                }

                var response = MapToResponse(template);
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

                var response = MapToResponse(template);
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
                var responses = templates.Select(MapToResponse).ToList();
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
                var responses = templates.Select(MapToResponse).ToList();
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
                var exists = await _childPartTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Child part template '{request.TemplateName}' already exists");
                }

                var template = new ChildPartTemplate
                {
                    TemplateName = request.TemplateName.Trim(),
                    ChildPartType = request.ChildPartType.Trim(),
                    Description = request.Description?.Trim(),
                    Category = request.Category?.Trim(),
                    Length = request.Length,
                    Diameter = request.Diameter,
                    InnerDiameter = request.InnerDiameter,
                    OuterDiameter = request.OuterDiameter,
                    Thickness = request.Thickness,
                    Width = request.Width,
                    Height = request.Height,
                    MaterialType = request.MaterialType?.Trim(),
                    MaterialGrade = request.MaterialGrade?.Trim(),
                    ProcessTemplateId = request.ProcessTemplateId,
                    ProcessTemplateName = request.ProcessTemplateName?.Trim(),
                    EstimatedCost = request.EstimatedCost,
                    EstimatedLeadTimeDays = request.EstimatedLeadTimeDays,
                    EstimatedWeight = request.EstimatedWeight,
                    IsActive = request.IsActive,
                    Status = request.Status?.Trim() ?? "Active",
                    IsDefault = request.IsDefault,
                    ApprovedBy = request.ApprovedBy?.Trim(),
                    ApprovalDate = request.ApprovalDate,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var templateId = await _childPartTemplateRepository.InsertAsync(template);
                return ApiResponse<int>.SuccessResponse(templateId, $"Child part template '{request.TemplateName}' created successfully");
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
                var existingTemplate = await _childPartTemplateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Child part template not found");
                }

                if (existingTemplate.TemplateName != request.TemplateName)
                {
                    var exists = await _childPartTemplateRepository.ExistsAsync(request.TemplateName);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Child part template '{request.TemplateName}' already exists");
                    }
                }

                existingTemplate.TemplateName = request.TemplateName.Trim();
                existingTemplate.ChildPartType = request.ChildPartType.Trim();
                existingTemplate.Description = request.Description?.Trim();
                existingTemplate.Category = request.Category?.Trim();
                existingTemplate.Length = request.Length;
                existingTemplate.Diameter = request.Diameter;
                existingTemplate.InnerDiameter = request.InnerDiameter;
                existingTemplate.OuterDiameter = request.OuterDiameter;
                existingTemplate.Thickness = request.Thickness;
                existingTemplate.Width = request.Width;
                existingTemplate.Height = request.Height;
                existingTemplate.MaterialType = request.MaterialType?.Trim();
                existingTemplate.MaterialGrade = request.MaterialGrade?.Trim();
                existingTemplate.ProcessTemplateId = request.ProcessTemplateId;
                existingTemplate.ProcessTemplateName = request.ProcessTemplateName?.Trim();
                existingTemplate.EstimatedCost = request.EstimatedCost;
                existingTemplate.EstimatedLeadTimeDays = request.EstimatedLeadTimeDays;
                existingTemplate.EstimatedWeight = request.EstimatedWeight;
                existingTemplate.IsActive = request.IsActive;
                existingTemplate.Status = request.Status?.Trim();
                existingTemplate.IsDefault = request.IsDefault;
                existingTemplate.ApprovedBy = request.ApprovedBy?.Trim();
                existingTemplate.ApprovalDate = request.ApprovalDate;
                existingTemplate.Remarks = request.Remarks?.Trim();
                existingTemplate.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";

                var success = await _childPartTemplateRepository.UpdateAsync(existingTemplate);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Child part template '{request.TemplateName}' updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update child part template");
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
                var template = await _childPartTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Child part template not found");
                }

                var success = await _childPartTemplateRepository.DeleteAsync(id);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Child part template deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete child part template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting child part template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByChildPartTypeAsync(string childPartType)
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetByChildPartTypeAsync(childPartType);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving templates by child part type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByCategoryAsync(string category)
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetByCategoryAsync(category);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving templates by category: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetByProcessTemplateIdAsync(int processTemplateId)
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetByProcessTemplateIdAsync(processTemplateId);
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving templates by process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ChildPartTemplateResponse>>> GetDefaultTemplatesAsync()
        {
            try
            {
                var templates = await _childPartTemplateRepository.GetDefaultTemplatesAsync();
                var responses = templates.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ChildPartTemplateResponse>>.ErrorResponse($"Error retrieving default templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ApproveTemplateAsync(int id, string approvedBy)
        {
            try
            {
                var template = await _childPartTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Child part template not found");
                }

                var success = await _childPartTemplateRepository.ApproveTemplateAsync(id, approvedBy);
                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Child part template approved by {approvedBy}")
                    : ApiResponse<bool>.ErrorResponse("Failed to approve child part template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error approving template: {ex.Message}");
            }
        }

        private static ChildPartTemplateResponse MapToResponse(ChildPartTemplate template)
        {
            return new ChildPartTemplateResponse
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                ChildPartType = template.ChildPartType,
                Description = template.Description,
                Category = template.Category,
                Length = template.Length,
                Diameter = template.Diameter,
                InnerDiameter = template.InnerDiameter,
                OuterDiameter = template.OuterDiameter,
                Thickness = template.Thickness,
                Width = template.Width,
                Height = template.Height,
                MaterialType = template.MaterialType,
                MaterialGrade = template.MaterialGrade,
                ProcessTemplateId = template.ProcessTemplateId,
                ProcessTemplateName = template.ProcessTemplateName,
                EstimatedCost = template.EstimatedCost,
                EstimatedLeadTimeDays = template.EstimatedLeadTimeDays,
                EstimatedWeight = template.EstimatedWeight,
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
