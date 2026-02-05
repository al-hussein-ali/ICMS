using System;

namespace ICMS.Application.DTOs.User
{
    public record UserDetailsDto(int Id, string UserName, bool IsActive, int PersonId, DateTime CreatedAt);
}
