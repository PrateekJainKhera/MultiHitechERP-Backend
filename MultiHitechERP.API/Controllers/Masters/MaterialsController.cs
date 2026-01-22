using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    /// <summary>
    /// Controller for Material master data operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        /// <summary>
        /// Get all materials
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _materialService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get material by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _materialService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get material by material code
        /// </summary>
        [HttpGet("by-code/{materialCode}")]
        public async Task<IActionResult> GetByMaterialCode(string materialCode)
        {
            var result = await _materialService.GetByMaterialCodeAsync(materialCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get all active materials
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveMaterials()
        {
            var result = await _materialService.GetActiveMaterialsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search materials by name
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var result = await _materialService.SearchByNameAsync(searchTerm);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get materials by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var result = await _materialService.GetByCategoryAsync(category);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get materials by grade
        /// </summary>
        [HttpGet("by-grade/{grade}")]
        public async Task<IActionResult> GetByGrade(string grade)
        {
            var result = await _materialService.GetByGradeAsync(grade);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get materials by type
        /// </summary>
        [HttpGet("by-type/{materialType}")]
        public async Task<IActionResult> GetByMaterialType(string materialType)
        {
            var result = await _materialService.GetByMaterialTypeAsync(materialType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get low stock materials
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockMaterials()
        {
            var result = await _materialService.GetLowStockMaterialsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Create new material
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMaterialRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _materialService.CreateMaterialAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update existing material
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMaterialRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _materialService.UpdateMaterialAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete material
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _materialService.DeleteMaterialAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Activate material
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _materialService.ActivateMaterialAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deactivate material
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _materialService.DeactivateMaterialAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
