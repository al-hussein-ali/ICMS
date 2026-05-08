using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Clinical;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class BatchExpirationTrackerService : IBatchExpirationTrackerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportNotificationService _notificationService;
        private readonly ILogger<BatchExpirationTrackerService> _logger;

        public BatchExpirationTrackerService(
            IUnitOfWork unitOfWork,
            IReportNotificationService notificationService,
            ILogger<BatchExpirationTrackerService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task TrackExpiringBatchesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Starting Batch Expiration Tracking at {Time} (UTC)", DateTime.UtcNow);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var thresholdDate = today.AddDays(30); // 30 days before expiry

            var expiringBatches = _unitOfWork.BatchRepository.GetQueryable()
                .Where(b => b.IsActive && b.TotalQuantity > 0 && b.ExpiryDate <= thresholdDate && b.ExpiryDate >= today)
                .ToList();

            if (!expiringBatches.Any())
            {
                _logger.LogInformation("No batches expiring soon found.");
                return;
            }

            foreach (var batch in expiringBatches)
            {
                var title = "Batch Expiring Soon";
                var message = $"Batch #{batch.CookNumber} ({batch.BatchName}) expires on {batch.ExpiryDate:yyyy-MM-dd}. Remaining: {batch.TotalQuantity}";

                await _notificationService.NotifyGeneralAlertAsync("warning", title, message, ct);
                
                _logger.LogInformation("Sent expiration alert for Batch {BatchId}", batch.Id);
            }

            _logger.LogInformation("Completed Batch Expiration Tracking. Found {Count} batches.", expiringBatches.Count);
        }
    }
}
