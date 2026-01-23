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
                // Business Rule 1: Validate template name is unique
                var exists = await _processTemplateRepository.ExistsAsync(request.TemplateName);
                if (exists)
                {
                    return ApiResponse<int>.ErrorResponse($"Process template '{request.TemplateName}' already exists");
                }

                // Business Rule 2: Validate required fields
                if (string.IsNullOrWhiteSpace(request.TemplateName))
                {
                    return ApiResponse<int>.ErrorResponse("Template name is required");
                }

                // Create Template
                var template = new ProcessTemplate
                {
                    TemplateName = request.TemplateName.Trim(),
                    Description = request.Description?.Trim(),
                    ProductId = request.ProductId,
                    ProductCode = request.ProductCode?.Trim(),
                    ProductName = request.ProductName?.Trim(),
                    ChildPartId = request.ChildPartId,
                    ChildPartName = request.ChildPartName?.Trim(),
                    TemplateType = request.TemplateType?.Trim() ?? "Standard",
                    IsActive = request.IsActive,
                    Status = request.Status?.Trim() ?? "Active",
                    IsDefault = request.IsDefault,
                    ApprovedBy = request.ApprovedBy?.Trim(),
                    ApprovalDate = request.ApprovalDate,
                    Remarks = request.Remarks?.Trim(),
                    CreatedBy = request.CreatedBy?.Trim() ?? "System"
                };

                var templateId = await _processTemplateRepository.InsertAsync(template);

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

                // Business Rule 1: Validate template name uniqueness if changed
                if (existingTemplate.TemplateName != request.TemplateName)
                {
                    var exists = await _processTemplateRepository.ExistsAsync(request.TemplateName);
                    if (exists)
                    {
                        return ApiResponse<bool>.ErrorResponse($"Process template '{request.TemplateName}' already exists");
                    }
                }

                // Update template
                existingTemplate.TemplateName = request.TemplateName.Trim();
                existingTemplate.Description = request.Description?.Trim();
                existingTemplate.ProductId = request.ProductId;
                existingTemplate.ProductCode = request.ProductCode?.Trim();
                existingTemplate.ProductName = request.ProductName?.Trim();
                existingTemplate.ChildPartId = request.ChildPartId;
                existingTemplate.ChildPartName = request.ChildPartName?.Trim();
                existingTemplate.TemplateType = request.TemplateType?.Trim();
                existingTemplate.IsActive = request.IsActive;
                existingTemplate.Status = request.Status?.Trim();
                existingTemplate.IsDefault = request.IsDefault;
                existingTemplate.ApprovedBy = request.ApprovedBy?.Trim();
                existingTemplate.ApprovalDate = request.ApprovalDate;
                existingTemplate.Remarks = request.Remarks?.Trim();
                existingTemplate.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";

                var success = await _processTemplateRepository.UpdateAsync(existingTemplate);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Process template '{request.TemplateName}' updated successfully")
                    : ApiResponse<bool>.ErrorResponse("Failed to update process template");
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
                    ProcessCode = request.ProcessCode?.Trim(),
                    ProcessName = request.ProcessName?.Trim(),
                    DefaultMachineId = request.DefaultMachineId,
                    DefaultMachineName = request.DefaultMachineName?.Trim(),
                    MachineType = request.MachineType?.Trim(),
                    SetupTimeMin = request.SetupTimeMin,
                    CycleTimeMin = request.CycleTimeMin,
                    CycleTimePerPiece = request.CycleTimePerPiece,
                    IsParallel = request.IsParallel,
                    ParallelGroupNo = request.ParallelGroupNo,
                    DependsOnSteps = request.DependsOnSteps?.Trim(),
                    RequiresQC = request.RequiresQC,
                    QCCheckpoints = request.QCCheckpoints?.Trim(),
                    WorkInstructions = request.WorkInstructions?.Trim(),
                    SafetyInstructions = request.SafetyInstructions?.Trim(),
                    ToolingRequired = request.ToolingRequired?.Trim(),
                    Remarks = request.Remarks?.Trim()
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
                existingStep.ProcessCode = request.ProcessCode?.Trim();
                existingStep.ProcessName = request.ProcessName?.Trim();
                existingStep.DefaultMachineId = request.DefaultMachineId;
                existingStep.DefaultMachineName = request.DefaultMachineName?.Trim();
                existingStep.MachineType = request.MachineType?.Trim();
                existingStep.SetupTimeMin = request.SetupTimeMin;
                existingStep.CycleTimeMin = request.CycleTimeMin;
                existingStep.CycleTimePerPiece = request.CycleTimePerPiece;
                existingStep.IsParallel = request.IsParallel;
                existingStep.ParallelGroupNo = request.ParallelGroupNo;
                existingStep.DependsOnSteps = request.DependsOnSteps?.Trim();
                existingStep.RequiresQC = request.RequiresQC;
                existingStep.QCCheckpoints = request.QCCheckpoints?.Trim();
                existingStep.WorkInstructions = request.WorkInstructions?.Trim();
                existingStep.SafetyInstructions = request.SafetyInstructions?.Trim();
                existingStep.ToolingRequired = request.ToolingRequired?.Trim();
                existingStep.Remarks = request.Remarks?.Trim();

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

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetByProductIdAsync(int productId)
        {
            try
            {
                var templates = await _processTemplateRepository.GetByProductIdAsync(productId);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving templates by product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetByChildPartIdAsync(int childPartId)
        {
            try
            {
                var templates = await _processTemplateRepository.GetByChildPartIdAsync(childPartId);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving templates by child part: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetByTemplateTypeAsync(string templateType)
        {
            try
            {
                var templates = await _processTemplateRepository.GetByTemplateTypeAsync(templateType);
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving templates by type: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProcessTemplateResponse>>> GetDefaultTemplatesAsync()
        {
            try
            {
                var templates = await _processTemplateRepository.GetDefaultTemplatesAsync();
                var responses = templates.Select(MapToTemplateResponse).ToList();
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProcessTemplateResponse>>.ErrorResponse($"Error retrieving default templates: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ApproveTemplateAsync(int id, string approvedBy)
        {
            try
            {
                var template = await _processTemplateRepository.GetByIdAsync(id);
                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Process template not found");
                }

                var success = await _processTemplateRepository.ApproveTemplateAsync(id, approvedBy);

                return success
                    ? ApiResponse<bool>.SuccessResponse(true, $"Process template approved by {approvedBy}")
                    : ApiResponse<bool>.ErrorResponse("Failed to approve process template");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error approving template: {ex.Message}");
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
                ProductId = template.ProductId,
                ProductCode = template.ProductCode,
                ProductName = template.ProductName,
                ChildPartId = template.ChildPartId,
                ChildPartName = template.ChildPartName,
                TemplateType = template.TemplateType,
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

        private static ProcessTemplateStepResponse MapToStepResponse(ProcessTemplateStep step)
        {
            return new ProcessTemplateStepResponse
            {
                Id = step.Id,
                TemplateId = step.TemplateId,
                StepNo = step.StepNo,
                ProcessId = step.ProcessId,
                ProcessCode = step.ProcessCode,
                ProcessName = step.ProcessName,
                DefaultMachineId = step.DefaultMachineId,
                DefaultMachineName = step.DefaultMachineName,
                MachineType = step.MachineType,
                SetupTimeMin = step.SetupTimeMin,
                CycleTimeMin = step.CycleTimeMin,
                CycleTimePerPiece = step.CycleTimePerPiece,
                IsParallel = step.IsParallel,
                ParallelGroupNo = step.ParallelGroupNo,
                DependsOnSteps = step.DependsOnSteps,
                RequiresQC = step.RequiresQC,
                QCCheckpoints = step.QCCheckpoints,
                WorkInstructions = step.WorkInstructions,
                SafetyInstructions = step.SafetyInstructions,
                ToolingRequired = step.ToolingRequired,
                Remarks = step.Remarks,
                CreatedAt = step.CreatedAt
            };
        }

        #endregion
    }
}
