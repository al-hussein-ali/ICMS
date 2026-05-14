using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int limit = 50, CancellationToken ct = default)
        {
            var userId = GetUserId();
            var notifications = await _notificationService.GetNotificationsAsync(userId, limit, ct);
            return Ok(notifications);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
        {
            await _notificationService.MarkAsReadAsync(id, ct);
            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken ct = default)
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId, ct);
            return NoContent();
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return 0;
            return int.Parse(userIdClaim);
        }
    }
}
