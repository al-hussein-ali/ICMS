using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshTokenAsync(int userId,CancellationToken cancellationToken = default);
    Task<bool> IsValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task InvalidateUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);

}
