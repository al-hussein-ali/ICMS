using System;

namespace ICMS.Application.DTOs.User
{
    public record UserCreateDto(string UserName, string PasswordHash, bool IsActive, int PersonId);
}
