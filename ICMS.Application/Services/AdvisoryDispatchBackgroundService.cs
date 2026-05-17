using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class AdvisoryDispatchBackgroundService : IAdvisoryDispatchBackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly ILogger<AdvisoryDispatchBackgroundService> _logger;

        private readonly IConfiguration _configuration;

        public AdvisoryDispatchBackgroundService(
            IUnitOfWork unitOfWork,
            IPushNotificationService pushNotificationService,
            IConfiguration configuration,
            ILogger<AdvisoryDispatchBackgroundService> logger)
        {
            _unitOfWork = unitOfWork;
            _pushNotificationService = pushNotificationService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task DispatchPendingAdvisoriesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Starting Advisory Dispatch background service at {Time} (UTC)", DateTime.UtcNow);

            var localNow = DateTime.UtcNow.AddHours(3);
            var today = DateOnly.FromDateTime(localNow);

            var pendingAdvisories = _unitOfWork.HealthAdvisoryRepository.GetQueryable()
                .Where(a => !a.IsSent && a.ScheduledDate <= today)
                .ToList();

            if (!pendingAdvisories.Any())
            {
                _logger.LogInformation("No pending health advisories to dispatch.");
                return;
            }

            foreach (var advisory in pendingAdvisories)
            {
                var deviceTokens = GetDeviceTokensForTarget(advisory.Target);

                if (deviceTokens.Any())
                {
                    var baseUrl = _configuration["ApiBaseUrl"]?.TrimEnd('/');
                    var fullImageUrl = !string.IsNullOrEmpty(advisory.ImageUrl) && !string.IsNullOrEmpty(baseUrl)
                        ? $"{baseUrl}{advisory.ImageUrl}"
                        : null;

                    var data = new Dictionary<string, string>
                    {
                        { "type", "advisory" },
                        { "id", advisory.Id.ToString() },
                        { "target", advisory.Target.ToString() },
                        { "audience", advisory.Target.ToString() }
                    };

                    if (!string.IsNullOrEmpty(fullImageUrl))
                    {
                        data.Add("image", fullImageUrl);
                    }

                    var result = await _pushNotificationService.SendMulticastNotificationAsync(
                        deviceTokens,
                        advisory.Title,
                        advisory.Content,
                        fullImageUrl,
                        data,
                        ct);

                    if (result.IsSuccess)
                    {
                        advisory.MarkAsSent();
                    }

                    if (result.UnregisteredTokens.Any())
                    {
                        _logger.LogInformation("Cleaning up {Count} unregistered FCM tokens.", result.UnregisteredTokens.Count);
                        var unregisteredDevices = _unitOfWork.UserDeviceRepository.GetQueryable()
                            .Where(ud => result.UnregisteredTokens.Contains(ud.FcmToken))
                            .ToList();

                        foreach (var dev in unregisteredDevices)
                        {
                            await _unitOfWork.UserDeviceRepository.DeleteAsync(dev, ct);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("No devices found for target {Target}. Marking advisory {Id} as sent to avoid continuous retries.", advisory.Target, advisory.Id);
                    advisory.MarkAsSent();
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            _logger.LogInformation("Completed Advisory Dispatch.");
        }

        private System.Collections.Generic.List<string> GetDeviceTokensForTarget(AdviceTarget target)
        {
            var userDeviceRepo = _unitOfWork.UserDeviceRepository.GetQueryable();

            switch (target)
            {
                case AdviceTarget.VaccinatedIndividuals:
                    var vaccinatedUsers = _unitOfWork.VaccinatedIndividualRepository.GetQueryable()
                        .Where(v => v.UserId != null)
                        .Select(v => v.UserId);
                        
                    return userDeviceRepo
                        .Where(ud => vaccinatedUsers.Contains(ud.UserId))
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();

                case AdviceTarget.PregnantWomen:
                    var pregnantUsers = _unitOfWork.PregnantWomanRepository.GetQueryable()
                        .Where(p => p.UserId != null)
                        .Select(p => p.UserId);

                    return userDeviceRepo
                        .Where(ud => pregnantUsers.Contains(ud.UserId))
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();

                case AdviceTarget.All:
                default:
                    return userDeviceRepo
                        .Select(ud => ud.FcmToken)
                        .Distinct()
                        .ToList();
            }
        }
    }
}
