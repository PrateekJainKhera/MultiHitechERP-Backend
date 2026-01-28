using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Planning;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    public class JobCardService : IJobCardService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardDependencyRepository _dependencyRepository;

        public JobCardService(IJobCardRepository jobCardRepository, IJobCardDependencyRepository dependencyRepository)
        {
            _jobCardRepository = jobCardRepository;
            _dependencyRepository = dependencyRepository;
        }

        public async Task<ApiResponse<JobCardResponse>> GetByIdAsync(int id)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<JobCardResponse>.ErrorResponse($"Job card with ID {id} not found");

                return ApiResponse<JobCardResponse>.SuccessResponse(MapToResponse(jobCard));
            }
            catch (Exception ex)
            {
                return ApiResponse<JobCardResponse>.ErrorResponse($"Error retrieving job card: {ex.Message}");
            }
        }

        public async Task<ApiResponse<JobCardResponse>> GetByJobCardNoAsync(string jobCardNo)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByJobCardNoAsync(jobCardNo);
                if (jobCard == null)
                    return ApiResponse<JobCardResponse>.ErrorResponse($"Job card {jobCardNo} not found");

                return ApiResponse<JobCardResponse>.SuccessResponse(MapToResponse(jobCard));
            }
            catch (Exception ex)
            {
                return ApiResponse<JobCardResponse>.ErrorResponse($"Error retrieving job card: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetAllAsync()
        {
            try
            {
                var jobCards = await _jobCardRepository.GetAllAsync();
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOrderIdAsync(int orderId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetByOrderIdAsync(orderId);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByProcessIdAsync(int processId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetByProcessIdAsync(processId);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByStatusAsync(string status)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetByStatusAsync(status);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<int>> CreateJobCardAsync(CreateJobCardRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.JobCardNo))
                    return ApiResponse<int>.ErrorResponse("Job card number is required");

                if (request.Quantity < 1)
                    return ApiResponse<int>.ErrorResponse("Quantity must be at least 1");

                var existing = await _jobCardRepository.GetByJobCardNoAsync(request.JobCardNo);
                if (existing != null)
                    return ApiResponse<int>.ErrorResponse($"Job card {request.JobCardNo} already exists");

                var jobCard = new JobCard
                {
                    JobCardNo = request.JobCardNo.Trim().ToUpper(),
                    CreationType = request.CreationType,
                    OrderId = request.OrderId,
                    OrderNo = request.OrderNo?.Trim(),
                    DrawingId = request.DrawingId,
                    DrawingNumber = request.DrawingNumber?.Trim(),
                    DrawingRevision = request.DrawingRevision?.Trim(),
                    DrawingName = request.DrawingName?.Trim(),
                    DrawingSelectionType = request.DrawingSelectionType,
                    ChildPartId = request.ChildPartId,
                    ChildPartName = request.ChildPartName?.Trim(),
                    ChildPartTemplateId = request.ChildPartTemplateId,
                    ProcessId = request.ProcessId,
                    ProcessName = request.ProcessName?.Trim(),
                    ProcessCode = request.ProcessCode?.Trim(),
                    StepNo = request.StepNo,
                    ProcessTemplateId = request.ProcessTemplateId,
                    WorkInstructions = request.WorkInstructions?.Trim(),
                    QualityCheckpoints = request.QualityCheckpoints?.Trim(),
                    SpecialNotes = request.SpecialNotes?.Trim(),
                    Quantity = request.Quantity,
                    Status = "Pending",
                    Priority = request.Priority,
                    ManufacturingDimensions = request.ManufacturingDimensions,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System",
                    Version = 1
                };

                var jobCardId = await _jobCardRepository.InsertAsync(jobCard);

                // Create dependencies if specified
                if (request.PrerequisiteJobCardIds != null && request.PrerequisiteJobCardIds.Any())
                {
                    foreach (var prerequisiteId in request.PrerequisiteJobCardIds)
                    {
                        var wouldCreateCircular = await _dependencyRepository.WouldCreateCircularDependencyAsync(jobCardId, prerequisiteId);
                        if (wouldCreateCircular)
                        {
                            await _jobCardRepository.DeleteAsync(jobCardId);
                            return ApiResponse<int>.ErrorResponse($"Cannot create dependency - would create circular dependency with job card {prerequisiteId}");
                        }

                        var prerequisite = await _jobCardRepository.GetByIdAsync(prerequisiteId);
                        if (prerequisite == null)
                        {
                            await _jobCardRepository.DeleteAsync(jobCardId);
                            return ApiResponse<int>.ErrorResponse($"Prerequisite job card {prerequisiteId} not found");
                        }

                        var dependency = new JobCardDependency
                        {
                            DependentJobCardId = jobCardId,
                            DependentJobCardNo = request.JobCardNo,
                            PrerequisiteJobCardId = prerequisiteId,
                            PrerequisiteJobCardNo = prerequisite.JobCardNo,
                            DependencyType = "Sequential",
                            IsResolved = prerequisite.Status == "Completed"
                        };

                        await _dependencyRepository.InsertAsync(dependency);
                    }
                }

                return ApiResponse<int>.SuccessResponse(jobCardId, $"Job card '{request.JobCardNo}' created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating job card: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateJobCardAsync(UpdateJobCardRequest request)
        {
            try
            {
                var existingJobCard = await _jobCardRepository.GetByIdAsync(request.Id);
                if (existingJobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                if (existingJobCard.Version != request.Version)
                    return ApiResponse<bool>.ErrorResponse("Job card has been modified by another user. Please refresh and try again.");

                existingJobCard.JobCardNo = request.JobCardNo.Trim().ToUpper();
                existingJobCard.CreationType = request.CreationType;
                existingJobCard.OrderId = request.OrderId;
                existingJobCard.OrderNo = request.OrderNo?.Trim();
                existingJobCard.DrawingId = request.DrawingId;
                existingJobCard.DrawingNumber = request.DrawingNumber?.Trim();
                existingJobCard.DrawingRevision = request.DrawingRevision?.Trim();
                existingJobCard.DrawingName = request.DrawingName?.Trim();
                existingJobCard.DrawingSelectionType = request.DrawingSelectionType;
                existingJobCard.ChildPartId = request.ChildPartId;
                existingJobCard.ChildPartName = request.ChildPartName?.Trim();
                existingJobCard.ChildPartTemplateId = request.ChildPartTemplateId;
                existingJobCard.ProcessId = request.ProcessId;
                existingJobCard.ProcessName = request.ProcessName?.Trim();
                existingJobCard.ProcessCode = request.ProcessCode?.Trim();
                existingJobCard.StepNo = request.StepNo;
                existingJobCard.ProcessTemplateId = request.ProcessTemplateId;
                existingJobCard.WorkInstructions = request.WorkInstructions?.Trim();
                existingJobCard.QualityCheckpoints = request.QualityCheckpoints?.Trim();
                existingJobCard.SpecialNotes = request.SpecialNotes?.Trim();
                existingJobCard.Quantity = request.Quantity;
                existingJobCard.Status = request.Status;
                existingJobCard.Priority = request.Priority;
                existingJobCard.ManufacturingDimensions = request.ManufacturingDimensions;
                existingJobCard.UpdatedBy = request.UpdatedBy?.Trim() ?? "System";
                existingJobCard.Version = request.Version + 1;

                var success = await _jobCardRepository.UpdateAsync(existingJobCard);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update job card");

                return ApiResponse<bool>.SuccessResponse(true, "Job card updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating job card: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteJobCardAsync(int id)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                if (jobCard.Status == "In Progress" || jobCard.Status == "Completed")
                    return ApiResponse<bool>.ErrorResponse("Cannot delete job card that has been started or completed");

                var dependents = await _jobCardRepository.GetDependentJobCardsAsync(id);
                if (dependents.Any())
                    return ApiResponse<bool>.ErrorResponse($"Cannot delete job card - {dependents.Count()} other job cards depend on it");

                var success = await _jobCardRepository.DeleteAsync(id);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to delete job card");

                return ApiResponse<bool>.SuccessResponse(true, "Job card deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting job card: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int id, string status)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var success = await _jobCardRepository.UpdateStatusAsync(id, status);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update job card status");

                return ApiResponse<bool>.SuccessResponse(true, $"Job card status updated to '{status}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetDependentJobCardsAsync(int jobCardId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetDependentJobCardsAsync(jobCardId);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving dependent job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetPrerequisiteJobCardsAsync(int jobCardId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetPrerequisiteJobCardsAsync(jobCardId);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving prerequisite job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AddDependencyAsync(int dependentJobCardId, int prerequisiteJobCardId)
        {
            try
            {
                var dependentJobCard = await _jobCardRepository.GetByIdAsync(dependentJobCardId);
                if (dependentJobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Dependent job card not found");

                var prerequisiteJobCard = await _jobCardRepository.GetByIdAsync(prerequisiteJobCardId);
                if (prerequisiteJobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Prerequisite job card not found");

                var wouldCreateCircular = await _dependencyRepository.WouldCreateCircularDependencyAsync(dependentJobCardId, prerequisiteJobCardId);
                if (wouldCreateCircular)
                    return ApiResponse<bool>.ErrorResponse("Cannot create dependency - would create circular dependency");

                var dependency = new JobCardDependency
                {
                    DependentJobCardId = dependentJobCardId,
                    DependentJobCardNo = dependentJobCard.JobCardNo,
                    PrerequisiteJobCardId = prerequisiteJobCardId,
                    PrerequisiteJobCardNo = prerequisiteJobCard.JobCardNo,
                    DependencyType = "Sequential",
                    IsResolved = prerequisiteJobCard.Status == "Completed"
                };

                await _dependencyRepository.InsertAsync(dependency);

                return ApiResponse<bool>.SuccessResponse(true, "Dependency added successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error adding dependency: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> RemoveDependencyAsync(int dependencyId)
        {
            try
            {
                var dependency = await _dependencyRepository.GetByIdAsync(dependencyId);
                if (dependency == null)
                    return ApiResponse<bool>.ErrorResponse("Dependency not found");

                var success = await _dependencyRepository.DeleteAsync(dependencyId);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to remove dependency");

                return ApiResponse<bool>.SuccessResponse(true, "Dependency removed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error removing dependency: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetBlockedJobCardsAsync()
        {
            try
            {
                var jobCards = await _jobCardRepository.GetBlockedJobCardsAsync();
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving blocked job cards: {ex.Message}");
            }
        }

        private static JobCardResponse MapToResponse(JobCard jobCard)
        {
            return new JobCardResponse
            {
                Id = jobCard.Id,
                JobCardNo = jobCard.JobCardNo,
                CreationType = jobCard.CreationType,
                OrderId = jobCard.OrderId,
                OrderNo = jobCard.OrderNo,
                DrawingId = jobCard.DrawingId,
                DrawingNumber = jobCard.DrawingNumber,
                DrawingRevision = jobCard.DrawingRevision,
                DrawingName = jobCard.DrawingName,
                DrawingSelectionType = jobCard.DrawingSelectionType,
                ChildPartId = jobCard.ChildPartId,
                ChildPartName = jobCard.ChildPartName,
                ChildPartTemplateId = jobCard.ChildPartTemplateId,
                ProcessId = jobCard.ProcessId,
                ProcessName = jobCard.ProcessName,
                ProcessCode = jobCard.ProcessCode,
                StepNo = jobCard.StepNo,
                ProcessTemplateId = jobCard.ProcessTemplateId,
                WorkInstructions = jobCard.WorkInstructions,
                QualityCheckpoints = jobCard.QualityCheckpoints,
                SpecialNotes = jobCard.SpecialNotes,
                Quantity = jobCard.Quantity,
                Status = jobCard.Status,
                Priority = jobCard.Priority,
                ManufacturingDimensions = jobCard.ManufacturingDimensions,
                CreatedAt = jobCard.CreatedAt,
                CreatedBy = jobCard.CreatedBy,
                UpdatedAt = jobCard.UpdatedAt,
                UpdatedBy = jobCard.UpdatedBy,
                Version = jobCard.Version
            };
        }
    }
}
