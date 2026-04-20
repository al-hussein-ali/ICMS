using ICMS.Domain.Constants;
using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ICMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.VaccinatedIndividual + "," + Roles.PregnantWoman)]
    [EnableRateLimiting("fixed")]
    public class UserDevicesController(IUserDeviceService userDeviceService) : ControllerBase
    {

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

            await userDeviceService.RegisterDeviceAsync(userId, fcmToken, ct);
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

            await userDeviceService.UnregisterDeviceAsync(userId, fcmToken, ct);
            return Ok();
        }
    }
}
