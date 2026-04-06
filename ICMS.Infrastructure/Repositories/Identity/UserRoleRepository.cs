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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<UserRole> _dbSet;

        public UserRoleRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<UserRole>();
        }

        public async Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(userRole, cancellationToken);
        }

        public async Task DeleteAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        {
            var userRole = await _dbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
            if (userRole != null)
            {
                _dbSet.Remove(userRole);
            }
        }

        public async Task<bool> ExistsAsync(int userId, int roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        }

        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
