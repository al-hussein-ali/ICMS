using FirebaseAdmin.Messaging;
using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.ExternalServices
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;
        private readonly IConfiguration _configuration;

        public PushNotificationService(ILogger<PushNotificationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<PushNotificationResult> SendMulticastNotificationAsync(
            IReadOnlyList<string> deviceTokens,
            string title,
            string body,
            string? imageUrl,
            Dictionary<string, string>? data,
            CancellationToken ct)
        {
            var result = new PushNotificationResult();

            if (deviceTokens == null || !deviceTokens.Any())
            {
                _logger.LogWarning("No device tokens provided for push notification.");
                result.IsSuccess = false;
                return result;
            }

            if (bool.TryParse(_configuration["Firebase:Mock"], out var mock) && mock)
            {
                _logger.LogInformation(
                    "Firebase mock mode enabled. Simulating successful dispatch of push notification '{Title}' to {Count} device(s).",
                    title, deviceTokens.Count);
                result.IsSuccess = true;
                return result;
            }

            // Create a linked cancellation token source with a timeout of 15 seconds to prevent hanging
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(15));
            var linkedToken = cts.Token;

            // Firebase restricts multicast to 500 tokens per request
            var batches = deviceTokens.Chunk(500);
            var success = true;

            foreach (var batch in batches)
            {
                var batchList = batch.ToList();
                var message = new MulticastMessage()
                {
                    Tokens = batchList,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = imageUrl
                    },
                    Data = data
                };

                try
                {
                    var response =
                        await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, linkedToken);
                    _logger.LogInformation(
                        "Successfully sent {SuccessCount} messages. Failed to send {FailureCount} messages.",
                        response.SuccessCount, response.FailureCount);

                    if (response.FailureCount > 0)
                    {
                        for (int i = 0; i < response.Responses.Count; i++)
                        {
                            var res = response.Responses[i];
                            if (!res.IsSuccess)
                            {
                                var errMsg = res.Exception?.Message ?? "Unknown Firebase error";
                                var isUnregistered = errMsg.Contains("Requested entity was not found") ||
                                                     errMsg.Contains("unregistered") ||
                                                     errMsg.Contains("NotFound") ||
                                                     errMsg.Contains("not a valid FCM registration token") ||
                                                     errMsg.Contains("invalid") ||
                                                     errMsg.Contains("Invalid") ||
                                                     errMsg.Contains("INVALID");

                                if (isUnregistered)
                                {
                                    _logger.LogWarning(
                                        "Expired or unregistered token detected: {ErrorMessage}. Marking for automatic removal.",
                                        errMsg);
                                    result.UnregisteredTokens.Add(batchList[i]);
                                }
                                else if (res.Exception is OperationCanceledException ||
                                         res.Exception is TaskCanceledException ||
                                         res.Exception?.InnerException is OperationCanceledException ||
                                         res.Exception?.InnerException is TaskCanceledException ||
                                         errMsg.Contains("canceled") ||
                                         errMsg.Contains("Timeout"))
                                {
                                    _logger.LogWarning(
                                        "Push notification send timed out or was canceled for token. FCM service might be temporarily unreachable.");
                                    success = false;
                                }
                                else
                                {
                                    _logger.LogError("Critical error sending push notification: {ErrorMessage}",
                                        errMsg);
                                    success = false;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException ||
                        ex is TaskCanceledException ||
                        ex.InnerException is OperationCanceledException ||
                        ex.InnerException is TaskCanceledException)
                    {
                        _logger.LogWarning(
                            "Push notification dispatch was canceled or timed out (FCM service may be unreachable).");
                    }
                    else
                    {
                        _logger.LogError(ex, "Failed to send multicast message via Firebase.");
                    }

                    success = false;
                }
            }

            result.IsSuccess = success;
            return result;
        }
    }
}
