using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/product-templates")]
    public class ProductTemplatesController : ControllerBase
    {
        private readonly IProductTemplateService _productTemplateService;

        public ProductTemplatesController(IProductTemplateService productTemplateService)
        {
            _productTemplateService = productTemplateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productTemplateService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productTemplateService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{templateCode}")]
        public async Task<IActionResult> GetByCode(string templateCode)
        {
            var result = await _productTemplateService.GetByCodeAsync(templateCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-name/{templateName}")]
        public async Task<IActionResult> GetByName(string templateName)
        {
            var result = await _productTemplateService.GetByNameAsync(templateName);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTemplates()
        {
            var result = await _productTemplateService.GetActiveTemplatesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-roller-type/{rollerType}")]
        public async Task<IActionResult> GetByRollerType(string rollerType)
        {
            var result = await _productTemplateService.GetByRollerTypeAsync(rollerType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-process-template/{processTemplateId:int}")]
        public async Task<IActionResult> GetByProcessTemplateId(int processTemplateId)
        {
            var result = await _productTemplateService.GetByProcessTemplateIdAsync(processTemplateId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productTemplateService.CreateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _productTemplateService.UpdateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productTemplateService.DeleteTemplateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
