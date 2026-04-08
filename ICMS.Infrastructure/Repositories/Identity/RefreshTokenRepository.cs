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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _appDbContext;

        public RefreshTokenRepository(AppDbContext appDbContextPool)
        {
            _appDbContext = appDbContextPool;
        }

        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await _appDbContext.AddAsync(refreshToken, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.RefreshTokens.Include(rt => rt.User)
                .AsNoTracking().FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<bool> InvalidateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            _appDbContext.Update(refreshToken);
            return await _appDbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        public async Task<bool> InvalidateUserTokensAsync(IEnumerable<RefreshToken> refreshTokens, CancellationToken cancellationToken = default)
        {
            _appDbContext.UpdateRange(refreshTokens);
            return await _appDbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
