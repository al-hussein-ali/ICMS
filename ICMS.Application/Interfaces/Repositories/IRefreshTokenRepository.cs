using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId,CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<bool> InvalidateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<bool> InvalidateUserTokensAsync(IEnumerable<RefreshToken> refreshTokens, CancellationToken cancellationToken = default);
}
