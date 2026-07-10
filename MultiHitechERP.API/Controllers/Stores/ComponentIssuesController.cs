using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Stores
{
    [ApiController]
    [Route("api/component-issues")]
    public class ComponentIssuesController : ControllerBase
    {
        private readonly IComponentIssueService _service;

        public ComponentIssuesController(IComponentIssueService service)
        {
            _service = service;
        }

        [HttpGet("components-in-stock")]
        public async Task<IActionResult> GetComponentsWithStock()
        {
            var result = await _service.GetComponentsWithStockAsync();
            return Ok(result);
        }

        // Shop-floor component stock (Qty / Reserved / Available per component)
        [HttpGet("shop-floor-stock")]
        public async Task<IActionResult> GetShopFloorStock()
        {
            var result = await _service.GetShopFloorStockAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-component/{componentId:int}")]
        public async Task<IActionResult> GetByComponent(int componentId)
        {
            var result = await _service.GetByComponentIdAsync(componentId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComponentIssueRequest request)
        {
            var result = await _service.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
