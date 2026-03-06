using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Auth;
using MultiHitechERP.API.Services;

namespace MultiHitechERP.API.Controllers.Auth
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        // ── Users ──────────────────────────────────────────────────────────────

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try { return Ok(await _authService.GetUsersAsync()); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest req)
        {
            try { return Ok(await _authService.CreateUserAsync(req)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest req)
        {
            try { await _authService.UpdateUserAsync(id, req); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest req)
        {
            try { await _authService.ResetPasswordAsync(id, req.NewPassword); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try { await _authService.DeleteUserAsync(id); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── Roles ──────────────────────────────────────────────────────────────

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try { return Ok(await _authService.GetRolesAsync()); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest req)
        {
            try { return Ok(await _authService.CreateRoleAsync(req)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleRequest req)
        {
            try { await _authService.UpdateRoleAsync(id, req); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try { await _authService.DeleteRoleAsync(id); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // ── Permissions ────────────────────────────────────────────────────────

        [HttpGet("roles/{id}/permissions")]
        public async Task<IActionResult> GetPermissions(int id)
        {
            try { return Ok(await _authService.GetPermissionsAsync(id)); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("roles/{id}/permissions")]
        public async Task<IActionResult> SavePermissions(int id, [FromBody] SavePermissionsRequest req)
        {
            try { await _authService.SavePermissionsAsync(id, req); return Ok(); }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}
