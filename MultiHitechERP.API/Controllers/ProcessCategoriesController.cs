using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/process-categories")]
    public class ProcessCategoriesController : ControllerBase
    {
        private readonly IProcessCategoryService _service;

        public ProcessCategoriesController(IProcessCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _service.CreateAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProcessCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("ID mismatch");

            var response = await _service.UpdateAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
