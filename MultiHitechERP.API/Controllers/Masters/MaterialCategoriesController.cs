using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialCategoriesController : ControllerBase
    {
        private readonly IMaterialCategoryService _categoryService;

        public MaterialCategoriesController(IMaterialCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("by-code/{categoryCode}")]
        public async Task<IActionResult> GetCategoryByCode(string categoryCode)
        {
            var response = await _categoryService.GetCategoryByCodeAsync(categoryCode);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

        [HttpGet("by-material-type/{materialType}")]
        public async Task<IActionResult> GetCategoriesByMaterialType(string materialType)
        {
            var response = await _categoryService.GetCategoriesByMaterialTypeAsync(materialType);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            var response = await _categoryService.GetActiveCategoriesAsync();
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategoriesByName([FromQuery] string searchTerm)
        {
            var response = await _categoryService.SearchCategoriesByNameAsync(searchTerm);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateMaterialCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _categoryService.CreateCategoryAsync(request);
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetCategoryById), new { id = response.Data }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateMaterialCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _categoryService.UpdateCategoryAsync(id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
    }
}
