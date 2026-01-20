using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperatorsController : ControllerBase
    {
        private readonly IOperatorService _operatorService;

        public OperatorsController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _operatorService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _operatorService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{operatorCode}")]
        public async Task<IActionResult> GetByOperatorCode(string operatorCode)
        {
            var result = await _operatorService.GetByOperatorCodeAsync(operatorCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveOperators()
        {
            var result = await _operatorService.GetActiveOperatorsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableOperators()
        {
            var result = await _operatorService.GetAvailableOperatorsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-department/{department}")]
        public async Task<IActionResult> GetByDepartment(string department)
        {
            var result = await _operatorService.GetByDepartmentAsync(department);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-shift/{shift}")]
        public async Task<IActionResult> GetByShift(string shift)
        {
            var result = await _operatorService.GetByShiftAsync(shift);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-skill-level/{skillLevel}")]
        public async Task<IActionResult> GetBySkillLevel(string skillLevel)
        {
            var result = await _operatorService.GetBySkillLevelAsync(skillLevel);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-machine/{machineId}")]
        public async Task<IActionResult> GetByMachineExpertise(Guid machineId)
        {
            var result = await _operatorService.GetByMachineExpertiseAsync(machineId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOperatorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _operatorService.CreateOperatorAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOperatorRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _operatorService.UpdateOperatorAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _operatorService.DeleteOperatorAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _operatorService.UpdateStatusAsync(id, request.Status);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/availability")]
        public async Task<IActionResult> UpdateAvailability(Guid id, [FromBody] UpdateAvailabilityRequest request)
        {
            var result = await _operatorService.UpdateAvailabilityAsync(id, request.IsAvailable);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignToJobCard(Guid id, [FromBody] AssignOperatorRequest request)
        {
            var result = await _operatorService.AssignToJobCardAsync(id, request.JobCardId, request.JobCardNo, request.MachineId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/release")]
        public async Task<IActionResult> ReleaseFromJobCard(Guid id)
        {
            var result = await _operatorService.ReleaseFromJobCardAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    // Helper DTOs for specific operations
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateAvailabilityRequest
    {
        public bool IsAvailable { get; set; }
    }

    public class AssignOperatorRequest
    {
        public Guid JobCardId { get; set; }
        public string JobCardNo { get; set; } = string.Empty;
        public Guid? MachineId { get; set; }
    }
}
