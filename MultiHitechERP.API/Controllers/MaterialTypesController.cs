using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/material-types")]
    public class MaterialTypesController : ControllerBase
    {
        private readonly IMaterialTypeRepository _repo;

        public MaterialTypesController(IMaterialTypeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var types = await _repo.GetAllAsync();
                return Ok(new { success = true, data = types });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMaterialTypeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { success = false, message = "Name is required" });

                var model = new MaterialTypeModel
                {
                    Name = request.Name.Trim(),
                    CreatedBy = "Admin"
                };

                var id = await _repo.CreateAsync(model);
                var created = await _repo.GetByIdAsync(id);
                return Ok(new { success = true, data = created, message = $"Material type '{model.Name}' created" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { success = false, message = "Material type not found" });

                await _repo.DeleteAsync(id);
                return Ok(new { success = true, message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateMaterialTypeRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
