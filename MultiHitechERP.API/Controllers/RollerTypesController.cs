using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/roller-types")]
    public class RollerTypesController : ControllerBase
    {
        private readonly IRollerTypeRepository _repository;

        public RollerTypesController(IRollerTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _repository.GetAllAsync();
            return Ok(new { success = true, data = types.Select(t => new { t.Id, t.TypeName, t.IsActive, t.CreatedAt }) });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRollerTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TypeName))
                return BadRequest(new { success = false, message = "Type name is required" });

            var rollerType = new RollerType
            {
                TypeName = request.TypeName.Trim(),
                CreatedBy = request.CreatedBy ?? "Admin"
            };

            var id = await _repository.CreateAsync(rollerType);
            return Ok(new { success = true, data = id, message = $"Roller type '{rollerType.TypeName}' created successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { success = false, message = "Roller type not found" });

            var success = await _repository.DeleteAsync(id);
            return success
                ? Ok(new { success = true, message = "Deleted successfully" })
                : BadRequest(new { success = false, message = "Failed to delete" });
        }
    }

    public class CreateRollerTypeRequest
    {
        public string TypeName { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
