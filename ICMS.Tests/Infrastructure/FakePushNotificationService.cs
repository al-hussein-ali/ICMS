using ICMS.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Tests.Infrastructure
{
    public class FakePushNotificationService : IPushNotificationService
    {
        public Task<PushNotificationResult> SendMulticastNotificationAsync(
            IReadOnlyList<string> deviceTokens, 
            string title, 
            string body, 
            string? imageUrl = null, 
            Dictionary<string, string>? data = null, 
            CancellationToken ct = default)
        {
            return Task.FromResult(new PushNotificationResult
            {
                IsSuccess = true,
                UnregisteredTokens = new List<string>()
            });
        }
    }
}
