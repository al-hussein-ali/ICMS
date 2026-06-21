using ICMS.Application.Interfaces.Repositories;
using ICMS.Domain.Entites.Identity;
using ICMS.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Infrastructure.Repositories.Identity
{
    public class UserDeviceRepository : Repository<UserDevice, int>, IUserDeviceRepository
    {
        public UserDeviceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<string>> GetFcmTokensForUsersAsync(IEnumerable<int> userIds, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(ud => userIds.Contains(ud.UserId))
                .Select(ud => ud.FcmToken)
                .Distinct()
                .ToListAsync(ct);
        }

        public async Task<List<UserDevice>> GetDevicesByTokensAsync(IEnumerable<string> fcmTokens, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(ud => fcmTokens.Contains(ud.FcmToken))
                .ToListAsync(ct);
        }
    }
}
