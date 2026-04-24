using ICMS.Application.DTOs.Auth;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Services
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService) : IAuthService
    {
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName, ct);
            if (user == null || !user.IsActive)
            {
                throw new DomainException("UserNotFoundOrInactive");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new DomainException("InvalidCredentials");
            }

            var roles = (await identityService.GetUserRolesAsync(user.Id, ct)).ToList();
            
            var accessToken = tokenService.GenerateAccessToken(user, roles);
            var refreshToken = await tokenService.GenerateRefreshToken(user.Id, ct);
            await unitOfWork.SaveChangesAsync(ct); // Ensure refresh token is persisted

            return new AuthResponseDto(accessToken, refreshToken);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshDto, CancellationToken ct = default)
        {
            var tokenEntity = await refreshTokenService.GetRefreshTokenAsync(refreshDto.RefreshToken, ct);

            if (tokenEntity == null)
            {
                throw new DomainException("InvalidRefreshToken:NotFound");
            }

            if (tokenEntity.IsRevoked)
            {
                throw new DomainException("InvalidRefreshToken:Revoked");
            }

            if (tokenEntity.ExpirationDate <= System.DateTime.UtcNow)
            {
                throw new DomainException("InvalidRefreshToken:Expired");
            }

            // Invalidate the old token to prevent reuse (Rotation)
            await refreshTokenService.InvalidateRefreshTokenAsync(tokenEntity, ct);

            // Get user to generate new token
            var user = await unitOfWork.UserRepository.GetByIdAsync(tokenEntity.UserId, ct);
            if (user == null || !user.IsActive)
            {
                throw new DomainException("UserNotFoundOrInactive");
            }

            var roles = (await identityService.GetUserRolesAsync(user.Id, ct)).ToList();

            var accessToken = tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = await tokenService.GenerateRefreshToken(user.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return new AuthResponseDto(accessToken, newRefreshToken);
        }
    }
}
