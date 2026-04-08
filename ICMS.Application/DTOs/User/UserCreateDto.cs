using System.Collections.Generic;

namespace ICMS.Application.DTOs.User
{
    public record UserCreateDto(string UserName, string Password, bool IsActive, int PersonId, IEnumerable<string>? Roles = null);
}
