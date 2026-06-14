using ICMS.Application.DTOs.Auth;
using ICMS.Application.Interfaces;
using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Exceptions;
using ICMS.Application.Utilities;
using ICMS.Application.Extensions;
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
            return await InternalLoginAsync(loginDto, null, ct);
        }

        public async Task<AuthResponseDto> BeneficiaryLoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            return await InternalLoginAsync(loginDto, [ICMS.Domain.Constants.Roles.VaccinatedIndividual, ICMS.Domain.Constants.Roles.PregnantWoman], ct);
        }

        public async Task<AuthResponseDto> FieldVisitWorkerLoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            return await InternalLoginAsync(loginDto, [ICMS.Domain.Constants.Roles.FieldVisitWorker], ct);
        }

        private async Task<AuthResponseDto> InternalLoginAsync(LoginDto loginDto, string[]? requiredRoles, CancellationToken ct = default)
        {
            var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName, ct, u => u.Person);
            if (user == null || !user.IsActive)
            {
                throw new DomainException("UserNotFoundOrInactive");
            }

            if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new DomainException("InvalidCredentials");
            }

            var roles = (await identityService.GetUserRolesAsync(user.Id, ct)).ToList();

            if (requiredRoles != null && !roles.Any(r => requiredRoles.Any(rr => string.Equals(rr, r, System.StringComparison.OrdinalIgnoreCase))))
            {
                throw new UnauthorizedException("UnauthorizedAccess");
            }

            var accessToken = tokenService.GenerateAccessToken(user, roles);
            var refreshToken = await tokenService.GenerateRefreshToken(user.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return new AuthResponseDto(accessToken, refreshToken, user.ToReadDto(roles));
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
                await refreshTokenService.InvalidateUserRefreshTokensAsync(tokenEntity.UserId, ct);
                throw new DomainException("InvalidRefreshToken:SecurityBreach");
            }

            if (tokenEntity.ExpirationDate <= System.DateTime.UtcNow)
            {
                throw new DomainException("InvalidRefreshToken:Expired");
            }

            await refreshTokenService.InvalidateRefreshTokenAsync(tokenEntity, ct);

            var user = await unitOfWork.UserRepository.GetByIdAsync(tokenEntity.UserId, ct, u => u.Person);
            if (user == null || !user.IsActive)
            {
                throw new DomainException("UserNotFoundOrInactive");
            }

            var roles = (await identityService.GetUserRolesAsync(user.Id, ct)).ToList();

            var accessToken = tokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = await tokenService.GenerateRefreshToken(user.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return new AuthResponseDto(accessToken, newRefreshToken, user.ToReadDto(roles));
        }
    }
}
