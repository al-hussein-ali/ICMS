using ICMS.Application.Interfaces.Repositories;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites;
using System.Security.Cryptography;

namespace ICMS.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository; 
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = new RefreshToken();

            refreshToken.UserId = userId;

            refreshToken.TokenId = Guid.CreateVersion7();
            refreshToken.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            refreshToken.CreatedAt = DateTime.UtcNow;
            refreshToken.ExpirationDate = refreshToken.CreatedAt.AddDays(7);
            refreshToken.IsRevoked = false;


            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken).ConfigureAwait(false);
            return refreshToken;
        }


        public async Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _refreshTokenRepository.InvalidateRefreshTokenAsync(token,cancellationToken).ConfigureAwait(false);
        }

        public async Task InvalidateUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            var tokens =  await _refreshTokenRepository.GetUserRefreshTokensAsync(userId, cancellationToken);
            
            foreach (var refreshToken in tokens)
            {
                refreshToken.IsRevoked = true;
            }
            
            
            
            await _refreshTokenRepository.InvalidateUserTokensAsync(tokens, cancellationToken).ConfigureAwait(false);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> IsValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token, cancellationToken: cancellationToken);

            if(refreshToken is null) return false;

            return refreshToken.IsRevoked || refreshToken.ExpirationDate <= DateTime.UtcNow;
        }
    }
}
