using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EstimationsController : ControllerBase
    {
        private readonly IEstimationService _estimationService;
        private readonly ILogger<EstimationsController> _logger;

        public EstimationsController(IEstimationService estimationService, ILogger<EstimationsController> logger)
        {
            _estimationService = estimationService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var response = await _estimationService.GetAllAsync();
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _estimationService.GetByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("by-customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationResponse>>), 200)]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var response = await _estimationService.GetByCustomerIdAsync(customerId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EstimationResponse>>), 200)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var response = await _estimationService.GetByStatusAsync(status);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateEstimationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _estimationService.CreateAsync(request);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        [HttpPost("{id}/revise")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 201)]
        public async Task<IActionResult> Revise(int id, [FromBody] CreateEstimationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _estimationService.ReviseAsync(id, request);
            if (!response.Success) return BadRequest(response);

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id}/submit")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 200)]
        public async Task<IActionResult> Submit(int id)
        {
            var response = await _estimationService.SubmitAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/approve")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 200)]
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveEstimationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _estimationService.ApproveAsync(id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{id}/reject")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 200)]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectEstimationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var response = await _estimationService.RejectAsync(id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("{id}/convert-to-order")]
        [ProducesResponseType(typeof(ApiResponse<EstimationResponse>), 200)]
        public async Task<IActionResult> ConvertToOrder(int id)
        {
            var response = await _estimationService.ConvertToOrderAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _estimationService.DeleteAsync(id);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
