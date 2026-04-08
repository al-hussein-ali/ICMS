using ICMS.Application.DTOs.Auth;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAsync([FromBody] LoginDto loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshTokenAsync([FromBody] RefreshTokenRequestDto refreshDto)
        {
            var result = await authService.RefreshTokenAsync(refreshDto);
            return Ok(result);
        }
    }
}
