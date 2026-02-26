using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Production
{
    [ApiController]
    [Route("api/osp")]
    public class OSPController : ControllerBase
    {
        private readonly IOSPTrackingService _ospService;

        public OSPController(IOSPTrackingService ospService)
        {
            _ospService = ospService;
        }

        /// <summary>GET /api/osp — all OSP tracking entries</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _ospService.GetAllAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>GET /api/osp/available-job-cards — scheduled OSP job cards without an active entry</summary>
        [HttpGet("available-job-cards")]
        public async Task<IActionResult> GetAvailableJobCards()
        {
            try
            {
                var result = await _ospService.GetAvailableJobCardsAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>POST /api/osp — create a single OSP tracking entry</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOSPTrackingRequest request)
        {
            try
            {
                var result = await _ospService.CreateAsync(request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>POST /api/osp/batch — send multiple job cards to the same vendor at once</summary>
        [HttpPost("batch")]
        public async Task<IActionResult> BatchCreate([FromBody] BatchCreateOSPRequest request)
        {
            try
            {
                var result = await _ospService.BatchCreateAsync(request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>POST /api/osp/{id}/receive — partial or full receive</summary>
        [HttpPost("{id:int}/receive")]
        public async Task<IActionResult> MarkReceived(int id, [FromBody] ReceiveOSPRequest request)
        {
            try
            {
                var result = await _ospService.MarkReceivedAsync(id, request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
