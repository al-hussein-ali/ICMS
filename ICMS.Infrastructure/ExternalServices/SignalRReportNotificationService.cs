using ICMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.ExternalServices
{
    /// <summary>
    /// Sends real-time notifications via SignalR.
    /// Uses IHubContext&lt;Hub&gt; (base class) so Infrastructure has no dependency on ICMS.API.
    /// In Program.cs, this is registered via a factory that supplies IHubContext&lt;ReportHub&gt;
    /// cast to IHubContext&lt;Hub&gt;, which is valid since ReportHub : Hub.
    /// </summary>
    public class SignalRReportNotificationService : IReportNotificationService
    {
        private readonly IHubContext<Hub> _hubContext;

        public SignalRReportNotificationService(IHubContext<Hub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyReportReadyAsync(string userId, string jobId, string downloadUrl, CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("ReportReady", new { jobId, downloadUrl }, ct);
        }

        public async Task NotifyReportFailedAsync(string userId, string jobId, string errorMessage, CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("ReportFailed", new { jobId, errorMessage }, ct);
        }

        public async Task NotifyGeneralAlertAsync(string type, string title, string message, CancellationToken ct = default)
        {
            await _hubContext.Clients.All.SendAsync("NewAlert", new { type, title, message }, ct);
        }

        public async Task NotifyUserAlertAsync(string userId, string type, string title, string message, CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group($"user-{userId}")
                .SendAsync("NewAlert", new { type, title, message }, ct);
        }
    }
}
