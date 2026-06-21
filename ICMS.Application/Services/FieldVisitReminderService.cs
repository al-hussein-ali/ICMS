using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Exceptions;
using ICMS.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class FieldVisitReminderService : IFieldVisitReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILocalizer _localizer;
        private readonly ILogger<FieldVisitReminderService> _logger;

        public FieldVisitReminderService(
            IUnitOfWork unitOfWork,
            IPushNotificationService pushNotificationService,
            ILocalizer localizer,
            ILogger<FieldVisitReminderService> logger)
        {
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<bool> SendRemindersForVisitAsync(int fieldVisitId, CancellationToken ct = default)
        {
            _logger.LogInformation("Manually dispatching reminders for FieldVisit ID {Id}", fieldVisitId);

            var fv = await _unitOfWork.FieldVisitRepository.GetByIdWithDetailsAsync(fieldVisitId, ct);
            if (fv == null)
                throw new NotFoundException("FieldVisitNotFound");

            if (fv.ReminderSent)
                throw new DomainException("ReminderAlreadySent");

            var individualIds = await GetTargetedIndividualIdsAsync(fv, ct);
            if (!individualIds.Any())
            {
                _logger.LogWarning("No targeted individuals found for FieldVisit ID {Id}", fieldVisitId);
                return false;
            }

            var deviceTokens = await GetDeviceTokensAsync(individualIds, ct);
            if (!deviceTokens.Any())
            {
                _logger.LogWarning("No registered devices found for targeted individuals of FieldVisit ID {Id}", fieldVisitId);
                return false;
            }

            // Manual trigger: localizer automatically translates using the active request culture
            var title = _localizer["FieldVisitReminderTitle"];
            var body = _localizer["FieldVisitReminderBody", fv.CampaignName, fv.VisitDate.ToString("yyyy-MM-dd")];

            var result = await SendNotificationsAsync(deviceTokens, title, body, fv.Id, ct);
            
            if (result)
            {
                fv.MarkReminderSent();
                await _unitOfWork.FieldVisitRepository.UpdateAsync(fv, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return result;
        }

        public async Task SendScheduledRemindersAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Starting automated FieldVisit reminder job at {Time} (UTC)", DateTime.UtcNow);

            var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var upcomingVisits = await _unitOfWork.FieldVisitRepository.GetUpcomingVisitsForRemindersAsync(tomorrow, ct);

            if (!upcomingVisits.Any())
            {
                _logger.LogInformation("No upcoming FieldVisits scheduled for tomorrow ({Tomorrow}) requiring reminders.", tomorrow);
                return;
            }

            foreach (var fv in upcomingVisits)
            {
                var individualIds = await GetTargetedIndividualIdsAsync(fv, ct);
                if (!individualIds.Any()) continue;

                var deviceTokens = await GetDeviceTokensAsync(individualIds, ct);
                if (!deviceTokens.Any()) continue;

                var dateStr = fv.VisitDate.ToString("yyyy-MM-dd");
                var title = _localizer["ScheduledFieldVisitReminderTitle"];
                var body = _localizer["ScheduledFieldVisitReminderBody", fv.CampaignName, dateStr];

                var result = await SendNotificationsAsync(deviceTokens, title, body, fv.Id, ct);
                if (result)
                {
                    fv.MarkReminderSent();
                    await _unitOfWork.FieldVisitRepository.UpdateAsync(fv, ct);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Completed automated FieldVisit reminder job.");
        }

        private async Task<List<int>> GetTargetedIndividualIdsAsync(FieldVisit fv, CancellationToken ct)
        {
            // Explicitly loaded individuals
            var explicitIds = fv.FieldVisitIndividuals
                .Select(fvi => fvi.VaccinatedIndividualId)
                .ToList();

            if (explicitIds.Any())
            {
                return explicitIds;
            }

            // Fallback: Query individuals with missed schedules matching the criteria
            var fallbackIds = await _unitOfWork.VaccinatedIndividualRepository.GetTargetedIndividualIdsForReminderAsync(fv.SubNeighborhoodId, fv.FromDate, fv.ToDate, ct);

            return fallbackIds;
        }

        private async Task<List<string>> GetDeviceTokensAsync(List<int> individualIds, CancellationToken ct)
        {
            var userIds = await _unitOfWork.VaccinatedIndividualRepository.GetUserIdsForIndividualsAsync(individualIds, ct);

            if (!userIds.Any()) return new List<string>();

            return await _unitOfWork.UserDeviceRepository.GetFcmTokensForUsersAsync(userIds, ct);
        }

        private async Task<bool> SendNotificationsAsync(IReadOnlyList<string> deviceTokens, string title, string body, int visitId, CancellationToken ct)
        {
            var data = new Dictionary<string, string>
            {
                { "type", "field_visit_reminder" },
                { "visitId", visitId.ToString() }
            };

            var result = await _pushNotificationService.SendMulticastNotificationAsync(
                deviceTokens,
                title,
                body,
                null,
                data,
                ct);

            if (result.UnregisteredTokens.Any())
            {
                _logger.LogInformation("Cleaning up {Count} expired FCM tokens.", result.UnregisteredTokens.Count);
                var expiredDevices = await _unitOfWork.UserDeviceRepository.GetDevicesByTokensAsync(result.UnregisteredTokens, ct);

                foreach (var device in expiredDevices)
                {
                    await _unitOfWork.UserDeviceRepository.DeleteAsync(device, ct);
                }
            }

            return result.IsSuccess;
        }
    }
}
