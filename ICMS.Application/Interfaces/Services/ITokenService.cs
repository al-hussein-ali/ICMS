using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;

namespace ICMS.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, IList<string> roles);
    
    Task<string> GenerateRefreshToken(int userId,CancellationToken cancellationToken = default);
    
}
