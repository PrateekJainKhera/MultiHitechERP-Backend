using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/child-part-templates")]
    public class ChildPartTemplatesController : ControllerBase
    {
        private readonly IChildPartTemplateService _childPartTemplateService;

        public ChildPartTemplatesController(IChildPartTemplateService childPartTemplateService)
        {
            _childPartTemplateService = childPartTemplateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _childPartTemplateService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _childPartTemplateService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-code/{templateCode}")]
        public async Task<IActionResult> GetByCode(string templateCode)
        {
            var result = await _childPartTemplateService.GetByCodeAsync(templateCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-name/{templateName}")]
        public async Task<IActionResult> GetByName(string templateName)
        {
            var result = await _childPartTemplateService.GetByNameAsync(templateName);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTemplates()
        {
            var result = await _childPartTemplateService.GetActiveTemplatesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-child-part-type/{childPartType}")]
        public async Task<IActionResult> GetByChildPartType(string childPartType)
        {
            var result = await _childPartTemplateService.GetByChildPartTypeAsync(childPartType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("by-roller-type/{rollerType}")]
        public async Task<IActionResult> GetByRollerType(string rollerType)
        {
            var result = await _childPartTemplateService.GetByRollerTypeAsync(rollerType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChildPartTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _childPartTemplateService.CreateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChildPartTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _childPartTemplateService.UpdateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _childPartTemplateService.DeleteTemplateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
