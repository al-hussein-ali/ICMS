using ICMS.Domain.Entites.Identity;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Repositories
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);
        Task DeleteAsync(int userId, int roleId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
