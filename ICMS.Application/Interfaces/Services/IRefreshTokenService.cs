using ICMS.Domain.Entites;

namespace ICMS.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId,CancellationToken cancellationToken = default);
    Task<bool> IsValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task InvalidateUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

}
