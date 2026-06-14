using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace ICMS.API.Handlers
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true &&
                   httpContext.User.IsInRole(ICMS.Domain.Constants.Roles.Admin);
        }
    }
}
