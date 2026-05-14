using ICMS.Application.DTOs.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ICMS.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<AuthResponseDto> BeneficiaryLoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<AuthResponseDto> FieldVisitWorkerLoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshDto, CancellationToken ct = default);
    }
}
