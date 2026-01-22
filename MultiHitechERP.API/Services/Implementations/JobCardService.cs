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

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetReadyForSchedulingAsync()
        {
            try
            {
                var jobCards = await _jobCardRepository.GetReadyForSchedulingAsync();
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving ready job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetScheduledJobCardsAsync()
        {
            try
            {
                var jobCards = await _jobCardRepository.GetScheduledJobCardsAsync();
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving scheduled job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetInProgressJobCardsAsync()
        {
            try
            {
                var jobCards = await _jobCardRepository.GetInProgressJobCardsAsync();
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving in-progress job cards: {ex.Message}");
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

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByMachineIdAsync(int machineId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetByMachineIdAsync(machineId);
                var responses = jobCards.Select(MapToResponse).ToList();
                return ApiResponse<IEnumerable<JobCardResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<JobCardResponse>>.ErrorResponse($"Error retrieving job cards: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<JobCardResponse>>> GetByOperatorIdAsync(int operatorId)
        {
            try
            {
                var jobCards = await _jobCardRepository.GetByOperatorIdAsync(operatorId);
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
                // Validation
                if (string.IsNullOrWhiteSpace(request.JobCardNo))
                    return ApiResponse<int>.ErrorResponse("Job card number is required");

                if (request.Quantity < 1)
                    return ApiResponse<int>.ErrorResponse("Quantity must be at least 1");

                // Check for duplicate job card number
                var existing = await _jobCardRepository.GetByJobCardNoAsync(request.JobCardNo);
                if (existing != null)
                    return ApiResponse<int>.ErrorResponse($"Job card {request.JobCardNo} already exists");

                // Map to entity
                var jobCard = new JobCard
                {
                    JobCardNo = request.JobCardNo.Trim().ToUpper(),
                    CreationType = request.CreationType,
                    OrderId = request.OrderId,
                    OrderNo = request.OrderNo?.Trim(),
                    DrawingId = request.DrawingId,
                    DrawingNumber = request.DrawingNumber?.Trim(),
                    DrawingRevision = request.DrawingRevision?.Trim(),
                    DrawingSelectionType = request.DrawingSelectionType,
                    ChildPartId = request.ChildPartId,
                    ChildPartName = request.ChildPartName?.Trim(),
                    ChildPartTemplateId = request.ChildPartTemplateId,
                    ProcessId = request.ProcessId,
                    ProcessName = request.ProcessName?.Trim(),
                    StepNo = request.StepNo,
                    ProcessTemplateId = request.ProcessTemplateId,
                    Quantity = request.Quantity,
                    CompletedQty = 0,
                    RejectedQty = 0,
                    ReworkQty = 0,
                    InProgressQty = 0,
                    Status = "Pending",
                    AssignedMachineId = request.AssignedMachineId,
                    AssignedMachineName = request.AssignedMachineName?.Trim(),
                    AssignedOperatorId = request.AssignedOperatorId,
                    AssignedOperatorName = request.AssignedOperatorName?.Trim(),
                    EstimatedSetupTimeMin = request.EstimatedSetupTimeMin,
                    EstimatedCycleTimeMin = request.EstimatedCycleTimeMin,
                    EstimatedTotalTimeMin = request.EstimatedTotalTimeMin,
                    MaterialStatus = request.MaterialStatus,
                    ManufacturingDimensions = request.ManufacturingDimensions,
                    Priority = request.Priority,
                    ScheduleStatus = request.ScheduleStatus,
                    ScheduledStartDate = request.ScheduledStartDate,
                    ScheduledEndDate = request.ScheduledEndDate,
                    IsRework = request.IsRework,
                    ReworkOrderId = request.ReworkOrderId,
                    ParentJobCardId = request.ParentJobCardId,
                    CreatedBy = request.CreatedBy?.Trim() ?? "System",
                    Version = 1
                };

                var jobCardId = await _jobCardRepository.InsertAsync(jobCard);

                // Create dependencies if specified
                if (request.PrerequisiteJobCardIds != null && request.PrerequisiteJobCardIds.Any())
                {
                    foreach (var prerequisiteId in request.PrerequisiteJobCardIds)
                    {
                        // Check for circular dependencies
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

                // Optimistic locking check
                if (existingJobCard.Version != request.Version)
                    return ApiResponse<bool>.ErrorResponse($"Job card has been modified by another user. Please refresh and try again.");

                // Update fields
                existingJobCard.JobCardNo = request.JobCardNo.Trim().ToUpper();
                existingJobCard.CreationType = request.CreationType;
                existingJobCard.OrderId = request.OrderId;
                existingJobCard.OrderNo = request.OrderNo?.Trim();
                existingJobCard.DrawingId = request.DrawingId;
                existingJobCard.DrawingNumber = request.DrawingNumber?.Trim();
                existingJobCard.DrawingRevision = request.DrawingRevision?.Trim();
                existingJobCard.DrawingSelectionType = request.DrawingSelectionType;
                existingJobCard.ChildPartId = request.ChildPartId;
                existingJobCard.ChildPartName = request.ChildPartName?.Trim();
                existingJobCard.ChildPartTemplateId = request.ChildPartTemplateId;
                existingJobCard.ProcessId = request.ProcessId;
                existingJobCard.ProcessName = request.ProcessName?.Trim();
                existingJobCard.StepNo = request.StepNo;
                existingJobCard.ProcessTemplateId = request.ProcessTemplateId;
                existingJobCard.Quantity = request.Quantity;
                existingJobCard.CompletedQty = request.CompletedQty;
                existingJobCard.RejectedQty = request.RejectedQty;
                existingJobCard.ReworkQty = request.ReworkQty;
                existingJobCard.InProgressQty = request.InProgressQty;
                existingJobCard.Status = request.Status;
                existingJobCard.AssignedMachineId = request.AssignedMachineId;
                existingJobCard.AssignedMachineName = request.AssignedMachineName?.Trim();
                existingJobCard.AssignedOperatorId = request.AssignedOperatorId;
                existingJobCard.AssignedOperatorName = request.AssignedOperatorName?.Trim();
                existingJobCard.EstimatedSetupTimeMin = request.EstimatedSetupTimeMin;
                existingJobCard.EstimatedCycleTimeMin = request.EstimatedCycleTimeMin;
                existingJobCard.EstimatedTotalTimeMin = request.EstimatedTotalTimeMin;
                existingJobCard.ActualStartTime = request.ActualStartTime;
                existingJobCard.ActualEndTime = request.ActualEndTime;
                existingJobCard.ActualTimeMin = request.ActualTimeMin;
                existingJobCard.MaterialStatus = request.MaterialStatus;
                existingJobCard.ManufacturingDimensions = request.ManufacturingDimensions;
                existingJobCard.Priority = request.Priority;
                existingJobCard.ScheduleStatus = request.ScheduleStatus;
                existingJobCard.ScheduledStartDate = request.ScheduledStartDate;
                existingJobCard.ScheduledEndDate = request.ScheduledEndDate;
                existingJobCard.IsRework = request.IsRework;
                existingJobCard.ReworkOrderId = request.ReworkOrderId;
                existingJobCard.ParentJobCardId = request.ParentJobCardId;
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

                // Check if job card has started
                if (jobCard.Status == "In Progress" || jobCard.Status == "Completed")
                    return ApiResponse<bool>.ErrorResponse("Cannot delete job card that has been started or completed");

                // Check if other job cards depend on this one
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

        public async Task<ApiResponse<bool>> UpdateMaterialStatusAsync(int id, string materialStatus)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var success = await _jobCardRepository.UpdateMaterialStatusAsync(id, materialStatus);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update material status");

                return ApiResponse<bool>.SuccessResponse(true, $"Material status updated to '{materialStatus}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating material status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateScheduleStatusAsync(int id, string scheduleStatus, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var success = await _jobCardRepository.UpdateScheduleStatusAsync(id, scheduleStatus, startDate, endDate);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update schedule status");

                return ApiResponse<bool>.SuccessResponse(true, $"Schedule status updated to '{scheduleStatus}'");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating schedule status: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignMachineAsync(int id, int machineId, string machineName)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var success = await _jobCardRepository.AssignMachineAsync(id, machineId, machineName);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to assign machine");

                return ApiResponse<bool>.SuccessResponse(true, $"Machine '{machineName}' assigned successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error assigning machine: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> AssignOperatorAsync(int id, int operatorId, string operatorName)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var success = await _jobCardRepository.AssignOperatorAsync(id, operatorId, operatorName);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to assign operator");

                return ApiResponse<bool>.SuccessResponse(true, $"Operator '{operatorName}' assigned successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error assigning operator: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> StartExecutionAsync(int id)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                if (jobCard.Status == "In Progress")
                    return ApiResponse<bool>.ErrorResponse("Job card is already in progress");

                if (jobCard.Status == "Completed")
                    return ApiResponse<bool>.ErrorResponse("Job card is already completed");

                // Check dependencies
                var hasUnresolved = await _jobCardRepository.HasUnresolvedDependenciesAsync(id);
                if (hasUnresolved)
                    return ApiResponse<bool>.ErrorResponse("Cannot start - job card has unresolved dependencies");

                // Check material availability
                if (jobCard.MaterialStatus != "Available" && jobCard.MaterialStatus != "Issued")
                    return ApiResponse<bool>.ErrorResponse($"Cannot start - material status is '{jobCard.MaterialStatus}'");

                var success = await _jobCardRepository.StartExecutionAsync(id, DateTime.UtcNow);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to start execution");

                return ApiResponse<bool>.SuccessResponse(true, "Job card execution started");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error starting execution: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CompleteExecutionAsync(int id)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                if (jobCard.Status != "In Progress")
                    return ApiResponse<bool>.ErrorResponse("Job card is not in progress");

                if (!jobCard.ActualStartTime.HasValue)
                    return ApiResponse<bool>.ErrorResponse("Job card has no start time");

                var endTime = DateTime.UtcNow;
                var actualTimeMin = (int)(endTime - jobCard.ActualStartTime.Value).TotalMinutes;

                var success = await _jobCardRepository.CompleteExecutionAsync(id, endTime, actualTimeMin);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to complete execution");

                return ApiResponse<bool>.SuccessResponse(true, "Job card execution completed");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error completing execution: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateQuantitiesAsync(int id, int completedQty, int rejectedQty, int reworkQty, int inProgressQty)
        {
            try
            {
                var jobCard = await _jobCardRepository.GetByIdAsync(id);
                if (jobCard == null)
                    return ApiResponse<bool>.ErrorResponse("Job card not found");

                var totalQty = completedQty + rejectedQty + reworkQty + inProgressQty;
                if (totalQty > jobCard.Quantity)
                    return ApiResponse<bool>.ErrorResponse($"Total quantities ({totalQty}) exceed job card quantity ({jobCard.Quantity})");

                var success = await _jobCardRepository.UpdateQuantitiesAsync(id, completedQty, rejectedQty, reworkQty, inProgressQty);
                if (!success)
                    return ApiResponse<bool>.ErrorResponse("Failed to update quantities");

                return ApiResponse<bool>.SuccessResponse(true, "Quantities updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating quantities: {ex.Message}");
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

                // Check for circular dependency
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
                DrawingSelectionType = jobCard.DrawingSelectionType,
                ChildPartId = jobCard.ChildPartId,
                ChildPartName = jobCard.ChildPartName,
                ChildPartTemplateId = jobCard.ChildPartTemplateId,
                ProcessId = jobCard.ProcessId,
                ProcessName = jobCard.ProcessName,
                StepNo = jobCard.StepNo,
                ProcessTemplateId = jobCard.ProcessTemplateId,
                Quantity = jobCard.Quantity,
                CompletedQty = jobCard.CompletedQty,
                RejectedQty = jobCard.RejectedQty,
                ReworkQty = jobCard.ReworkQty,
                InProgressQty = jobCard.InProgressQty,
                Status = jobCard.Status,
                AssignedMachineId = jobCard.AssignedMachineId,
                AssignedMachineName = jobCard.AssignedMachineName,
                AssignedOperatorId = jobCard.AssignedOperatorId,
                AssignedOperatorName = jobCard.AssignedOperatorName,
                EstimatedSetupTimeMin = jobCard.EstimatedSetupTimeMin,
                EstimatedCycleTimeMin = jobCard.EstimatedCycleTimeMin,
                EstimatedTotalTimeMin = jobCard.EstimatedTotalTimeMin,
                ActualStartTime = jobCard.ActualStartTime,
                ActualEndTime = jobCard.ActualEndTime,
                ActualTimeMin = jobCard.ActualTimeMin,
                MaterialStatus = jobCard.MaterialStatus,
                MaterialStatusUpdatedAt = jobCard.MaterialStatusUpdatedAt,
                ManufacturingDimensions = jobCard.ManufacturingDimensions,
                Priority = jobCard.Priority,
                ScheduleStatus = jobCard.ScheduleStatus,
                ScheduledStartDate = jobCard.ScheduledStartDate,
                ScheduledEndDate = jobCard.ScheduledEndDate,
                IsRework = jobCard.IsRework,
                ReworkOrderId = jobCard.ReworkOrderId,
                ParentJobCardId = jobCard.ParentJobCardId,
                CreatedAt = jobCard.CreatedAt,
                CreatedBy = jobCard.CreatedBy,
                UpdatedAt = jobCard.UpdatedAt,
                UpdatedBy = jobCard.UpdatedBy,
                Version = jobCard.Version
            };
        }
    }
}
