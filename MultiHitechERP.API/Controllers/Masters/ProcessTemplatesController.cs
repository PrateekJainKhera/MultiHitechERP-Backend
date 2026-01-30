using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    [ApiController]
    [Route("api/process-templates")]
    public class ProcessTemplatesController : ControllerBase
    {
        private readonly IProcessTemplateService _processTemplateService;

        public ProcessTemplatesController(IProcessTemplateService processTemplateService)
        {
            _processTemplateService = processTemplateService;
        }

        #region Template CRUD Operations

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _processTemplateService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _processTemplateService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-name/{templateName}")]
        public async Task<IActionResult> GetByName(string templateName)
        {
            var result = await _processTemplateService.GetByNameAsync(templateName);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTemplates()
        {
            var result = await _processTemplateService.GetActiveTemplatesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Old endpoints removed - replaced with GetByApplicableType
        // [HttpGet("default")]
        // [HttpGet("by-product/{productId:int}")]
        // [HttpGet("by-child-part/{childPartId:int}")]
        // [HttpGet("by-type/{templateType}")]

        [HttpGet("by-applicable-type/{applicableType}")]
        public async Task<IActionResult> GetByApplicableType(string applicableType)
        {
            var result = await _processTemplateService.GetByApplicableTypeAsync(applicableType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _processTemplateService.CreateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProcessTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _processTemplateService.UpdateTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _processTemplateService.DeleteTemplateAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // Approval endpoint removed in simplified version
        // [HttpPost("{id:int}/approve")]

        #endregion

        #region Template with Steps Operations

        [HttpGet("{id:int}/with-steps")]
        public async Task<IActionResult> GetTemplateWithSteps(int id)
        {
            var result = await _processTemplateService.GetTemplateWithStepsAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("with-steps")]
        public async Task<IActionResult> CreateTemplateWithSteps([FromBody] CreateProcessTemplateWithStepsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _processTemplateService.CreateTemplateWithStepsAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Template Steps Operations

        [HttpGet("{templateId:int}/steps")]
        public async Task<IActionResult> GetStepsByTemplateId(int templateId)
        {
            var result = await _processTemplateService.GetStepsByTemplateIdAsync(templateId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("steps")]
        public async Task<IActionResult> AddStep([FromBody] CreateProcessTemplateStepRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _processTemplateService.AddStepToTemplateAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("steps/{stepId:int}")]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateProcessTemplateStepRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (stepId != request.Id)
                return BadRequest("ID mismatch");

            var result = await _processTemplateService.UpdateStepAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("steps/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(int stepId)
        {
            var result = await _processTemplateService.DeleteStepAsync(stepId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion
    }
}
