using FirebaseAdmin.Messaging;
using ICMS.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.ExternalServices
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendMulticastNotificationAsync(IReadOnlyList<string> deviceTokens, string title, string body, CancellationToken ct = default)
        {
            if (deviceTokens == null || !deviceTokens.Any())
            {
                _logger.LogWarning("No device tokens provided for push notification.");
                return false;
            }

            // Firebase restricts multicast to 500 tokens per request
            var batches = deviceTokens.Chunk(500);
            var success = true;

            foreach (var batch in batches)
            {
                var message = new MulticastMessage()
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };

                try
                {
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, ct);
                    _logger.LogInformation("Successfully sent {SuccessCount} messages. Failed to send {FailureCount} messages.",
                        response.SuccessCount, response.FailureCount);
                    
                    if (response.FailureCount > 0)
                    {
                        success = false;
                        foreach (var res in response.Responses.Where(r => !r.IsSuccess))
                        {
                            _logger.LogError("Error sending message: {ErrorMessage}", res.Exception.Message);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Failed to send multicast message via Firebase.");
                    success = false;
                }
            }

            return success;
        }
    }
}
