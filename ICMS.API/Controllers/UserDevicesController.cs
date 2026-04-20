using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public class UserDevicesController : ControllerBase
    {
        private readonly IUserDeviceService _userDeviceService;

        public UserDevicesController(IUserDeviceService userDeviceService)
        {
            _userDeviceService = userDeviceService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice([FromBody] string fcmToken, CancellationToken ct = default)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                return BadRequest("FCM Token is required.");
            }

            await _userDeviceService.RegisterDeviceAsync(userId, fcmToken, ct);
            return Ok();
        }

        [HttpPost("unregister")]
        public async Task<IActionResult> UnregisterDevice([FromBody] string fcmToken, CancellationToken ct = default)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(fcmToken))
            {
                return BadRequest("FCM Token is required.");
            }

            await _userDeviceService.UnregisterDeviceAsync(userId, fcmToken, ct);
            return Ok();
        }
    }
}
