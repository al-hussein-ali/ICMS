using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ICMS.Domain.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ICMS.Tests.Infrastructure
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                var authHeader = authHeaderValues.ToString();
                if (authHeader.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(token);
                            var claims = jwtToken.Claims.ToList();

                            // Ensure Sub maps to NameIdentifier
                            var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == "sub");
                            if (subClaim != null && !claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                            {
                                claims.Add(new Claim(ClaimTypes.NameIdentifier, subClaim.Value));
                            }

                            // Ensure Name maps to Name
                            var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name || c.Type == "name" || c.Type == "unique_name");
                            if (nameClaim != null && !claims.Any(c => c.Type == ClaimTypes.Name))
                            {
                                claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
                            }

                            // Ensure Roles map to ClaimTypes.Role
                            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role).ToList();
                            foreach (var rc in roleClaims)
                            {
                                if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == rc.Value))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, rc.Value));
                                }
                            }

                            var identity = new ClaimsIdentity(claims, "Test");
                            var principal = new ClaimsPrincipal(identity);
                            var ticket = new AuthenticationTicket(principal, "Test");

                            return Task.FromResult(AuthenticateResult.Success(ticket));
                        }
                        catch
                        {
                            // Fallback on parsing error
                        }
                    }
                }
            }

            var defaultClaims = new[] 
            { 
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, Roles.Admin),
                new Claim(ClaimTypes.Role, Roles.VaccinationManager),
                new Claim(ClaimTypes.Role, Roles.InventoryManager),
                new Claim(ClaimTypes.Role, Roles.ReproductiveHealthManager)
            };
            var defaultIdentity = new ClaimsIdentity(defaultClaims, "Test");
            var defaultPrincipal = new ClaimsPrincipal(defaultIdentity);
            var defaultTicket = new AuthenticationTicket(defaultPrincipal, "Test");

            var result = AuthenticateResult.Success(defaultTicket);

            return Task.FromResult(result);
        }
    }
}
