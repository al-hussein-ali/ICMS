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
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, ct);
                    _logger.LogInformation("Successfully sent {SuccessCount} messages. Failed to send {FailureCount} messages.",
                        response.SuccessCount, response.FailureCount);
                    
                    if (response.FailureCount > 0)
                    {
                        for (int i = 0; i < response.Responses.Count; i++)
                        {
                            var res = response.Responses[i];
                            if (!res.IsSuccess)
                            {
                                var errMsg = res.Exception.Message;
                                var isUnregistered = errMsg.Contains("Requested entity was not found") || 
                                                      errMsg.Contains("unregistered") || 
                                                      errMsg.Contains("NotFound");

                                if (isUnregistered)
                                {
                                    _logger.LogWarning("Expired or unregistered token detected: {ErrorMessage}. Marking for automatic removal.", errMsg);
                                    result.UnregisteredTokens.Add(batchList[i]);
                                }
                                else
                                {
                                    _logger.LogError("Critical error sending push notification: {ErrorMessage}", errMsg);
                                    success = false;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Failed to send multicast message via Firebase.");
                    success = false;
                }
            }

            result.IsSuccess = success;
            return result;
        }
    }
}
