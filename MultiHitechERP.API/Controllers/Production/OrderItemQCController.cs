using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Production
{
    [ApiController]
    [Route("api/order-item-qc")]
    public class OrderItemQCController : ControllerBase
    {
        private readonly IOrderItemQCService _service;

        public OrderItemQCController(IOrderItemQCService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET /api/order-item-qc/{orderItemId}
        /// Returns the latest QC record for a specific order item (null data if none exists).
        /// </summary>
        [HttpGet("{orderItemId:int}")]
        public async Task<IActionResult> GetLatest(int orderItemId)
        {
            try
            {
                var record = await _service.GetLatestByOrderItemAsync(orderItemId);
                return Ok(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/order-item-qc/pending
        /// Returns order items whose assembly is complete but QC is not yet Passed.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var result = await _service.GetPendingItemsAsync();
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/order-item-qc/{orderItemId}/submit
        /// Submit QC result (Passed or Failed) with optional PDF certificate.
        /// Form fields: orderId (int), qcStatus (string), qcBy (string), notes (string?), certificate (IFormFile?)
        /// </summary>
        [HttpPost("{orderItemId:int}/submit")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit(
            int orderItemId,
            [FromForm] int orderId,
            [FromForm] string qcStatus,
            [FromForm] string qcBy,
            [FromForm] string? notes,
            IFormFile? certificate)
        {
            try
            {
                System.IO.Stream? stream = null;
                string? contentType = null;
                string? fileName = null;

                if (certificate != null && certificate.Length > 0)
                {
                    stream = certificate.OpenReadStream();
                    contentType = certificate.ContentType;
                    fileName = certificate.FileName;
                }

                var result = await _service.SubmitQCAsync(
                    orderItemId, orderId, qcStatus, qcBy, notes,
                    stream, contentType, fileName);

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
