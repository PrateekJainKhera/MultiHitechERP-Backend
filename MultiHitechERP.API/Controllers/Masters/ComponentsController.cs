using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentService _componentService;

        public ComponentsController(IComponentService componentService)
        {
            _componentService = componentService;
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockComponents()
        {
            var response = await _componentService.GetLowStockComponentsAsync();
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComponents()
        {
            var response = await _componentService.GetAllComponentsAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComponentById(int id)
        {
            var response = await _componentService.GetComponentByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("by-part-number/{partNumber}")]
        public async Task<IActionResult> GetComponentByPartNumber(string partNumber)
        {
            var response = await _componentService.GetComponentByPartNumberAsync(partNumber);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("by-category/{category}")]
        public async Task<IActionResult> GetComponentsByCategory(string category)
        {
            var response = await _componentService.GetComponentsByCategoryAsync(category);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchComponentsByName([FromQuery] string searchTerm)
        {
            var response = await _componentService.SearchComponentsByNameAsync(searchTerm);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComponent([FromBody] CreateComponentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _componentService.CreateComponentAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetComponentById), new { id = response.Data }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComponent(int id, [FromBody] UpdateComponentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _componentService.UpdateComponentAsync(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComponent(int id)
        {
            var response = await _componentService.DeleteComponentAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
    }
}
