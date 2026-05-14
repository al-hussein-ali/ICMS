using ICMS.Domain.Entites.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification, Guid>
    {
        Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 50, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default);
        Task MarkAllAsReadAsync(int userId, CancellationToken ct = default);
    }
}
