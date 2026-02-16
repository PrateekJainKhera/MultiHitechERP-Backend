using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/child-part-types")]
    public class ChildPartTypesController : ControllerBase
    {
        private readonly IChildPartTypeRepository _repository;

        public ChildPartTypesController(IChildPartTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _repository.GetActiveAsync();
            var response = types.Select(t => new ChildPartTypeResponse
            {
                Id = t.Id,
                TypeName = t.TypeName,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            });
            return Ok(new { success = true, data = response });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildPartTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TypeName))
                return BadRequest(new { success = false, message = "Type name is required" });

            var childPartType = new ChildPartType
            {
                TypeName = request.TypeName.Trim().ToUpper(),
                CreatedBy = request.CreatedBy ?? "Admin"
            };

            var id = await _repository.CreateAsync(childPartType);
            return Ok(new { success = true, data = id, message = $"Child part type '{childPartType.TypeName}' created successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { success = false, message = "Child part type not found" });

            var success = await _repository.DeleteAsync(id);
            return success
                ? Ok(new { success = true, message = "Deleted successfully" })
                : BadRequest(new { success = false, message = "Failed to delete" });
        }
    }

    public class CreateChildPartTypeRequest
    {
        public string TypeName { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
