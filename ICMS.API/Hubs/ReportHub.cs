using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ICMS.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time report generation notifications.
    ///
    /// Connection: ws(s)://host/hubs/reports
    /// Auth: pass JWT as query param: ?access_token=eyJ...
    ///
    /// Client events:
    ///   "ReportReady"  → { jobId, downloadUrl }
    ///   "ReportFailed" → { jobId, errorMessage }
    /// </summary>
    [Authorize]
    public class ReportHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                // Group this connection under the user's ID so we can target them
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
