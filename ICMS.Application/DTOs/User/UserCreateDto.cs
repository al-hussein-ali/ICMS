using System.Collections.Generic;
using ICMS.Application.DTOs.Person;

namespace ICMS.Application.DTOs.User
{
    public record UserCreateDto(string UserName, string Password, bool IsActive, int? PersonId, PersonCreateDto? PersonCreateDto, IEnumerable<string>? Roles = null);
}
