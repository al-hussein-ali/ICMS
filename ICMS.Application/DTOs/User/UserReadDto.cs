using System.Collections.Generic;

namespace ICMS.Application.DTOs.User
{
    public record UserReadDto(int Id, string UserName, bool IsActive, int PersonId, IEnumerable<string>? Roles = null);
}
