using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;
using System.Threading.Tasks;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/masters/[controller]")]
    public class MachineModelsController : ControllerBase
    {
        private readonly IMachineModelService _service;

        public MachineModelsController(IMachineModelService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all machine models
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<MachineModelResponse[]>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Get machine model by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<MachineModelResponse>>> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Create a new machine model
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateMachineModelRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<int>.ErrorResponse("Invalid request data"));

            var response = await _service.CreateAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Update an existing machine model
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] UpdateMachineModelRequest request)
        {
            if (id != request.Id)
                return BadRequest(ApiResponse<bool>.ErrorResponse("ID mismatch"));

            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.ErrorResponse("Invalid request data"));

            var response = await _service.UpdateAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete a machine model
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
