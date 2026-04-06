using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task AssignRolesToUserAsync(int userId, IEnumerable<string> roleNames, CancellationToken ct = default);
        Task RemoveRolesFromUserAsync(int userId, IEnumerable<string> roleNames, CancellationToken ct = default);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId, CancellationToken ct = default);
        Task<bool> IsUserInRoleAsync(int userId, string roleName, CancellationToken ct = default);
    }
}
