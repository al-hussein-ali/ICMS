using ICMS.Domain.Entites;

namespace ICMS.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string> roles);
    
    Task<string> GenerateRefreshToken(int userId,CancellationToken cancellationToken = default);
    
}