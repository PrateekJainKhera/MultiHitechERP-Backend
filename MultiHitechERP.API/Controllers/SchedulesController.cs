using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// Get all schedules
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _scheduleService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get schedule by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _scheduleService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get schedules by machine ID
        /// </summary>
        [HttpGet("by-machine/{machineId}")]
        public async Task<IActionResult> GetByMachine(int machineId)
        {
            var result = await _scheduleService.GetByMachineIdAsync(machineId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get schedules by job card ID
        /// </summary>
        [HttpGet("by-jobcard/{jobCardId}")]
        public async Task<IActionResult> GetByJobCard(int jobCardId)
        {
            var result = await _scheduleService.GetByJobCardIdAsync(jobCardId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get schedules by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _scheduleService.GetByDateRangeAsync(startDate, endDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get full scheduling tree for an order: Order → Child Parts → Process Steps with machine assignments
        /// </summary>
        [HttpGet("order/{orderId}/tree")]
        public async Task<IActionResult> GetOrderSchedulingTree(int orderId)
        {
            var result = await _scheduleService.GetOrderSchedulingTreeAsync(orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get full scheduling tree for an order item: OrderItem → Child Parts → Process Steps with machine assignments
        /// For multi-product orders (e.g., ORD-007-A)
        /// </summary>
        [HttpGet("order-item/{orderItemId}/tree")]
        public async Task<IActionResult> GetOrderItemSchedulingTree(int orderItemId)
        {
            var result = await _scheduleService.GetOrderItemSchedulingTreeAsync(orderItemId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// SEMI-AUTOMATIC SCHEDULING: Get intelligent machine suggestions for a job card
        /// </summary>
        [HttpGet("suggestions/{jobCardId}")]
        public async Task<IActionResult> GetMachineSuggestions(int jobCardId)
        {
            var result = await _scheduleService.GetMachineSuggestionsAsync(jobCardId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Create a new schedule
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _scheduleService.CreateScheduleAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update an existing schedule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _scheduleService.UpdateScheduleAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete a schedule
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _scheduleService.DeleteScheduleAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update schedule status
        /// </summary>
        [HttpPost("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _scheduleService.UpdateStatusAsync(id, request.Status, request.UpdatedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Reschedule an existing schedule
        /// </summary>
        [HttpPost("{id}/reschedule")]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleRequest request)
        {
            var result = await _scheduleService.RescheduleAsync(
                id,
                request.NewStartTime,
                request.NewEndTime,
                request.Reason,
                request.RescheduledBy
            );
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }

    // Helper DTOs for controller endpoints
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }

    public class RescheduleRequest
    {
        public DateTime NewStartTime { get; set; }
        public DateTime NewEndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? RescheduledBy { get; set; }
    }
}
