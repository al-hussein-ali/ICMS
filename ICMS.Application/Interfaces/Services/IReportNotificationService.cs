namespace ICMS.Application.Interfaces.Services
{
    /// <summary>
    /// Abstraction for sending real-time report completion notifications
    /// to a specific user via SignalR. Lives in Application layer so it
    /// is not coupled to the SignalR infrastructure.
    /// </summary>
    public interface IReportNotificationService
    {
        Task NotifyReportReadyAsync(string userId, string jobId, string downloadUrl, CancellationToken ct = default);
        Task NotifyReportFailedAsync(string userId, string jobId, string errorMessage, CancellationToken ct = default);
        Task NotifyGeneralAlertAsync(string type, string title, string message, CancellationToken ct = default);
    }
}
