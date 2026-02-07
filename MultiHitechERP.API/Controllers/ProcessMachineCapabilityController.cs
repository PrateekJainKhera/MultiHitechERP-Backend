using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/process-machine-capability")]
    public class ProcessMachineCapabilityController : ControllerBase
    {
        private readonly IProcessMachineCapabilityService _service;
        private readonly ILogger<ProcessMachineCapabilityController> _logger;

        public ProcessMachineCapabilityController(
            IProcessMachineCapabilityService service,
            ILogger<ProcessMachineCapabilityController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all process-machine capabilities
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get process-machine capability by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get all machines capable of performing a specific process
        /// </summary>
        [HttpGet("process/{processId}/capable-machines")]
        public async Task<IActionResult> GetCapableMachinesForProcess(int processId)
        {
            var result = await _service.GetCapableMachinesForProcessAsync(processId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all capabilities for a specific process
        /// </summary>
        [HttpGet("process/{processId}")]
        public async Task<IActionResult> GetByProcessId(int processId)
        {
            var result = await _service.GetByProcessIdAsync(processId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get all processes a specific machine can perform
        /// </summary>
        [HttpGet("machine/{machineId}")]
        public async Task<IActionResult> GetByMachineId(int machineId)
        {
            var result = await _service.GetByMachineIdAsync(machineId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Create a new process-machine capability
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessMachineCapabilityRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<int>.ErrorResponse("Invalid request data"));
            }

            var result = await _service.CreateAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        /// <summary>
        /// Update an existing process-machine capability
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProcessMachineCapabilityRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));
            }

            if (id != request.Id)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));
            }

            var result = await _service.UpdateAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete a process-machine capability
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
