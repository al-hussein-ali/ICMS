using ICMS.Domain.Entites.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IUserDeviceRepository : IRepository<UserDevice, int>
    {
        Task<List<string>> GetFcmTokensForUsersAsync(IEnumerable<int> userIds, CancellationToken ct = default);
        Task<List<UserDevice>> GetDevicesByTokensAsync(IEnumerable<string> fcmTokens, CancellationToken ct = default);
    }
}
