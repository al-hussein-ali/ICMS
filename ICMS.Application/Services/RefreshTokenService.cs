using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Identity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class RefreshTokenService(IUnitOfWork unitOfWork) : IRefreshTokenService
    {
        public async Task<RefreshToken> GenerateRefreshTokenAsync(int userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                TokenId = Guid.CreateVersion7(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            refreshToken.ExpirationDate = refreshToken.CreatedAt.AddDays(7);

            await unitOfWork.RefreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return refreshToken;
        }

        public async Task InvalidateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            token.IsRevoked = true;
            await unitOfWork.RefreshTokenRepository.InvalidateRefreshTokenAsync(token, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task InvalidateUserRefreshTokensAsync(int userId, CancellationToken cancellationToken = default)
        {
            var tokens = await unitOfWork.RefreshTokenRepository.GetUserRefreshTokensAsync(userId, cancellationToken);
            var tokenList = tokens.ToList();
            
            foreach (var refreshToken in tokenList)
            {
                refreshToken.IsRevoked = true;
            }

            await unitOfWork.RefreshTokenRepository.InvalidateUserTokensAsync(tokenList, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await unitOfWork.RefreshTokenRepository.GetByTokenAsync(token, cancellationToken);
        }

        public async Task<bool> IsValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var refreshToken = await unitOfWork.RefreshTokenRepository.GetByTokenAsync(token, cancellationToken);

            if (refreshToken is null) return false;

            return !refreshToken.IsRevoked && refreshToken.ExpirationDate > DateTime.UtcNow;
        }
    }
}
