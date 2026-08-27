using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/scrap-sales")]
    public class ScrapSalesController : ControllerBase
    {
        private readonly IScrapSaleService _service;

        public ScrapSalesController(IScrapSaleService service)
        {
            _service = service;
        }

        // Material-wise wastage vs sold + full sales ledger
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var result = await _service.GetOverviewAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Record a scrap sale to a kabadi (manual, weight-based)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScrapSaleRequest request)
        {
            var result = await _service.CreateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
