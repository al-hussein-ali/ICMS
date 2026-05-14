using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportNotificationService _signalRService;

        public NotificationService(IUnitOfWork unitOfWork, IReportNotificationService signalRService)
        {
            _unitOfWork = unitOfWork;
            _signalRService = signalRService;
        }

        public async Task CreateNotificationAsync(int userId, string title, string message, string? targetUrl = null, string? jobId = null, CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                TargetUrl = targetUrl,
                JobId = jobId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.NotificationRepository.AddAsync(notification, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Also push via SignalR to the specific user
            await _signalRService.NotifyUserAlertAsync(userId.ToString(), "Info", title, message, ct);
        }

        public async Task<List<Notification>> GetNotificationsAsync(int userId, int limit = 50, CancellationToken ct = default)
        {
            return await _unitOfWork.NotificationRepository.GetUserNotificationsAsync(userId, limit, ct);
        }

        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken ct = default)
        {
            await _unitOfWork.NotificationRepository.MarkAsReadAsync(notificationId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task MarkAllAsReadAsync(int userId, CancellationToken ct = default)
        {
            await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(userId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task DeleteNotificationAsync(Guid notificationId, CancellationToken ct = default)
        {
            await _unitOfWork.NotificationRepository.DeleteAsync(notificationId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        public async Task NotifyReportReadyAsync(string userId, string jobId, string downloadUrl, CancellationToken ct = default)
        {
            await _signalRService.NotifyReportReadyAsync(userId, jobId, downloadUrl, ct);
        }

        public async Task NotifyReportFailedAsync(string userId, string jobId, string errorMessage, CancellationToken ct = default)
        {
            await _signalRService.NotifyReportFailedAsync(userId, jobId, errorMessage, ct);
        }
    }
}
