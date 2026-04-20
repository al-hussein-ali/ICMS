using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IUserDeviceService
    {
        Task RegisterDeviceAsync(int userId, string fcmToken, CancellationToken ct = default);
        Task UnregisterDeviceAsync(int userId, string fcmToken, CancellationToken ct = default);
    }
}
