using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Auth;
using MultiHitechERP.API.Services;

namespace MultiHitechERP.API.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                var result = await _authService.LoginAsync(req.Username, req.Password);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["X-Session-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
                await _authService.LogoutAsync(token);
            return Ok(new { message = "Logged out" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            try
            {
                var token = Request.Headers["X-Session-Token"].FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                    return Unauthorized(new { message = "No session" });

                var user = await _authService.ValidateSessionAsync(token);
                if (user == null)
                    return Unauthorized(new { message = "Invalid or expired session" });

                return Ok(new { user.Id, user.FullName, user.Username, user.RoleId, user.RoleName, user.IsAdmin });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("modules")]
        public IActionResult GetModules()
        {
            return Ok(new { modules = AuthService.Modules, actions = AuthService.Actions });
        }
    }
}
