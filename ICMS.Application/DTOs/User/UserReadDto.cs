using System;

namespace ICMS.Application.DTOs.User
{
    public record UserReadDto(int Id, string UserName, bool IsActive, int PersonId);
}
