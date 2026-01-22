using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Planning
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardsController : ControllerBase
    {
        private readonly IJobCardService _jobCardService;

        public JobCardsController(IJobCardService jobCardService)
        {
            _jobCardService = jobCardService;
        }

        // Basic CRUD Operations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _jobCardService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _jobCardService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-job-card-no/{jobCardNo}")]
        public async Task<IActionResult> GetByJobCardNo(string jobCardNo)
        {
            var result = await _jobCardService.GetByJobCardNoAsync(jobCardNo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _jobCardService.GetByOrderIdAsync(orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-process/{processId}")]
        public async Task<IActionResult> GetByProcessId(int processId)
        {
            var result = await _jobCardService.GetByProcessIdAsync(processId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _jobCardService.GetByStatusAsync(status);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobCardRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobCardService.CreateJobCardAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobCardRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobCardService.UpdateJobCardAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _jobCardService.DeleteJobCardAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Status Operations
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateJobCardStatusRequest request)
        {
            var result = await _jobCardService.UpdateStatusAsync(id, request.Status);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/material-status")]
        public async Task<IActionResult> UpdateMaterialStatus(int id, [FromBody] UpdateMaterialStatusRequest request)
        {
            var result = await _jobCardService.UpdateMaterialStatusAsync(id, request.MaterialStatus);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/schedule-status")]
        public async Task<IActionResult> UpdateScheduleStatus(int id, [FromBody] UpdateScheduleStatusRequest request)
        {
            var result = await _jobCardService.UpdateScheduleStatusAsync(id, request.ScheduleStatus, request.StartDate, request.EndDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Assignment Operations
        [HttpPost("{id}/assign-machine")]
        public async Task<IActionResult> AssignMachine(int id, [FromBody] AssignMachineRequest request)
        {
            var result = await _jobCardService.AssignMachineAsync(id, request.MachineId, request.MachineName);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/assign-operator")]
        public async Task<IActionResult> AssignOperator(int id, [FromBody] AssignOperatorRequest request)
        {
            var result = await _jobCardService.AssignOperatorAsync(id, request.OperatorId, request.OperatorName);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Execution Operations
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartExecution(int id)
        {
            var result = await _jobCardService.StartExecutionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteExecution(int id)
        {
            var result = await _jobCardService.CompleteExecutionAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/update-quantities")]
        public async Task<IActionResult> UpdateQuantities(int id, [FromBody] UpdateQuantitiesRequest request)
        {
            var result = await _jobCardService.UpdateQuantitiesAsync(id, request.CompletedQty, request.RejectedQty, request.ReworkQty, request.InProgressQty);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Dependency Operations
        [HttpGet("{id}/dependents")]
        public async Task<IActionResult> GetDependentJobCards(int id)
        {
            var result = await _jobCardService.GetDependentJobCardsAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/prerequisites")]
        public async Task<IActionResult> GetPrerequisiteJobCards(int id)
        {
            var result = await _jobCardService.GetPrerequisiteJobCardsAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/add-dependency")]
        public async Task<IActionResult> AddDependency(int id, [FromBody] AddDependencyRequest request)
        {
            var result = await _jobCardService.AddDependencyAsync(id, request.PrerequisiteJobCardId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("dependencies/{dependencyId}")]
        public async Task<IActionResult> RemoveDependency(int dependencyId)
        {
            var result = await _jobCardService.RemoveDependencyAsync(dependencyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Query Operations
        [HttpGet("ready-for-scheduling")]
        public async Task<IActionResult> GetReadyForScheduling()
        {
            var result = await _jobCardService.GetReadyForSchedulingAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("scheduled")]
        public async Task<IActionResult> GetScheduledJobCards()
        {
            var result = await _jobCardService.GetScheduledJobCardsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("in-progress")]
        public async Task<IActionResult> GetInProgressJobCards()
        {
            var result = await _jobCardService.GetInProgressJobCardsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedJobCards()
        {
            var result = await _jobCardService.GetBlockedJobCardsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-machine/{machineId}")]
        public async Task<IActionResult> GetByMachineId(int machineId)
        {
            var result = await _jobCardService.GetByMachineIdAsync(machineId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-operator/{operatorId}")]
        public async Task<IActionResult> GetByOperatorId(int operatorId)
        {
            var result = await _jobCardService.GetByOperatorIdAsync(operatorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    // Helper DTOs for specific operations
    public class UpdateJobCardStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateMaterialStatusRequest
    {
        public string MaterialStatus { get; set; } = string.Empty;
    }

    public class UpdateScheduleStatusRequest
    {
        public string ScheduleStatus { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AssignMachineRequest
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; } = string.Empty;
    }

    public class AssignOperatorRequest
    {
        public int OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
    }

    public class UpdateQuantitiesRequest
    {
        public int CompletedQty { get; set; }
        public int RejectedQty { get; set; }
        public int ReworkQty { get; set; }
        public int InProgressQty { get; set; }
    }

    public class AddDependencyRequest
    {
        public int PrerequisiteJobCardId { get; set; }
    }
}
