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
    /// Service implementation for Process Template business logic
    /// </summary>
    public class ProcessTemplateService : IProcessTemplateService
    {
        private readonly IProcessTemplateRepository _processTemplateRepository;

        public ProcessTemplateService(IProcessTemplateRepository processTemplateRepository)
        {
            _processTemplateRepository = processTemplateRepository;
        }

        #region Template CRUD Operations

        public async Task<ApiResponse<ProcessTemplateResponse>> GetByIdAsync(int id)
        {
            try
            {
                var template = await _processTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<ProcessTemplateResponse>.ErrorResponse($"Process template with ID {id} not found");
                }

                var response = MapToTemplateResponse(template);
                return ApiResponse<ProcessTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessTemplateResponse>.ErrorResponse($"Error retrieving process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProcessTemplateResponse>> GetByNameAsync(string templateName)
        {
            try
            {
                var template = await _processTemplateRepository.GetByNameAsync(templateName);
                if (template == null)
                {
                    return ApiResponse<ProcessTemplateResponse>.ErrorResponse($"Process template '{templateName}' not found");
                }

                var response = MapToTemplateResponse(template);
                return ApiResponse<ProcessTemplateResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessTemplateResponse>.ErrorResponse($"Error retrieving process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetAllAsync()
        {
            try
            {
                var templates = await _processTemplateRepository.GetAllAsync();
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving process templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetActiveTemplatesAsync()
        {
            try
            {
                var templates = await _processTemplateRepository.GetActiveTemplatesAsync();
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving active templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateTemplateAsync(CreateProcessTemplateRequest request)
        {
            try
            {
                // Validate template name is unique
                var exists = await _processTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Process template '{request.TemplateName}' already exists");
                }

                // Validate at least one applicable type
                if (request.ApplicableTypes == null || !request.ApplicableTypes.Any())
                {
                    return ApiResponse<int>.ErrorResponse("At least one applicable roller type is required");
                }

                // Create Template
                var template = new ProcessTemplate
                {
                    TemplateName = request.TemplateName.Trim(),
                    Description = request.Description?.Trim(),
                    ApplicableTypes = request.ApplicableTypes,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System",
                    UpdatedAt = DateTime.UtcNow
                };

                var templateId = await _processTemplateRepository.InsertAsync(template);

                // Insert steps if any
                if (request.Steps != null && request.Steps.Any())
                {
                    foreach (var stepRequest in request.Steps)
                    {
                        var step = new ProcessTemplateStep
                        {
                            TemplateId = templateId,
                            StepNo = stepRequest.StepNo,
                            ProcessId = stepRequest.ProcessId,
                            ProcessName = stepRequest.ProcessName,
                            IsMandatory = stepRequest.IsMandatory,
                            CanBeParallel = stepRequest.CanBeParallel
                        };
                        await _processTemplateRepository.InsertStepAsync(step);
                    }
                }

                return ApiResponse<int>.SuccessResponse(templateId, $"Process template '{request.TemplateName}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateTemplateAsync(UpdateProcessTemplateRequest request)
        {
            try
            {
                // Get existing template
                var existingTemplate = await _processTemplateRepository.GetByIdAsync(request.Id);
                if (existingTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Process template not found");
                }

                // Validate template name uniqueness if changed
                if (existingTemplate.TemplateName != request.TemplateName)
                {
                    var exists = await _processTemplateRepository.ExistsAsync(request.TemplateName);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Process template '{request.TemplateName}' already exists");
                    }
                }

                // Validate at least one applicable type
                if (request.ApplicableTypes == null || !request.ApplicableTypes.Any())
                {
                    return ApiResponse<bool>.ErrorResponse("At least one applicable roller type is required");
                }

                // Update template
                var template = new ProcessTemplate
                {
                    Id = request.Id,
                    TemplateName = request.TemplateName.Trim(),
                    Description = request.Description?.Trim(),
                    ApplicableTypes = request.ApplicableTypes,
                    IsActive = request.IsActive,
                    CreatedAt = existingTemplate.CreatedAt,
                    CreatedBy = existingTemplate.CreatedBy,
                    UpdatedAt = DateTime.UtcNow
                };

                var success = await _processTemplateRepository.UpdateAsync(template);
                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update process template");
                }

                // Update steps: Delete all and re-insert
                await _processTemplateRepository.DeleteAllStepsAsync(request.Id);

                if (request.Steps != null && request.Steps.Any())
                {
                    foreach (var stepRequest in request.Steps)
                    {
                        var step = new ProcessTemplateStep
                        {
                            TemplateId = request.Id,
                            StepNo = stepRequest.StepNo,
                            ProcessId = stepRequest.ProcessId,
                            ProcessName = stepRequest.ProcessName,
                            IsMandatory = stepRequest.IsMandatory,
                            CanBeParallel = stepRequest.CanBeParallel
                        };
                        await _processTemplateRepository.InsertStepAsync(step);
                    }
                }

                return ApiResponse<bool>.SuccessResponse(true, $"Process template '{request.TemplateName}' updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating process template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTemplateAsync(int id)
        {
            try
            {
                var template = await _processTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Process template not found");
                }

                var success = await _processTemplateRepository.DeleteAsync(id);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Process template deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete process template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting process template: {ex.Message}");
            }
        }

        #endregion

        #region Template with Steps Operations

        public async Task<ApiResponse<ProcessTemplateWithStepsResponse>> GetTemplateWithStepsAsync(int templateId)
        {
            try
            {
                var template = await _processTemplateRepository.GetByIdAsync(templateId);
                if (template == null)
                {
                    return ApiResponse<ProcessTemplateWithStepsResponse>.ErrorResponse($"Process template with ID {templateId} not found");
                }

                var steps = await _processTemplateRepository.GetStepsByTemplateIdAsync(templateId);

                var response = new ProcessTemplateWithStepsResponse
                {
                    Template = MapToTemplateResponse(template),
                    Steps = steps.Select(MapToStepResponse).ToList()
                };

                return ApiResponse<ProcessTemplateWithStepsResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<ProcessTemplateWithStepsResponse>.ErrorResponse($"Error retrieving template with steps: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateTemplateWithStepsAsync(CreateProcessTemplateWithStepsRequest request)
        {
            try
            {
                // Create the template first
                var templateResult = await CreateTemplateAsync(request.Template);
                if (!templateResult.Success)
                {
                    return ApiResponse<int>.ErrorResponse(templateResult.Message);
                }

                var templateId = templateResult.Data;

                // Create the steps
                foreach (var stepRequest in request.Steps)
                {
                    stepRequest.TemplateId = templateId; // Ensure correct template ID
                    await AddStepToTemplateAsync(stepRequest);
                }

                return ApiResponse<int>.SuccessResponse(templateId, $"Process template '{request.Template.TemplateName}' with {request.Steps.Count} steps created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating template with steps: {ex.Message}");
            }
        }

        #endregion

        #region Template Steps Operations

        public async Task<ApiResponse<IEnumerable<ProcessTemplateStepResponse>>> GetStepsByTemplateIdAsync(int templateId)
        {
            try
            {
                var steps = await _processTemplateRepository.GetStepsByTemplateIdAsync(templateId);
                var responses = steps.Select(MapToStepResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateStepResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateStepResponse>>.ErrorResponse($"Error retrieving template steps: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> AddStepToTemplateAsync(CreateProcessTemplateStepRequest request)
        {
            try
            {
                // Business Rule 1: Validate template exists
                var template = await _processTemplateRepository.GetByIdAsync(request.TemplateId);
                if (template == null)
                {
                    return ApiResponse<int>.ErrorResponse($"Template with ID {request.TemplateId} not found");
                }

                // Business Rule 2: Validate step number
                if (request.StepNo <= 0)
                {
                    return ApiResponse<int>.ErrorResponse("Step number must be greater than 0");
                }

                // Create Step
                var step = new ProcessTemplateStep
                {
                    TemplateId = request.TemplateId,
                    StepNo = request.StepNo,
                    ProcessId = request.ProcessId,
                    ProcessName = request.ProcessName?.Trim(),
                    IsMandatory = request.IsMandatory,
                    CanBeParallel = request.CanBeParallel
                };

                var stepId = await _processTemplateRepository.InsertStepAsync(step);

                return ApiResponse<int>.SuccessResponse(stepId, $"Step {request.StepNo} added to template successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error adding step to template: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStepAsync(UpdateProcessTemplateStepRequest request)
        {
            try
            {
                // Get existing step
                var existingStep = await _processTemplateRepository.GetStepByIdAsync(request.Id);
                if (existingStep == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Template step not found");
                }

                // Update step
                existingStep.TemplateId = request.TemplateId;
                existingStep.StepNo = request.StepNo;
                existingStep.ProcessId = request.ProcessId;
                existingStep.ProcessName = request.ProcessName?.Trim();
                existingStep.IsMandatory = request.IsMandatory;
                existingStep.CanBeParallel = request.CanBeParallel;

                var success = await _processTemplateRepository.UpdateStepAsync(existingStep);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Step {request.StepNo} updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update template step");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating template step: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteStepAsync(int stepId)
        {
            try
            {
                var step = await _processTemplateRepository.GetStepByIdAsync(stepId);
                if (step == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Template step not found");
                }

                var success = await _processTemplateRepository.DeleteStepAsync(stepId);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, "Template step deleted successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to delete template step");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting template step: {ex.Message}");
            }
        }

        #endregion

        #region Business Queries

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetByApplicableTypeAsync(string applicableType)
        {
            try
            {
                var templates = await _processTemplateRepository.GetByApplicableTypeAsync(applicableType);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving templates by applicable type: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private static ProcessTemplateResponse MapToTemplateResponse(ProcessTemplate template)
        {
            return new ProcessTemplateResponse
            {
                Id = template.Id,
                TemplateName = template.TemplateName,
                Description = template.Description,
                ApplicableTypes = template.ApplicableTypes ?? new List<string>(),
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                CreatedBy = template.CreatedBy,
                UpdatedAt = template.UpdatedAt
            };
        }

        private static ProcessTemplateStepResponse MapToStepResponse(ProcessTemplateStep step)
        {
            return new ProcessTemplateStepResponse
            {
                Id = step.Id,
                TemplateId = step.TemplateId,
                StepNo = step.StepNo,
                ProcessId = step.ProcessId,
                ProcessName = step.ProcessName,
                IsMandatory = step.IsMandatory,
                CanBeParallel = step.CanBeParallel
            };
        }

        #endregion
    }
}
