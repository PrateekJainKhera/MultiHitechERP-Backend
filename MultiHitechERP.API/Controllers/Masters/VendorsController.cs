using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/vendors")]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vendorService.GetAllVendorsAsync();
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _vendorService.GetActiveVendorsAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _vendorService.GetVendorByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("searchTerm is required");
            var result = await _vendorService.SearchVendorsAsync(searchTerm);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVendorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _vendorService.CreateVendorAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVendorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _vendorService.UpdateVendorAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _vendorService.DeleteVendorAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
