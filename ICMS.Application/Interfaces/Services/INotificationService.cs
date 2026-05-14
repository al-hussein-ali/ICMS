using ICMS.Domain.Entites.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string title, string message, string? targetUrl = null, string? jobId = null, CancellationToken ct = default);
        Task<List<Notification>> GetNotificationsAsync(int userId, int limit = 50, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(int userId, CancellationToken ct = default);
    }
}
