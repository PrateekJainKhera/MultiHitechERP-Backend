using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Procurement
{
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrdersController(IPurchaseOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-vendor/{vendorId:int}")]
        public async Task<IActionResult> GetByVendor(int vendorId)
            => Ok(await _service.GetByVendorAsync(vendorId));

        [HttpGet("by-pr/{prId:int}")]
        public async Task<IActionResult> GetByPurchaseRequest(int prId)
            => Ok(await _service.GetByPurchaseRequestAsync(prId));

        [HttpPut("{id:int}/send")]
        public async Task<IActionResult> Send(int id)
        {
            var result = await _service.SendAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _service.CancelAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
