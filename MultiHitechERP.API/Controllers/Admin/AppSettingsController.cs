using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Response;
using MultiHitechERP.API.Repositories.Interfaces;

namespace MultiHitechERP.API.Controllers.Admin
{
    [ApiController]
    [Route("api/app-settings")]
    public class AppSettingsController : ControllerBase
    {
        private readonly IAppSettingsRepository _settingsRepo;

        public AppSettingsController(IAppSettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var settings = await _settingsRepo.GetAllAsync();
                var result = settings.Select(s => new
                {
                    s.Key,
                    s.Value,
                    s.Description,
                    updatedAt = s.UpdatedAt?.ToString("yyyy-MM-dd HH:mm"),
                    s.UpdatedBy
                });
                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetByKey(string key)
        {
            try
            {
                var value = await _settingsRepo.GetValueAsync(key);
                if (value == null) return NotFound(ApiResponse<object>.ErrorResponse($"Setting '{key}' not found"));
                return Ok(ApiResponse<object>.SuccessResponse(new { key, value }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] UpdateSettingRequest request)
        {
            try
            {
                await _settingsRepo.SetValueAsync(key, request.Value, request.UpdatedBy);
                return Ok(ApiResponse<object>.SuccessResponse(new { key, value = request.Value }, "Setting updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }

    public class UpdateSettingRequest
    {
        public string Value { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }
}
