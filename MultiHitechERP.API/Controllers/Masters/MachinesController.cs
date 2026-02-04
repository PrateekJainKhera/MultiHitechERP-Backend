using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/machines")]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineService _machineService;

        public MachinesController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _machineService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _machineService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{machineCode}")]
        public async Task<IActionResult> GetByMachineCode(string machineCode)
        {
            var result = await _machineService.GetByMachineCodeAsync(machineCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMachines()
        {
            var result = await _machineService.GetActiveMachinesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-type/{machineType}")]
        public async Task<IActionResult> GetByType(string machineType)
        {
            var result = await _machineService.GetByTypeAsync(machineType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-department/{department}")]
        public async Task<IActionResult> GetByDepartment(string department)
        {
            var result = await _machineService.GetByDepartmentAsync(department);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMachineRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _machineService.CreateMachineAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMachineRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _machineService.UpdateMachineAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _machineService.DeleteMachineAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
