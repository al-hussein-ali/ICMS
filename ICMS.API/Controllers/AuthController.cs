using ICMS.Application.DTOs.Auth;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;

namespace ICMS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [EnableRateLimiting("stricter")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("beneficiary-login")]
        public async Task<ActionResult<AuthResponseDto>> BeneficiaryLoginAsync([FromBody] LoginDto loginDto)
        {
            var result = await authService.BeneficiaryLoginAsync(loginDto);
            return Ok(result);
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
    }
}
