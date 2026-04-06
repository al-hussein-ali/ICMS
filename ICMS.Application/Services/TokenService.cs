using ICMS.Application.Interfaces.Services;
using ICMS.Domain.Entites.Common;
using ICMS.Domain.Entites.Identity;
using ICMS.Domain.Entites.Clinical;
using ICMS.Domain.Entites.Maternal;
using ICMS.Domain.Entites.Visits;
using ICMS.Domain.Entites.Audit;
using ICMS.Domain.Entites.Geography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using ICMS.Application.Settings;

namespace ICMS.Application.Services;

public class TokenService(IRefreshTokenService refreshTokenService,JwtOptions jwtOptions) : ITokenService
{
    public string GenerateAccessToken(User user, IList<string> roles)
    {

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Name,user.UserName), 
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role,role)));
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256),
            
            
            Subject = new ClaimsIdentity(claims),
            
            Expires = DateTime.Now.AddMinutes(jwtOptions.ExpiresIn),
        };


        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        
        var accessToken = tokenHandler.WriteToken(securityToken);
        return  accessToken;   
    }



    public async Task<string> GenerateRefreshToken(int userId, CancellationToken cancellationToken = default)
    {
        await refreshTokenService.InvalidateUserRefreshTokensAsync(userId, cancellationToken);
        
        var newToken =  await refreshTokenService.GenerateRefreshTokenAsync(userId, cancellationToken);
        
        return newToken.Token;
    }

  
}
