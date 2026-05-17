using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public class PushNotificationResult
    {
        public bool IsSuccess { get; set; }
        public List<string> UnregisteredTokens { get; set; } = new();
    }

    public interface IPushNotificationService
    {
        Task<PushNotificationResult> SendMulticastNotificationAsync(
            IReadOnlyList<string> deviceTokens, 
            string title, 
            string body, 
            string? imageUrl = null, 
            Dictionary<string, string>? data = null, 
            CancellationToken ct = default);
    }
}
