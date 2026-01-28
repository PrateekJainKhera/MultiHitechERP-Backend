using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Planning
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardsController : ControllerBase
    {
        private readonly IJobCardService _jobCardService;

        public JobCardsController(IJobCardService jobCardService)
        {
            _jobCardService = jobCardService;
        }

        // Basic CRUD Operations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _jobCardService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _jobCardService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-job-card-no/{jobCardNo}")]
        public async Task<IActionResult> GetByJobCardNo(string jobCardNo)
        {
            var result = await _jobCardService.GetByJobCardNoAsync(jobCardNo);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _jobCardService.GetByOrderIdAsync(orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-process/{processId}")]
        public async Task<IActionResult> GetByProcessId(int processId)
        {
            var result = await _jobCardService.GetByProcessIdAsync(processId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _jobCardService.GetByStatusAsync(status);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateJobCardRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobCardService.CreateJobCardAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateJobCardRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _jobCardService.UpdateJobCardAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _jobCardService.DeleteJobCardAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Status Operations
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateJobCardStatusRequest request)
        {
            var result = await _jobCardService.UpdateStatusAsync(id, request.Status);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Dependency Operations
        [HttpGet("{id}/dependents")]
        public async Task<IActionResult> GetDependentJobCards(int id)
        {
            var result = await _jobCardService.GetDependentJobCardsAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/prerequisites")]
        public async Task<IActionResult> GetPrerequisiteJobCards(int id)
        {
            var result = await _jobCardService.GetPrerequisiteJobCardsAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/add-dependency")]
        public async Task<IActionResult> AddDependency(int id, [FromBody] AddDependencyRequest request)
        {
            var result = await _jobCardService.AddDependencyAsync(id, request.PrerequisiteJobCardId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("dependencies/{dependencyId}")]
        public async Task<IActionResult> RemoveDependency(int dependencyId)
        {
            var result = await _jobCardService.RemoveDependencyAsync(dependencyId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Query Operations
        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedJobCards()
        {
            var result = await _jobCardService.GetBlockedJobCardsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    // Helper DTOs for specific operations
    public class UpdateJobCardStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class AddDependencyRequest
    {
        public int PrerequisiteJobCardId { get; set; }
    }
}
