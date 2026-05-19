using ICMS.Application.DTOs.Auth;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using ICMS.Application.DTOs.User;
using ICMS.API.Extensions;
using ICMS.Domain.Constants;

namespace ICMS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("stricter")]
    public class AuthController(IAuthService authService, IUserService userService, ILogger<AuthController> logger) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto loginDto)
        {
            logger.LogInformation("Standard login attempt for user: {UserName}", loginDto.UserName);
            var result = await authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("beneficiary-login")]
        public async Task<ActionResult<AuthResponseDto>> BeneficiaryLoginAsync([FromBody] LoginDto loginDto, [FromQuery] string role)
        {
            try 
            {
                logger.LogInformation("Beneficiary login attempt for user: {UserName} with role: {Role}", loginDto.UserName, role);
                var result = await authService.BeneficiaryLoginAsync(loginDto);
                
                if (string.IsNullOrWhiteSpace(role) || 
                    result.User.Roles == null || 
                    !result.User.Roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)))
                {
                    logger.LogWarning("Beneficiary login failed: User {UserName} does not have the requested role: {Role}", loginDto.UserName, role);
                    return Unauthorized();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during beneficiary login for user: {UserName}", loginDto.UserName);
                throw; // Let GlobalExceptionHandler handle it, but we have the log now
            }
        }

        [AllowAnonymous]
        [HttpGet("beneficiary-login")]
        public IActionResult BeneficiaryLoginHealthCheck()
        {
            return Ok(new { status = "Beneficiary login endpoint is reachable via GET. Use POST for actual login." });
        }

        [AllowAnonymous]
        [HttpPost("fieldworker-login")]
        public async Task<ActionResult<AuthResponseDto>> FieldVisitWorkerLoginAsync([FromBody] LoginDto loginDto)
        {
            var result = await authService.FieldVisitWorkerLoginAsync(loginDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshTokenAsync([FromBody] RefreshTokenRequestDto refreshDto)
        {
            var result = await authService.RefreshTokenAsync(refreshDto);
            return Ok(result);
        }

        [Authorize(Roles = Roles.VaccinatedIndividual + "," + Roles.PregnantWoman)]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangeOwnPasswordDto changeOwnPasswordDto)
        {
            var userId = ClaimsPrincipalExtensions.GetUserId(User);
            logger.LogInformation("Authenticated user {UserId} is attempting to change their password.", userId);

            var success = await userService.ChangeOwnPasswordAsync(userId, changeOwnPasswordDto.OldPassword, changeOwnPasswordDto.NewPassword);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
