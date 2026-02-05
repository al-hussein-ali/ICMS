using ICMS.Domain.Entites;

namespace ICMS.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(int userId,CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<bool> InvalidateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<bool> InvalidateUserTokensAsync(IEnumerable<RefreshToken> refreshTokens, CancellationToken cancellationToken = default);
}
