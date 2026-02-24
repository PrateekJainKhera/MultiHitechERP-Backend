using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Models.Masters;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/machine-types")]
    public class MachineTypesController : ControllerBase
    {
        private readonly IMachineTypeRepository _repository;

        public MachineTypesController(IMachineTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _repository.GetAllAsync();
            return Ok(new { success = true, data = types.Select(t => new { t.Id, t.Name, t.IsActive, t.CreatedAt }) });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMachineTypeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { success = false, message = "Name is required" });

            var machineType = new MachineType
            {
                Name = request.Name.Trim(),
                CreatedBy = request.CreatedBy ?? "Admin"
            };

            var id = await _repository.CreateAsync(machineType);
            return Ok(new { success = true, data = id, message = $"Machine type '{machineType.Name}' created successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { success = false, message = "Machine type not found" });

            var success = await _repository.DeleteAsync(id);
            return success
                ? Ok(new { success = true, message = "Deleted successfully" })
                : BadRequest(new { success = false, message = "Failed to delete" });
        }
    }

    public class CreateMachineTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
    }
}
