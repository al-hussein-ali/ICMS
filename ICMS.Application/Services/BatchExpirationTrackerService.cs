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
            // Increased threshold to 90 days for better visibility during testing
            var thresholdDate = today.AddDays(90); 

            // Log total active batches for debugging
            var activeCount = _unitOfWork.BatchRepository.GetQueryable().Count(b => b.IsActive && b.TotalQuantity > 0);
            _logger.LogInformation("Checking expiration for {Count} active batches with inventory.", activeCount);

            var expiringBatches = _unitOfWork.BatchRepository.GetQueryable()
                .Where(b => b.IsActive && b.TotalQuantity > 0 && b.ExpiryDate <= thresholdDate && b.ExpiryDate >= today)
                .ToList();

            if (!expiringBatches.Any())
            {
                _logger.LogInformation("No batches expiring within the next 90 days found.");
                return;
            }

            foreach (var batch in expiringBatches)
            {
                var title = "تنبيه: اقتراب انتهاء صلاحية دفعة";
                var message = $"الدفعة #{batch.CookNumber} ({batch.BatchName}) ستنتهي في {batch.ExpiryDate:yyyy-MM-dd}. الكمية المتبقية: {batch.TotalQuantity}";

                // Also send English version for internationalization if needed, but primary is Arabic as per UI
                await _notificationService.NotifyGeneralAlertAsync("warning", title, message, ct);
                
                _logger.LogInformation("Sent expiration alert for Batch {BatchId} (Cook: {CookNumber})", batch.Id, batch.CookNumber);
            }

            _logger.LogInformation("Completed Batch Expiration Tracking. Found {Count} expiring batches.", expiringBatches.Count);
        }
    }
}
