using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Production;
using MultiHitechERP.API.Repositories.Interfaces;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Services.Implementations
{
    /// <summary>
    /// ProductionService implementation with comprehensive execution logic
    /// Validates machine/operator availability and enforces production workflow
    /// </summary>
    public class ProductionService : IProductionService
    {
        private readonly IJobCardExecutionRepository _executionRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IMachineRepository _machineRepository;
        private readonly IOperatorRepository _operatorRepository;

        public ProductionService(
            IJobCardExecutionRepository executionRepository,
            IJobCardRepository jobCardRepository,
            IMachineRepository machineRepository,
            IOperatorRepository operatorRepository)
        {
            _executionRepository = executionRepository;
            _jobCardRepository = jobCardRepository;
            _machineRepository = machineRepository;
            _operatorRepository = operatorRepository;
        }

        public async Task<ApiResponse<JobCardExecution>> GetByIdAsync(int id)
        {
            var execution = await _executionRepository.GetByIdAsync(id);
            if (execution == null)
                return ApiResponse<JobCardExecution>.ErrorResponse("Execution record not found");

            return ApiResponse<JobCardExecution>.SuccessResponse(execution);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetAllAsync()
        {
            var executions = await _executionRepository.GetAllAsync();
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByJobCardIdAsync(int jobCardId)
        {
            var executions = await _executionRepository.GetByJobCardIdAsync(jobCardId);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByMachineIdAsync(int machineId)
        {
            var executions = await _executionRepository.GetByMachineIdAsync(machineId);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByOperatorIdAsync(int operatorId)
        {
            var executions = await _executionRepository.GetByOperatorIdAsync(operatorId);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<int>> CreateExecutionAsync(JobCardExecution execution)
        {
            var id = await _executionRepository.InsertAsync(execution);
            return ApiResponse<int>.SuccessResponse(id, "Execution record created successfully");
        }

        public async Task<ApiResponse<bool>> UpdateExecutionAsync(JobCardExecution execution)
        {
            var existing = await _executionRepository.GetByIdAsync(execution.Id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            var success = await _executionRepository.UpdateAsync(execution);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update execution record");

            return ApiResponse<bool>.SuccessResponse(true, "Execution record updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteExecutionAsync(int id)
        {
            var existing = await _executionRepository.GetByIdAsync(id);
            if (existing == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            var success = await _executionRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to delete execution record");

            return ApiResponse<bool>.SuccessResponse(true, "Execution record deleted successfully");
        }

        public async Task<ApiResponse<int>> StartProductionAsync(int jobCardId, int machineId, int operatorId, int quantityStarted)
        {
            // Validate job card exists and is ready
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return ApiResponse<int>.ErrorResponse("Job card not found");

            if (jobCard.Status != "Ready" && jobCard.Status != "In Progress")
                return ApiResponse<int>.ErrorResponse($"Cannot start production - job card status is '{jobCard.Status}'");

            // Check if job card already has active execution
            var currentExecution = await _executionRepository.GetCurrentExecutionForJobCardAsync(jobCardId);
            if (currentExecution != null)
                return ApiResponse<int>.ErrorResponse("Job card already has an active execution");

            // Validate machine availability
            var machine = await _machineRepository.GetByIdAsync(machineId);
            if (machine == null)
                return ApiResponse<int>.ErrorResponse("Machine not found");

            // Validate operator availability
            var operatorEntity = await _operatorRepository.GetByIdAsync(operatorId);
            if (operatorEntity == null)
                return ApiResponse<int>.ErrorResponse("Operator not found");

            if (!operatorEntity.IsAvailable)
                return ApiResponse<int>.ErrorResponse($"Operator '{operatorEntity.OperatorName}' is not available");

            // Create execution record
            var execution = new JobCardExecution
            {
                JobCardId = jobCardId,
                JobCardNo = jobCard.JobCardNo,
                OrderNo = jobCard.OrderNo,
                MachineId = machineId,
                MachineName = machine.MachineName,
                OperatorId = operatorId,
                OperatorName = operatorEntity.OperatorName,
                StartTime = DateTime.UtcNow,
                QuantityStarted = quantityStarted,
                ExecutionStatus = "Started"
            };

            var executionId = await _executionRepository.InsertAsync(execution);

            // Update job card status to In Progress
            await _jobCardRepository.UpdateStatusAsync(jobCardId, "In Progress");

            // Mark operator as unavailable
            await _operatorRepository.AssignToJobCardAsync(operatorId, jobCardId, jobCard.JobCardNo, machineId);

            return ApiResponse<int>.SuccessResponse(executionId, "Production started successfully");
        }

        public async Task<ApiResponse<bool>> PauseProductionAsync(int executionId)
        {
            var execution = await _executionRepository.GetByIdAsync(executionId);
            if (execution == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            if (execution.ExecutionStatus != "Started" && execution.ExecutionStatus != "InProgress")
                return ApiResponse<bool>.ErrorResponse($"Cannot pause - execution status is '{execution.ExecutionStatus}'");

            var success = await _executionRepository.PauseExecutionAsync(executionId, DateTime.UtcNow);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to pause production");

            return ApiResponse<bool>.SuccessResponse(true, "Production paused successfully");
        }

        public async Task<ApiResponse<bool>> ResumeProductionAsync(int executionId)
        {
            var execution = await _executionRepository.GetByIdAsync(executionId);
            if (execution == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            if (execution.ExecutionStatus != "Paused")
                return ApiResponse<bool>.ErrorResponse($"Cannot resume - execution status is '{execution.ExecutionStatus}'");

            var success = await _executionRepository.ResumeExecutionAsync(executionId, DateTime.UtcNow);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to resume production");

            return ApiResponse<bool>.SuccessResponse(true, "Production resumed successfully");
        }

        public async Task<ApiResponse<bool>> CompleteProductionAsync(int executionId, int quantityCompleted, int? quantityRejected)
        {
            var execution = await _executionRepository.GetByIdAsync(executionId);
            if (execution == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            if (execution.ExecutionStatus == "Completed")
                return ApiResponse<bool>.ErrorResponse("Execution is already completed");

            // Calculate total time
            var totalTimeMin = (int)(DateTime.UtcNow - execution.StartTime).TotalMinutes;
            if (execution.IdleTimeMin.HasValue)
                totalTimeMin -= execution.IdleTimeMin.Value;

            // Update execution record
            var success = await _executionRepository.CompleteExecutionAsync(executionId, DateTime.UtcNow, totalTimeMin);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to complete production");

            // Update quantities
            await _executionRepository.UpdateQuantitiesAsync(executionId, quantityCompleted, quantityRejected ?? 0, 0);

            // Update job card status
            var jobCard = await _jobCardRepository.GetByIdAsync(execution.JobCardId);
            if (jobCard != null)
            {
                var totalCompleted = await _executionRepository.GetTotalCompletedQuantityForJobCardAsync(execution.JobCardId);

                // If all quantity completed, mark job card as completed
                if (totalCompleted >= jobCard.Quantity)
                {
                    await _jobCardRepository.UpdateStatusAsync(execution.JobCardId, "Completed");
                }
            }

            // Release operator
            if (execution.OperatorId.HasValue)
                await _operatorRepository.ReleaseFromJobCardAsync(execution.OperatorId.Value);

            return ApiResponse<bool>.SuccessResponse(true, "Production completed successfully");
        }

        public async Task<ApiResponse<bool>> UpdateQuantitiesAsync(int executionId, int? completed, int? rejected, int? inProgress)
        {
            var execution = await _executionRepository.GetByIdAsync(executionId);
            if (execution == null)
                return ApiResponse<bool>.ErrorResponse("Execution record not found");

            var success = await _executionRepository.UpdateQuantitiesAsync(executionId, completed, rejected, inProgress);
            if (!success)
                return ApiResponse<bool>.ErrorResponse("Failed to update quantities");

            return ApiResponse<bool>.SuccessResponse(true, "Quantities updated successfully");
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetActiveExecutionsAsync()
        {
            var executions = await _executionRepository.GetActiveExecutionsAsync();
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return ApiResponse<IEnumerable<JobCardExecution>>.ErrorResponse("Status is required");

            var executions = await _executionRepository.GetByStatusAsync(status);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return ApiResponse<IEnumerable<JobCardExecution>>.ErrorResponse("Start date must be before end date");

            var executions = await _executionRepository.GetByDateRangeAsync(startDate, endDate);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<JobCardExecution>> GetCurrentExecutionForJobCardAsync(int jobCardId)
        {
            var execution = await _executionRepository.GetCurrentExecutionForJobCardAsync(jobCardId);
            if (execution == null)
                return ApiResponse<JobCardExecution>.ErrorResponse("No active execution found for this job card");

            return ApiResponse<JobCardExecution>.SuccessResponse(execution);
        }

        public async Task<ApiResponse<IEnumerable<JobCardExecution>>> GetExecutionHistoryForJobCardAsync(int jobCardId)
        {
            var executions = await _executionRepository.GetExecutionHistoryForJobCardAsync(jobCardId);
            return ApiResponse<IEnumerable<JobCardExecution>>.SuccessResponse(executions);
        }

        public async Task<ApiResponse<int>> GetTotalExecutionTimeForJobCardAsync(int jobCardId)
        {
            var totalTime = await _executionRepository.GetTotalExecutionTimeForJobCardAsync(jobCardId);
            return ApiResponse<int>.SuccessResponse(totalTime);
        }

        public async Task<ApiResponse<int>> GetTotalCompletedQuantityForJobCardAsync(int jobCardId)
        {
            var totalCompleted = await _executionRepository.GetTotalCompletedQuantityForJobCardAsync(jobCardId);
            return ApiResponse<int>.SuccessResponse(totalCompleted);
        }
    }
}
