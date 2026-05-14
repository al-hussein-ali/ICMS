using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Common;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Common
{
    public class NotificationRepository : Repository<Notification, Guid>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int limit = 50, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default)
        {
            var notification = await _context.Notifications.FindAsync(new object[] { notificationId }, ct);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
            }
        }

        public async Task MarkAllAsReadAsync(int userId, CancellationToken ct = default)
        {
            var notifications = await _dbSet.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(ct);
            foreach (var n in notifications) n.IsRead = true;
        }

        public async Task DeleteAsync(Guid notificationId, CancellationToken ct = default)
        {
            var notification = await _dbSet.FindAsync(new object[] { notificationId }, ct);
            if (notification != null)
            {
                _dbSet.Remove(notification);
            }
        }
    }
}
