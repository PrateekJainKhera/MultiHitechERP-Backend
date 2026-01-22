using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessesController : ControllerBase
    {
        private readonly IProcessService _processService;

        public ProcessesController(IProcessService processService)
        {
            _processService = processService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _processService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _processService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{processCode}")]
        public async Task<IActionResult> GetByProcessCode(string processCode)
        {
            var result = await _processService.GetByProcessCodeAsync(processCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProcesses()
        {
            var result = await _processService.GetActiveProcessesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-type/{processType}")]
        public async Task<IActionResult> GetByProcessType(string processType)
        {
            var result = await _processService.GetByProcessTypeAsync(processType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-department/{department}")]
        public async Task<IActionResult> GetByDepartment(string department)
        {
            var result = await _processService.GetByDepartmentAsync(department);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-machine-type/{machineType}")]
        public async Task<IActionResult> GetByMachineType(string machineType)
        {
            var result = await _processService.GetByMachineTypeAsync(machineType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("outsourced")]
        public async Task<IActionResult> GetOutsourcedProcesses()
        {
            var result = await _processService.GetOutsourcedProcessesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _processService.CreateProcessAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProcessRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _processService.UpdateProcessAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _processService.DeleteProcessAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _processService.ActivateProcessAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _processService.DeactivateProcessAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
