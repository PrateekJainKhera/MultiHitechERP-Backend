using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Scheduling
{
    [ApiController]
    [Route("api/scheduling-planner")]
    public class SchedulingPlannerController : ControllerBase
    {
        private readonly ISchedulingPlannerService _plannerService;

        public SchedulingPlannerController(ISchedulingPlannerService plannerService)
        {
            _plannerService = plannerService;
        }

        /// <summary>Step 1 — Orders with material issued (not yet machine-scheduled)</summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetSchedulableOrders()
        {
            var result = await _plannerService.GetSchedulableOrdersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Step 2 — Job cards for selected orders/order-items, grouped by child part</summary>
        [HttpPost("job-cards")]
        public async Task<IActionResult> GetJobCardsForOrders([FromBody] GetJobCardsForOrdersRequest request)
        {
            var result = await _plannerService.GetJobCardsForOrdersAsync(request.OrderIds, request.OrderItemIds);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Step 4 — Per-category machine suggestions for selected job cards</summary>
        [HttpPost("category-machines")]
        public async Task<IActionResult> GetCategoryMachineSuggestions([FromBody] GetCategoryMachineSuggestionsRequest request)
        {
            var result = await _plannerService.GetCategoryMachineSuggestionsAsync(request.JobCardIds, request.TargetDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Step 5 — Batch create all schedules</summary>
        [HttpPost("batch")]
        public async Task<IActionResult> BatchCreate([FromBody] BatchCreateSchedulesV2Request request)
        {
            var result = await _plannerService.BatchCreateSchedulesAsync(request.Schedules);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Batch reschedule — move multiple schedules to a new date/shift</summary>
        [HttpPost("batch-reschedule")]
        public async Task<IActionResult> BatchReschedule([FromBody] BatchRescheduleRequest request)
        {
            var result = await _plannerService.BatchRescheduleAsync(
                request.ScheduleIds, request.ShiftId, request.NewDate,
                request.Reason, request.RescheduledBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Create rework job card from a production job card</summary>
        [HttpPost("rework/{parentJobCardId}")]
        public async Task<IActionResult> CreateRework(int parentJobCardId, [FromBody] CreateReworkJobCardRequest request)
        {
            var result = await _plannerService.CreateReworkJobCardAsync(
                parentJobCardId, request.ReworkQty, request.Notes, request.CreatedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>Full rework — report rejection and create new job cards for ALL steps of the child part</summary>
        [HttpPost("full-rework/{jobCardId}")]
        public async Task<IActionResult> CreateFullRework(int jobCardId, [FromBody] FullReworkRequest request)
        {
            try
            {
                var result = await _plannerService.CreateFullReworkAsync(
                    jobCardId, request.RejectedQty, request.Reason, request.ReportedBy);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
