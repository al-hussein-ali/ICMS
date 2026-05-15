using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IPushNotificationService
    {
        Task<bool> SendMulticastNotificationAsync(
            IReadOnlyList<string> deviceTokens, 
            string title, 
            string body, 
            string? imageUrl = null, 
            Dictionary<string, string>? data = null, 
            CancellationToken ct = default);
    }
}
