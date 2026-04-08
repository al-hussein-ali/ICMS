using System;
using System.Security.Claims;

namespace ICMS.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid.");
            }
            return userId;
        }
    }
}
