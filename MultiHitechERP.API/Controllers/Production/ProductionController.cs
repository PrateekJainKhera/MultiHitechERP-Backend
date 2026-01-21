using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Production
{
    /// <summary>
    /// Production execution management API endpoints
    /// Handles shop floor production tracking and execution
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _service;

        public ProductionController(IProductionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all production executions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get production execution by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<JobCardExecutionResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get executions by job card ID
        /// </summary>
        [HttpGet("by-job-card/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetByJobCardId(Guid jobCardId)
        {
            var response = await _service.GetByJobCardIdAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get executions by machine ID
        /// </summary>
        [HttpGet("by-machine/{machineId:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetByMachineId(Guid machineId)
        {
            var response = await _service.GetByMachineIdAsync(machineId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get executions by operator ID
        /// </summary>
        [HttpGet("by-operator/{operatorId:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetByOperatorId(Guid operatorId)
        {
            var response = await _service.GetByOperatorIdAsync(operatorId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get all active executions (Started, InProgress, Paused)
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetActiveExecutions()
        {
            var response = await _service.GetActiveExecutionsAsync();
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get executions by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetByStatus(string status)
        {
            var response = await _service.GetByStatusAsync(status);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get executions by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var response = await _service.GetByDateRangeAsync(startDate, endDate);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Get current active execution for a job card
        /// </summary>
        [HttpGet("current/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse>>> GetCurrentExecution(Guid jobCardId)
        {
            var response = await _service.GetCurrentExecutionForJobCardAsync(jobCardId);
            if (!response.Success)
                return NotFound(response);

            var dto = MapToResponse(response.Data);
            return Ok(ApiResponse<JobCardExecutionResponse>.SuccessResponse(dto));
        }

        /// <summary>
        /// Get execution history for a job card
        /// </summary>
        [HttpGet("history/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<JobCardExecutionResponse[]>>> GetExecutionHistory(Guid jobCardId)
        {
            var response = await _service.GetExecutionHistoryForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            var dtos = response.Data.Select(MapToResponse).ToArray();
            return Ok(ApiResponse<JobCardExecutionResponse[]>.SuccessResponse(dtos));
        }

        /// <summary>
        /// Start production on a job card
        /// </summary>
        [HttpPost("start")]
        public async Task<ActionResult<ApiResponse<Guid>>> StartProduction([FromBody] StartProductionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<Guid>.ErrorResponse("Invalid request data"));

            var response = await _service.StartProductionAsync(
                request.JobCardId,
                request.MachineId,
                request.OperatorId,
                request.QuantityStarted);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Pause production execution
        /// </summary>
        [HttpPost("{id:guid}/pause")]
        public async Task<ActionResult<ApiResponse<bool>>> PauseProduction(Guid id)
        {
            var response = await _service.PauseProductionAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Resume paused production execution
        /// </summary>
        [HttpPost("{id:guid}/resume")]
        public async Task<ActionResult<ApiResponse<bool>>> ResumeProduction(Guid id)
        {
            var response = await _service.ResumeProductionAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Complete production execution
        /// </summary>
        [HttpPost("{id:guid}/complete")]
        public async Task<ActionResult<ApiResponse<bool>>> CompleteProduction(Guid id, [FromBody] CompleteProductionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.CompleteProductionAsync(
                id,
                request.QuantityCompleted,
                request.QuantityRejected);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update production quantities during execution
        /// </summary>
        [HttpPatch("{id:guid}/quantities")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateQuantities(Guid id, [FromBody] UpdateQuantitiesRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateQuantitiesAsync(
                id,
                request.QuantityCompleted,
                request.QuantityRejected,
                request.QuantityInProgress);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get total execution time for a job card (in minutes)
        /// </summary>
        [HttpGet("total-time/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalExecutionTime(Guid jobCardId)
        {
            var response = await _service.GetTotalExecutionTimeForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get total completed quantity for a job card
        /// </summary>
        [HttpGet("total-completed/{jobCardId:guid}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalCompletedQuantity(Guid jobCardId)
        {
            var response = await _service.GetTotalCompletedQuantityForJobCardAsync(jobCardId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a production execution record
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var response = await _service.DeleteExecutionAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // Helper Methods
        private static JobCardExecutionResponse MapToResponse(Models.Production.JobCardExecution execution)
        {
            return new JobCardExecutionResponse
            {
                Id = execution.Id,
                JobCardId = execution.JobCardId,
                JobCardNo = execution.JobCardNo,
                OrderNo = execution.OrderNo,
                MachineId = execution.MachineId,
                MachineName = execution.MachineName,
                OperatorId = execution.OperatorId,
                OperatorName = execution.OperatorName,
                StartTime = execution.StartTime,
                EndTime = execution.EndTime,
                PausedTime = execution.PausedTime,
                ResumedTime = execution.ResumedTime,
                TotalTimeMin = execution.TotalTimeMin,
                IdleTimeMin = execution.IdleTimeMin,
                QuantityStarted = execution.QuantityStarted,
                QuantityCompleted = execution.QuantityCompleted,
                QuantityRejected = execution.QuantityRejected,
                QuantityInProgress = execution.QuantityInProgress,
                ExecutionStatus = execution.ExecutionStatus,
                Notes = execution.Notes,
                IssuesEncountered = execution.IssuesEncountered,
                CreatedAt = execution.CreatedAt,
                CreatedBy = execution.CreatedBy
            };
        }
    }
}
